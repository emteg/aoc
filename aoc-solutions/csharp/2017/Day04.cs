using Common;

namespace _2017;

public static class Day04
{
    public static string Part1(IEnumerable<string> input)
    {
        return input
            .Select(line => new Passphrase(line))
            .Count(passphrase => passphrase.AllWordsUnique)
            .ToString();
    }

    public static string Part1Sample() => Part1(Sample1.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        return input
            .Select(line => new Passphrase(line))
            .Count(passphrase => passphrase.AllWordsUniqueAndNoAnagrams)
            .ToString();
    }
    
    public static string Part2Sample() => Part2(Sample2.Lines());

    private readonly struct Passphrase
    {
        public readonly bool AllWordsUnique;
        public readonly bool AllWordsUniqueAndNoAnagrams;

        public Passphrase(string value)
        {
            string[] words = value.Split(" ");
            HashSet<string> uniqueWords = new(words);
            AllWordsUnique = words.Length == uniqueWords.Count;
            if (!AllWordsUnique)
                return;

            AllWordsUniqueAndNoAnagrams = false;
            for (int wordIndex = 0; wordIndex < words.Length; wordIndex++)
            {
                for (int i = 0; i < words.Length; i++)
                {
                    if (i == wordIndex)
                        continue;
                    
                    string word = words[wordIndex];
                    HashSet<char> wordUniqueChars = new(word);
                    
                    string otherWord = words[i];
                    HashSet<char> otherUniqueChars = new(otherWord);

                    if (!wordUniqueChars.SetEquals(otherUniqueChars)) 
                        continue; 
                    
                    // if we get here: both words contain the same chars
                    // ... but not necessarily the same amounts
                    Dictionary<char, int> wordCharCounts = new();
                    foreach (char c in wordUniqueChars) 
                        wordCharCounts[c] = word.Count(wordChar => wordChar == c);
                        
                    Dictionary<char, int> otherWordCharCounts = new();
                    foreach (char c in otherUniqueChars) 
                        otherWordCharCounts[c] = otherWord.Count(wordChar => wordChar == c);

                    bool isDifferent = false;
                    foreach ((char c, int count) in wordCharCounts)
                    {
                        if (otherWordCharCounts[c] != count) // if any char counts is different...
                        {
                            isDifferent = true; // ...we can stop
                            break;
                        }
                    }
                        
                    if (!isDifferent)
                        return;
                }
            }

            AllWordsUniqueAndNoAnagrams = true;
        }
    }
    
    private const string Sample1 = """
                                  aa bb cc dd ee
                                  aa bb cc dd aa
                                  aa bb cc dd aaa
                                  """;
    
    private const string Sample2 = """
                                  abcde fghij
                                  abcde xyz ecdab
                                  a ab abc abd abf abj
                                  iiii oiii ooii oooi oooo
                                  oiii ioii iioi iiio
                                  """;
}