using Microsoft.AspNetCore.Identity;
using System;

namespace Lab5_6_7.Services
{
    public class ShaArgon2PasswordHasher : IPasswordHasher<IdentityUser>
    {
        public string HashPassword(IdentityUser user, string password)
        {
            // TODO: implement SHA + Argon2 password hashing
            throw new NotImplementedException();
        }

        public PasswordVerificationResult VerifyHashedPassword(IdentityUser user, string hashedPassword, string providedPassword)
        {
            // TODO: implement SHA + Argon2 password hashing verification
            throw new NotImplementedException();
        }
    }
}
