using Common;

namespace _2015;

public static class Day10
{
    public static string Part1(IEnumerable<string> input)
    {
        return LookAndSay(input.First(), 40).Length.ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        return LookAndSay(input.First(), 50).Length.ToString();
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private static string LookAndSay(string s, int iterations)
    {
        while (iterations > 0)
        {
            List<char> chars = [];
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                int howMany = 1;
                for (int j = i + 1; j < s.Length; j++)
                {
                    if (s[j] != c)
                        break;
                    
                    howMany++;
                }

                string howManyStr = howMany.ToString();
                chars.AddRange(howManyStr.ToCharArray());
                chars.Add(c);
                i += howMany - 1;
            }

            string result = new string(chars.ToArray());
            s = result;
            iterations--;
        }

        return s;
    }
    
    private const string Sample = "111221";
}