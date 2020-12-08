using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lab4.PasswordHashers;

namespace Lab4
{
    class Program
    {
        static Random rnd = new Random();
        static void Main(string[] args)
        {
            var hashingTypes = new [] { HashingTypes.MD5, HashingTypes.Sha1Salt, HashingTypes.Bcrypt };
            var factory = new PasswordHasherFactory();

            foreach (var hashingType in hashingTypes)
            {
                var passwords = GeneratePasswords();
                var hasher = factory.GetHasher(hashingType);
                var result = passwords.AsParallel().Select(hasher.Hash).ToList();

                var csv = new StringBuilder();
                foreach (var (hash, salt) in result)
                {
                    csv.AppendLine($"{hash},{salt}");
                }

                File.WriteAllText($"../../../{hashingType.ToString()}.csv", csv.ToString());
            }
        }

        private static List<string> GeneratePasswords()
        {
            var top100 = new List<string>(100);
            var top100000 = new List<string>(100_000);
            var passwords = new List<string>(100_000);

            CommonlyUsedPasswordsService.FillTop100(top100);
            CommonlyUsedPasswordsService.FillTop100000(top100000);

            for (int i = 0; i < 100_000; i++)
                passwords.Add(rnd.Next(1, 101) switch
                {
                    < 10 => top100[rnd.Next(0, 100)],
                    var x when x >= 10 && x < 80 => top100000[rnd.Next(0, 100_000)],
                    var x when x >= 80 && x < 85 => new Guid().ToString().Substring(0, 10),
                    _ => HumanPasswordGenerator.ApplyHumanLikeStrategy(top100[rnd.Next(0, 100)], rnd)
                });

            return passwords;
        }

    }
    public enum HashingTypes
    {
        MD5,
        Sha1Salt,
        Bcrypt
    }
}
