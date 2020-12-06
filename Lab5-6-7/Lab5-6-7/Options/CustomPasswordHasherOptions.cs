using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab5_6_7.Options
{
    public class CustomPasswordHasherOptions
    {
        public string Version { get; set; }

        public static string SectionName = "PasswordHasherVersion";
    }

    public static class PasswordHasherVersion
    {
        public static string[] Versions = { V1 };
        public static int VersionIdentifierLength => 4;

        public const string V1 = "0001";

    }
}
