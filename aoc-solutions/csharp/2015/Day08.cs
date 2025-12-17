namespace _2015;

public static class Day08
{
    public static string Part1(IEnumerable<string> input)
    {
        (int totalCodeLength, int totalMemoryLength) = ExecutePart1(input);

        return (totalCodeLength - totalMemoryLength).ToString();
    }

    public static string Part2(IEnumerable<string> input)
    {
        (int totalCodeLength, int totalNewLength) = ExecutePart2(input);
        
        return (totalNewLength - totalCodeLength).ToString();
    }
    
    private static (int totalCodeLength, int totalMemoryLength) ExecutePart1(IEnumerable<string> lines)
    {
        int totalCodeLength = 0;
        int totalMemoryLength = 0;

        foreach (string line in lines)
        {
            (int codeLength, int memoryLength) = CountCharacters(line);
            totalCodeLength += codeLength;
            totalMemoryLength += memoryLength;
        }

        return (totalCodeLength, totalMemoryLength);
    }

    private static (int codeLength, int memoryLength) CountCharacters(string s)
    {
        int codeLength = s.Length;
        int memoryLength = 0;
        
        for (int i = 1; i < s.Length - 1; i++) // skip opening and closing quotes
        {
            char c = s[i];

            if (c != '\\')
            {
                memoryLength++;
                continue;
            }

            // char is '\'
            
            char c2 = s[i + 1];
            
            if (c2 is '\\' or '"') // sequence is '\\' or '\"' - count & skip 1 char
            {
                memoryLength++;
                i++;
                continue;
            }

            if (c2 is not 'x')
                throw new InvalidOperationException($"Illegal escape sequence '{c}{c2}'");
            
            // sequence is \x-- - count 1 and skip 3 chars
            memoryLength++;
            i += 3;
        }

        return (codeLength, memoryLength);
    }

    private static (int totalOriginalLength, int totalNewLength) ExecutePart2(IEnumerable<string> lines)
    {
        int totalOriginalLength = 0;
        int totalNewLength = 0;
        
        foreach (string line in lines)
        {
            (int originalLength, int newLength) = CountEscapedCharacters(line);
            totalOriginalLength += originalLength;
            totalNewLength += newLength;
        }
        
        return (totalOriginalLength, totalNewLength);
    }

    private static (int originalLength, int newLength) CountEscapedCharacters(string line)
    {
        int originalLength = line.Length;
        int addedCharacters = 2; // open and closing quotes
        
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c is '"' or '\\') 
                addedCharacters += 1;
        }
        
        return (originalLength, originalLength + addedCharacters);
    }
}