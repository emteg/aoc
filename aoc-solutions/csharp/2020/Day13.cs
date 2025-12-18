using Common;

namespace _2020;

public static class Day13
{
    public static string Part1(IEnumerable<string> input)
    {
        int desiredDeparture = int.Parse(input.First());
        int[] busIds = input
            .Skip(1)
            .First()
            .Split(',')
            .Where(it => it != "x")
            .Select(int.Parse)
            .ToArray();

        int earliestId = 0;
        int bestWaitTime = int.MaxValue;
        
        foreach (int busId in busIds)
        {
            int closestDeparture = (desiredDeparture / busId) * busId;
            if (closestDeparture < desiredDeparture)
                closestDeparture += busId;
            int waitTime = closestDeparture - desiredDeparture;
            if (waitTime < bestWaitTime)
            {
                bestWaitTime = waitTime;
                earliestId = busId;
            }
        }

        int result = earliestId * bestWaitTime;
        return result.ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        return string.Empty;
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private const string Sample = """
                                  939
                                  7,13,x,x,59,x,31,19
                                  """;
}