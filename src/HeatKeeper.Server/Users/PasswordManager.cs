using Microsoft.AspNetCore.Identity;

namespace HeatKeeper.Server.Users
{
    public class PasswordManager : IPasswordManager
    {
        private readonly PasswordHasher<PasswordManager> passwordHasher = new PasswordHasher<PasswordManager>();

        public string GetHashedPassword(string password)
        {
            return passwordHasher.HashPassword(this, password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return passwordHasher.VerifyHashedPassword(this, hashedPassword, password) == PasswordVerificationResult.Success;
        }
    }
}
