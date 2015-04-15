# AnagramFinder Notes

# Problem

For a given word or phrase, find and return all anagrams of the word or phrase found in a dictionary file.

## Example

input: Octopus

sample outputs: copouts, coo puts, coo tups, coop uts, etc.

## Costs

This is a processor-intensive task, so our main problem is going to be speed, especially if we want to deliver near-real-time answers. I've written boggle-style algorithms in the past, and processing a 10x10 grid of characters can take a few seconds just to find distinct words. Finding anagrams is much more costly because we must ensure that we use up all the characters and we are looking possibly to match on a combination of words.

## Assumptions

1. I'll only return unique anagrams (scoot up and up scoot count as duplicates)

2. Words are anagrams of themselves (so for an input: octopus, expect octopus to be part of the result set).

## Algorithmic Approach

Recursion lends itself well to permutation, so we'll start there and just use loops to set up our recursive calls.

1. loop through each (distinct) character and recurse, using the remaining characters to recurse further (accomplishes the same thing as 1-a-i). We only want to use distinct characters to kick off our recursive call to avoid duplication from the beginning.

2. store any completed words along with the remaining characters, spin off a new set of recursive calls using the remaining characters. as more words are completed, store with previously-completed words.

3. if the current pattern doesn't match the pattern of any word in the dictionary (i.e. it can't possibly match any known words), return.

4. once all recursive calls return, find any sets of words that were stored with no remaining characters. remove the original input from the list, since it's almost certainly in there somewhere.

5. return/print results.

Recursive approaches in general are a little tougher on the mind (people just weren't designed to think recursively by nature), but I think use of recursion reduces enough complexity in this case compared to the iterative approach that it is worth doing.

The concern here would be how much of the call stack we're using, but since we're just taking 1 starting character at a time and eliminating negative tests as soon as we find them I think we're pretty safe unless we encounter an excessively long string. In that case we'll probably end up killing the application before our stack overflows.

## For Comparison

It turns out someone was already doing this and has a web interface for it. I'll be using the following URL to benchmark my own tests:

 [http://wordsmith.org/anagram/anagram.cgi?anagram=octopus](http://wordsmith.org/anagram/anagram.cgi?anagram=octopus)

## Peripheral problems to solve

1. Reading in the dictionary

    1. We'll just store it locally in a text file and store the whole thing in memory. It's less than 1MB; I think we can spare that. We don't need to load it directly from the given URL for each request, that's for sure. We'll just load it up on once per App_Start. File.ReadAllLines ought to suffice.

2. Preparing the input (strip all non-alpha characters, compose the characters logically). A regex replace of [^a-zA-Z] with empty string should do the trick.

3. We have a few days, so let's throw together an API and make it public.

## Attempt #1

My first attempt failed to recognize some duplicates and took a few minutes to run the sample input: octopus. It did, however, work. I decided to sort the anagram fragments while building them and terminating early on paths where the same words were being found. This led to removal of the duplicates and a marginal (25%) improvement.

## Attempt #2

For attempt #2 I decided to do a little research. I found an algorithm by Donald Knuth where he compares two strings to decide if they are anagrams. He starts out by alphabetizing them. I figured I could leverage this by alphabetizing my dictionary and input strings. Using this approach and a little trial-and-error, I was able to get the processing time for the "octopus" input down to 100ms or so.

Then I also optimized the code to submit fewer attempts (since I only needed to test distinctly unique alphabetized fragments instead of all possible combinations). I accomplished this by tracking not just "characters left to analyze" but also "characters skipped, to be added back in later". Once a new word is found, if any characters were skipped they get added back into "characters left to analyze" before continuing. With that improvement I got the sample down to 3ms.

## Other Observations

The online anagram finder I linked above appears to use a much smaller dictionary. My algorithm found more than triple the number of anagrams for the sample problem and exponentially more for larger samples. Because of this I added an optional limit into my algorithm (automatically 1000 for the API) to reduce loading times.

My solution still has a few collisions happening. If I had more time to reason it out I'm sure I'd pinpoint where it's happening, but it's specifically happening when a character in the pattern repeats (octopus with two o's, for instance). It's not a huge performance hit (unless of course there are many different repeating characters), but worth mentioning.
