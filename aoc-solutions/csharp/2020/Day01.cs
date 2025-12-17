using Common;

namespace _2020;

public static class Day01
{
    public static string Part1(IEnumerable<string> input)
    {
        uint[] numbers = input.Select(uint.Parse).ToArray();

        for (int i = 0; i < numbers.Length - 1; i++)
        {
            for (int j = i + 1; j < numbers.Length; j++)
            {
                if (numbers[i] + numbers[j] == 2020u)
                    return (numbers[i] * numbers[j]).ToString();
            }
        }

        return string.Empty;
    }

    public static string Part1Sample() => Part1(Sample.Lines());

    public static string Part2(IEnumerable<string> input)
    {
        ulong[] numbers = input.Select(ulong.Parse).ToArray();

        for (int i = 0; i < numbers.Length - 2; i++)
        {
            for (int j = i + 1; j < numbers.Length - 1; j++)
            {
                if (numbers[i] + numbers[j] > 2020u)
                    continue;
                
                for (int k = j + 1; k < numbers.Length; k++)
                {
                    if (numbers[i] + numbers[j] + numbers[k] == 2020u)
                        return (numbers[i] * numbers[j] * numbers[k]).ToString();
                }
            }
        }

        return string.Empty;
    }

    public static string Part2Sample() => Part2(Sample.Lines());
    
    private const string Sample = """
        1721
        979
        366
        299
        675
        1456
        """;
}