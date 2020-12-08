using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
   static class HumanPasswordGenerator
    {
        public static string ApplyHumanLikeStrategy(string password, Random rnd)
        {
            var strategies = new Func<string, string>[]
            {
                value => value.Reverse().ToString(),
                value => value + value,
                value => value.Replace("a", "@").Replace("e", "3").Replace("i", "1").Replace("s", "5")
                    .Replace("A", "@").Replace("E", "3").Replace("I", "1").Replace("S", "5"),
                value => value.Split().Select(s => s + s).Aggregate((acc, cur) => acc + cur),
                value => value + "123",
                value => value.Split().Select((s, i) => i % 2 == 0 ? s.ToUpper() : s.ToLower()).Aggregate((acc, cur) => acc + cur),
                value => value + rnd.Next(1960, 2021),
                value => "xXx" + value + "xXx"
            };

            var strategy = strategies[rnd.Next(0, strategies.Length)];
            return strategy(password);
        }
    }
}
