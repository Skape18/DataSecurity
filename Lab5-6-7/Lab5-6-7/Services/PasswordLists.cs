using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Lab5_6_7.Services
{
    internal static class PasswordLists
    {
        private static HashSet<string> _top100Passwords;

        public static HashSet<string> GetTop100PasswordList()
        {
            if (_top100Passwords != null)
                return _top100Passwords;

            var assembly = typeof(PasswordLists).GetTypeInfo().Assembly;
            using (var stream = assembly.GetManifestResourceStream("Lab5_6_7.Top100Passwords.txt"))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    _top100Passwords = new HashSet<string>(
                        GetLines(streamReader),
                        StringComparer.OrdinalIgnoreCase);
                }
            }
            return _top100Passwords;
        }

        private static IEnumerable<string> GetLines(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                yield return reader.ReadLine();
            }
        }
    }
}
