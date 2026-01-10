using Common;

namespace _2024;

public static class Day01
{
    public static string Part1(IEnumerable<string> input)
    {
        List<int> left = [];
        List<int> right = [];
        
        foreach (string line in input)
        {
            int[] values = line.Split("   ").Select(int.Parse).ToArray();
            left.Add(values[0]);
            right.Add(values[1]);
        }
        left.Sort();
        right.Sort();
        
        int result = left
            .Select((leftValue, index) => Math.Abs(leftValue - right[index]))
            .Sum();
        
        return result.ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        List<int> left = [];
        Dictionary<int, int> valueOccurrencesInRightList = [];
        
        foreach (string line in input)
        {
            int[] values = line.Split("   ").Select(int.Parse).ToArray();
            int leftValue = values[0];
            int rightValue = values[1];
            left.Add(leftValue);
            if (valueOccurrencesInRightList.TryGetValue(rightValue, out _))
                valueOccurrencesInRightList[rightValue] += 1;
            else
                valueOccurrencesInRightList.Add(rightValue, 1);
        }

        int result = left
            .Where(leftValue => valueOccurrencesInRightList.ContainsKey(leftValue))
            .Sum(leftValue => leftValue * valueOccurrencesInRightList[leftValue]);

        return result.ToString();
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private const string Sample = """
                                  3   4
                                  4   3
                                  2   5
                                  1   3
                                  3   9
                                  3   3
                                  """;
}