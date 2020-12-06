using Lab5_6_7.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;

namespace Lab5_6_7.Services
{
    public class BcryptPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
    {
        private CustomPasswordHasherOptions _options;

        public BcryptPasswordHasher(IOptions<CustomPasswordHasherOptions> options)
        {
            _options = options.Value;
        }

        public virtual string HashPassword(TUser user, string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            return _options.Version switch
            {
                PasswordHasherVersion.V1 => PasswordHasherVersion.V1 + BCrypt.Net.BCrypt.EnhancedHashPassword(password),
                _ => throw new ArgumentOutOfRangeException(nameof(_options.Version),
                    "No such password hasher version exists.")
            };
        }

        public virtual PasswordVerificationResult VerifyHashedPassword(TUser user, string versionAndHashedPassword,
            string providedPassword)
        {
            if (versionAndHashedPassword == null)
            {
                throw new ArgumentNullException(nameof(versionAndHashedPassword));
            }

            if (providedPassword == null)
            {
                throw new ArgumentNullException(nameof(providedPassword));
            }

            var version = versionAndHashedPassword.Substring(0, 4);
            var isVerified = version switch
            {
                PasswordHasherVersion.V1 => BCrypt.Net.BCrypt.EnhancedVerify(providedPassword,
                                       versionAndHashedPassword.Substring(4, versionAndHashedPassword.Length - 4)),
                _ => false,
            };

            return isVerified ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }
    }
}
