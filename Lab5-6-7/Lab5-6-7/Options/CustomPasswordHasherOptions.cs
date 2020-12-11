using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab5_6_7.Options
{
    public class CustomPasswordHasherOptions
    {
        public string Version { get; set; }
        public string Pepper { get; set; }

        public static string SectionName = "CustomPasswordHasherOptions";
    }

    public static class PasswordHasherVersion
    {
        public static string[] Versions = { V1 };
        public static int VersionIdentifierLength => 3;

        public const string V1 = "001";
    }
}
