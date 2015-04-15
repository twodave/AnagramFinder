using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnagramFinder.Engine {
    public class Finder {
        public IEnumerable<string> FindAnagrams2(string inputPhrase, int? limit = null, Action<String> log = null) {
            // alpha characters only, please
            bool loggingEnabled = log != null;

            if (loggingEnabled) {
                log(String.Format("input phrase: {0}", inputPhrase));
            }

            if (loggingEnabled) {
                log("Filtering input phrase...");
            }

            string filteredPhrase = DictionaryLoader.AlphabetizeString(FilterInput(inputPhrase).ToLower());

            if (loggingEnabled) {
                log(String.Format("filtered phrase: {0}", filteredPhrase));
            }

            if (!anagramsFound.ContainsKey(filteredPhrase)) {
                anagramsFound[filteredPhrase] = new List<string>();
                PerformSearch2(filteredPhrase, String.Empty, filteredPhrase, String.Empty, new List<string>(), limit);
            }
            else {
                // we already ran this one through, let's just return the cached result
            }

            // still getting duplicates when a character repeats
            return anagramsFound[filteredPhrase].Distinct();
        }

        private IEnumerable<string> GetWordsMatchingPattern(string alphabetizedPattern) {
            if (anagramKey.ContainsKey(alphabetizedPattern)) {
                return anagramKey[alphabetizedPattern];
            }
            else {
                return new string[] { };
            }
        }

        private void PerformSearch2(string originalPhrase, string pattern, string remainingCharacters, string charactersToAddBackForTheNextWord, List<string> wordsFoundSoFar, int? limit) {
            if (anagramsFound[originalPhrase].Count == (limit ?? -1)) return;
            foreach (string word in GetWordsMatchingPattern(pattern)) {
                List<string> newWordList = new List<string>(wordsFoundSoFar);
                newWordList.Add(word);
                newWordList.Sort();
                string newRemainingCharacters = remainingCharacters + charactersToAddBackForTheNextWord;
                if (newRemainingCharacters.Length > 0) {
                    // not done yet, let's continue the search with a fresh empty pattern and an expanded word list
                    PerformSearch2(originalPhrase, String.Empty, newRemainingCharacters, String.Empty, newWordList, limit);
                }
                else {
                    string anagram = String.Join(" ", newWordList);
                    // we're done with this path, let's add the result to our dictionary.
                    anagramsFound[originalPhrase].Add(anagram);
                }
            }
            

            if (remainingCharacters.Length == 0) {
                return; // no characters left, need to quit here
            }

            // let's keep plugging along. add all possible next characters to the pattern and fire off another round
            List<char> searchedAlready = new List<char>();
            for (int i = 0; i < remainingCharacters.Length; i++) {
                if (searchedAlready.Contains(remainingCharacters[i])) {
                    continue; // avoid duplicate searches
                }

                string newPattern = pattern + remainingCharacters[i].ToString();
                string newRemainingCharacters = remainingCharacters.Substring(i + 1);
                string newCharactersToAddBack = charactersToAddBackForTheNextWord + remainingCharacters.Substring(0, i);
                PerformSearch2(originalPhrase, newPattern, newRemainingCharacters, newCharactersToAddBack, wordsFoundSoFar, limit);
                searchedAlready.Add(remainingCharacters[i]);
            }
        }

        /// <summary>
        /// Find all of the anagrams for a given input phrase
        /// </summary>
        /// <param name="inputPhrase">The input phrase to search</param>
        /// <param name="log">For verbose output, supply this with a delegate of your choice.</param>
        /// <returns></returns>
        public IEnumerable<string> FindAnagrams1(string inputPhrase, Action<String> log = null) {
            // alpha characters only, please
            bool loggingEnabled = log != null;

            if (loggingEnabled) {
                log(String.Format("input phrase: {0}", inputPhrase));
            }

            if (loggingEnabled) {
                log("Filtering input phrase...");
            }

            string filteredPhrase = FilterInput(inputPhrase).ToLower();

            if (loggingEnabled) {
                log(String.Format("filtered phrase: {0}", filteredPhrase));
            }

            if (!anagramsFound.ContainsKey(filteredPhrase)) {
                anagramsFound[filteredPhrase] = new List<string>();
                PerformSearch1(filteredPhrase, String.Empty, filteredPhrase, new List<string>());
            }
            else {
                // we already ran this one through, let's just return the cached result
            }

            return anagramsFound[filteredPhrase];
        }

        Dictionary<string, List<string>> anagramsFound = new Dictionary<string, List<string>>();

        private void PerformSearch1(string originalPhrase, string pattern, string remainingCharacters, List<string> wordsFoundSoFar){
            bool patternIsAWord = IsWord(pattern);
            bool patternMatchesALargerWord = pattern == String.Empty || IsBeginningOfLargerWord(pattern);

            string anagramFragment = String.Format("{0} {1}",String.Join(" ", wordsFoundSoFar),pattern);

            // this will only find paths we both analyzed and found words for. it could be improved.
            bool pathAlreadyAnalyzed = anagramsFound[originalPhrase].Any(anagram => anagram.StartsWith(anagramFragment));

            // following cases are possible:

            // 1. pattern is a word and we have characters left to play with (temp. store, reset & continue)
            // 2. pattern is a word and we have are out of characters (final store & terminate)
            // 3. pattern starts a longer word and we have characters left to play with (add character continue)
            // 4. pattern starts a longer word and we are out of characters (terminate)
            // 5. pattern is not a word and does not start a longer word (terminate)
            // 6. pattern matches a path we have already traversed (terminate)

            // exit case, either pattern isn't a word and it also doesn't match a longer word OR we've already been down this path.
            if (pathAlreadyAnalyzed || (!patternIsAWord && !patternMatchesALargerWord)) {
                return; // done with this path
            }

            // pattern is a word
            if (patternIsAWord) {
                List<string> newWordList = new List<string>(wordsFoundSoFar);
                newWordList.Add(pattern);
                newWordList.Sort();

                string anagram = String.Join(" ", newWordList);

                if (remainingCharacters.Length > 0) {
                    // not done yet, let's continue the search with a fresh empty pattern and an expanded word list
                    PerformSearch1(originalPhrase, String.Empty, remainingCharacters, newWordList);
                }
                else {
                    // we're done with this path, let's add the result to our dictionary.
                    anagramsFound[originalPhrase].Add(anagram);

                    return; // need to return here or we could end up searching an empty list of remaining characters below
                }
            }

            // pattern matches a larger word
            if (patternMatchesALargerWord) {
                if (remainingCharacters.Length == 0) {
                    return; // no characters left, need to quit here
                }
                // let's keep plugging along. add all possible next characters to the pattern and fire off another round
                List<char> searchedAlready = new List<char>();
                for (int i = 0; i < remainingCharacters.Length; i++) {
                    if (searchedAlready.Contains(remainingCharacters[i])) {
                        continue; // avoid duplicate searches
                    }

                    string newPattern = pattern + remainingCharacters[i].ToString();
                    string newRemainingCharacters = remainingCharacters.Substring(0, i) + remainingCharacters.Substring(i + 1);
                    PerformSearch1(originalPhrase, newPattern, newRemainingCharacters, wordsFoundSoFar);
                    searchedAlready.Add(remainingCharacters[i]);
                }
            }
        }

        private static bool IsBeginningOfLargerWord(string pattern) {
            return wordList.Any(word => word.Length > pattern.Length && word.StartsWith(pattern));
        }

        private static bool IsWord(string pattern) {
            return wordList.Contains(pattern);
        }

        private static string[] wordList { get { return DictionaryLoader.GetWordList(); } }
        private static Dictionary<string, List<string>> anagramKey { get { return DictionaryLoader.GetAnagramKey(); } }

        private static string FilterInput(string phrase) {
            Regex filter = new Regex("[^a-zA-Z]");
            phrase = filter.Replace(phrase, String.Empty);
            return phrase;
        }
    }
}
