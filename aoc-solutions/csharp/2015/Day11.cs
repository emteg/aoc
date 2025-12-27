using Common;

namespace _2015;

public static class Day11
{
    public static string Part1(IEnumerable<string> input)
    {
        return IncrementPasswordUntilValid(input.First());
    }

    public static string Part1Sample() => string.Empty;
    
    public static string Part2(IEnumerable<string> input)
    {
        return IncrementPasswordUntilValid(IncrementPasswordUntilValid(input.First()));
    }
    
    public static string Part2Sample() => string.Empty;
    
    private static string IncrementPasswordUntilValid(string password)
    {
        while (true)
        {
            password = IncrementPassword(password);
            if (IsValid(password))
                return password;
        }
    }
    
    private static string IncrementPassword(string password)
    {
        char[] result = password.ToCharArray();
        bool continueLoop = true;
        int i = password.Length - 1;

        while (continueLoop && i >= 0)
        {
            (char newChar, continueLoop) = IncrementChar(result[i]);
            result[i] = newChar;
            
            if (!continueLoop)
                return new string(result);

            i--;
        }
        
        throw new Exception($"Couldn't increment password '{password}'");
    }
    
    private static (char newChar, bool carry) IncrementChar(char c)
    {
        while (true)
        {
            if (c is 'z')
                return ('a', true);

            c++;
            
            if (c is 'i' or 'o' or 'l') // don't return illegal characters
                continue;

            return (c, false);
        }
    }

    private static bool IsValid(string password)
    {
        if (password.Length != 8)
            return false;

        if (!password.Equals(password, StringComparison.InvariantCultureIgnoreCase))
            return false;

        if (password.Contains('i') || password.Contains('o') || password.Contains('l'))
            return false;

        if (!IncludesAtLeastOneIncreasingStraight(password))
            return false;

        if (!IncludesAtLeastTwoDifferentNonOverlappingPairsOfLetters(password))
            return false;
        
        return true;
    }

    private static bool IncludesAtLeastOneIncreasingStraight(string password)
    {
        for (int i = 0; i < password.Length - 2; i++)
        {
            char c1 = password[i];
            char c2 = password[i + 1];
            char c3 = password[i + 2];

            if (c2 == c1 + 1 && c3 == c2 + 1)
                return true;
        }

        return false;
    }

    private static bool IncludesAtLeastTwoDifferentNonOverlappingPairsOfLetters(string password)
    {
        for (int i = 0; i < password.Length -2; i++)
        {
            char pair1C1 = password[i];
            char pair1C2 = password[i + 1];
            
            if (pair1C1 != pair1C2)
                continue;

            for (int j = i + 2; j < password.Length - 1; j++)
            {
                char pair2C1 = password[j];
                char pair2C2 = password[j + 1];
                
                if (pair2C1 != pair2C2)
                    continue;

                if (pair1C1 != pair2C1)
                    return true;
            }
        }
        return false;
    }
}