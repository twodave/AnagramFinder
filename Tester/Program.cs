using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnagramFinder.Engine;

namespace AnagramFinder.Tester {
    class Program {
        static void Main(string[] args) {
            DictionaryLoader.LoadAlphabetizedKey();

            Action<String> logger = (message) => {
                string template = "{0:hh:mm:ss.fff}: {1}";
                Console.WriteLine(String.Format(template, DateTime.Now, message));
            };

            var results = new Finder().FindAnagrams2("bottom",5000, logger).ToList();

            logger("Results:");

            foreach (string result in results) {
                logger(result);
            }
            Console.WriteLine(Environment.NewLine + "Press any key to exit...");
            Console.ReadKey();
        }
    }
}
