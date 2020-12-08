using System.Security.Cryptography;
using System.Text;

namespace Lab4.PasswordHashers
{
    public class PasswordHasherFactory
    {
        private static MD5Hasher _md5Hasher = new MD5Hasher();
        private static Sha1Hasher _sha1Hasher = new Sha1Hasher();
        private static BcryptHasher _bcryptHasher = new BcryptHasher();

        public IPasswordHasher GetHasher(HashingTypes types) => types switch
        {
            HashingTypes.MD5 => _md5Hasher,
            HashingTypes.Sha1Salt => _sha1Hasher,
            HashingTypes.Bcrypt => _bcryptHasher
        };
    }

    public class BcryptHasher : IPasswordHasher
    {
        public (string, string) Hash(string password)
        {
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            return (BCrypt.Net.BCrypt.HashPassword(password, salt), salt);
        }
    }

    public class Sha1Hasher : IPasswordHasher
    {
        public (string, string) Hash(string password)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var salt = BCrypt.Net.BCrypt.GenerateSalt();
                var toHash = salt + password;
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(toHash));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return (sb.ToString(), salt);
            }
        }
    }

    public class MD5Hasher : IPasswordHasher
    {
        public (string, string) Hash(string password)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
                var hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (var hashByte in hashBytes)
                {
                    sb.Append(hashByte.ToString("X2"));
                }
                return (sb.ToString(), "");
            }
        }
    }

    public interface IPasswordHasher
    {
        (string hash, string salt) Hash(string password);
    }
}
