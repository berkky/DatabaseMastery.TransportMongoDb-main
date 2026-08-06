using DatabaseMastery.TransportMongoDb.Entities;
using Microsoft.AspNetCore.Identity;

namespace DatabaseMastery.TransportMongoDb.Security
{
    public class AdminCredentialService : IAdminCredentialService
    {
        private readonly IPasswordHasher<AdminUser> _passwordHasher;

        public AdminCredentialService(IPasswordHasher<AdminUser> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public string NormalizeUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username is required.", nameof(username));
            }

            return username.Trim().ToUpperInvariant();
        }

        public string HashPassword(AdminUser adminUser, string password)
        {
            ArgumentNullException.ThrowIfNull(adminUser);

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password is required.", nameof(password));
            }

            return _passwordHasher.HashPassword(adminUser, password);
        }

        public PasswordVerificationResult VerifyPassword(
            AdminUser adminUser,
            string password)
        {
            ArgumentNullException.ThrowIfNull(adminUser);

            if (string.IsNullOrEmpty(password))
            {
                return PasswordVerificationResult.Failed;
            }

            return _passwordHasher.VerifyHashedPassword(
                adminUser,
                adminUser.PasswordHash,
                password);
        }
    }
}
