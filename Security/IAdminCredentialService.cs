using DatabaseMastery.TransportMongoDb.Entities;
using Microsoft.AspNetCore.Identity;

namespace DatabaseMastery.TransportMongoDb.Security
{
    public interface IAdminCredentialService
    {
        string NormalizeUsername(string username);

        string HashPassword(AdminUser adminUser, string password);

        PasswordVerificationResult VerifyPassword(AdminUser adminUser, string password);
    }
}
