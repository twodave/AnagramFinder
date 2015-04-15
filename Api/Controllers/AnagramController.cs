using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers {
    public class AnagramController : ApiController {
        // GET api/anagram/octopus
        public IEnumerable<string> Get(string word) {
            if (String.IsNullOrWhiteSpace(word)) return new string[] { };
            return new AnagramFinder.Engine.Finder().FindAnagrams2(word, 1000);
        }
    }
}