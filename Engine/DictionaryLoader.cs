using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AnagramFinder.Engine {
    public static class DictionaryLoader {
        public static string PathToWordList {
            get {
                if (HttpContext.Current != null) {
                    return Path.Combine(HttpContext.Current.Server.MapPath("~/bin/"), "words.txt");
                }
                else {
                    return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "words.txt");
                }
            }
        }

        private static string[] _wordList = null;

        public static void LoadDictionary() {
            _wordList = File.ReadAllLines(PathToWordList);
        }

        public static string[] GetWordList(bool reload = false) {
            if (reload || _wordList == null || _wordList.Length == 0) {
                LoadDictionary();
            }
            return _wordList;
        }

        public static string AlphabetizeString(string input) {
            char[] wordArray = input.ToCharArray();
            Array.Sort(wordArray);
            string alphabetized = new string(wordArray);
            return alphabetized;
        }

        public static void LoadAlphabetizedKey() {
            alphaKey = new Dictionary<string, List<string>>();
            foreach (string word in GetWordList()) {
                string alphabetized = AlphabetizeString(word);    

                if (!alphaKey.ContainsKey(alphabetized)) {
                    alphaKey.Add(alphabetized, new List<string>());
                }
                alphaKey[alphabetized].Add(word);
            }
        }

        public static Dictionary<string, List<string>> GetAnagramKey(bool reload = false) {
            if (reload || alphaKey == null || alphaKey.Keys.Count == 0) {
                LoadAlphabetizedKey();
            }
            return alphaKey;
        }

        private static Dictionary<String, List<String>> alphaKey { get; set; }
    }
}
