using Common;

namespace _2015;

public static class Day01
{
    public static string Part1(IEnumerable<string> input)
    {
        int result = 0;
        foreach (char c in input.Select(str => str.ToCharArray()).SelectMany(chars => chars))
        {
            if (c == '(')
                result++;
            else if (c == ')')
                result--;
            
        }
        return result.ToString();
    }
    
    public static string Part2(IEnumerable<string> input)
    {
        int result = 0;
        uint position = 0;
        foreach (char c in input.Select(str => str.ToCharArray()).SelectMany(chars => chars))
        {
            position++;
            
            if (c == '(')
                result++;
            else if (c == ')')
                result--;
            
            if (result < 0)
                break;
        }
        return position.ToString();
    }
}