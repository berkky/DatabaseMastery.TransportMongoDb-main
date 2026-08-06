using System.Security.Claims;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Services.AdminUserServices;
using DatabaseMastery.TransportMongoDb.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.Controllers
{
    public class AccountController : Controller
    {
        public const string AuthenticationScheme = "TransportAdmin";

        private const string GenericLoginError = "Kullanıcı adı veya parola hatalı.";

        private readonly IAdminUserService _adminUserService;
        private readonly IAdminCredentialService _credentialService;

        public AccountController(
            IAdminUserService adminUserService,
            IAdminCredentialService credentialService)
        {
            _adminUserService = adminUserService;
            _credentialService = credentialService;
        }

        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToLocalOrDashboard(returnUrl);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new AdminLoginViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Login(
            AdminLoginViewModel model,
            string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string normalizedUsername;
            try
            {
                normalizedUsername = _credentialService.NormalizeUsername(model.Username);
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError(string.Empty, GenericLoginError);
                return View(model);
            }

            var adminUser = await _adminUserService
                .GetByNormalizedUsernameAsync(normalizedUsername);

            if (adminUser is null ||
                !adminUser.IsActive ||
                (adminUser.LockoutEndUtc.HasValue &&
                 adminUser.LockoutEndUtc.Value > DateTime.UtcNow))
            {
                ModelState.AddModelError(string.Empty, GenericLoginError);
                return View(model);
            }

            var verification = _credentialService.VerifyPassword(
                adminUser,
                model.Password);

            if (verification is not (
                PasswordVerificationResult.Success or
                PasswordVerificationResult.SuccessRehashNeeded))
            {
                ModelState.AddModelError(string.Empty, GenericLoginError);
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, adminUser.AdminUserId),
                new(ClaimTypes.Name, adminUser.Username),
                new(ClaimTypes.Role, adminUser.Role)
            };

            var identity = new ClaimsIdentity(claims, AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties
            {
                IsPersistent = false,
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                AuthenticationScheme,
                principal,
                properties);

            return RedirectToLocalOrDashboard(returnUrl);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToLocalOrDashboard(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "AdminLayout");
        }
    }
}
