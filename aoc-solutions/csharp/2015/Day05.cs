namespace _2015;

public static class Day05
{
    public static string Part1(IEnumerable<string> input)
    {
        return input.Count(IsNicePart1).ToString();
    }

    #region Part 1
    
    private static bool IsNicePart1(string s)
    {
        return DoesNotContainAnyBadPairs(s) && ContainsAtLeastThreeVowels(s) && ContainsAtLeastOnePair(s);
    }
    
    private static bool DoesNotContainAnyBadPairs(string s)
    {
        return !s.Contains("ab") && !s.Contains("cd") && !s.Contains("pq") && !s.Contains("xy");
    }
    
    private static bool ContainsAtLeastThreeVowels(string s) => s.Count(IsVowel) >= 3;
    
    private static bool ContainsAtLeastOnePair(string s)
    {
        for (int i = 0; i < s.Length - 1; i++)
        {
            if (s[i] == s[i + 1])
                return true;
        }

        return false;
    }
    
    private static bool IsVowel(char c) => c is 'a' or 'e' or 'i' or 'o' or 'u';
    
    #endregion
    
    public static string Part2(IEnumerable<string> input)
    {
        return input.Count(IsNicePart2).ToString();
    }

    #region Part 2
    
    private static bool IsNicePart2(string s)
    {
        return ContainsPairsOfAnyTwoLettersAtLeastTwice(s) && ContainsAtLeastOneSymmetricTriple(s);
    }
    
    private static bool ContainsPairsOfAnyTwoLettersAtLeastTwice(string s)
    {
        for (int i = 0; i < s.Length - 3; i++)
        {
            string pair = s.Substring(i, 2);
            for (int j = i + 2; j < s.Length - 1; j++)
            {
                string other = s.Substring(j, 2);
                if (pair == other) 
                    return true;
            }
        }

        return false;
    }
    
    private static bool ContainsAtLeastOneSymmetricTriple(string s)
    {
        for (int i = 0; i < s.Length - 2; i++)
        {
            string triple = s.Substring(i, 3);
            if (triple[0] == triple[2])
                return true;
        }
        return false;
    }
    
    #endregion
}