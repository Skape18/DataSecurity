using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lab4
{
    static class CommonlyUsedPasswordsService
    {
        public static void FillTop100000(List<string> top100) => FillTop(top100, "./Top100000.txt");
        public static void FillTop100(List<string> top100) => FillTop(top100, "./Top100.txt");

        private static void FillTop(List<string> top100, string filePath)
        {
            string line;
            var file = new StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                top100.Add(line.Trim());
            }
        }
    }
}
