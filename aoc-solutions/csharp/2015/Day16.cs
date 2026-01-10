using Common;

namespace _2015;

public static class Day16
{
    public static string Part1(IEnumerable<string> input)
    {
        Dictionary<string, Func<int, int, bool>> operators = new()
        {
            {"children", IsEqual },
            {"cats", IsEqual},
            {"samoyeds", IsEqual},
            {"pomeranians", IsEqual},
            {"akitas", IsEqual},
            {"vizslas", IsEqual},
            {"goldfish", IsEqual},
            {"trees", IsEqual},
            {"cars", IsEqual},
            {"perfumes", IsEqual},
        };
        
        return FindSue(input, operators).ToString();
    }

    public static string Part1Sample() => "There is no sample input for this puzzle";
    
    public static string Part2(IEnumerable<string> input)
    {
        Dictionary<string, Func<int, int, bool>> operators = new()
        {
            {"children", IsEqual },
            {"cats", SuesValueIsGreaterThan},
            {"samoyeds", IsEqual},
            {"pomeranians", SuesValueIsLessThan},
            {"akitas", IsEqual},
            {"vizslas", IsEqual},
            {"goldfish",SuesValueIsLessThan},
            {"trees", SuesValueIsGreaterThan},
            {"cars", IsEqual},
            {"perfumes", IsEqual},
        };
        
        return FindSue(input, operators).ToString();
    }
    
    public static string Part2Sample() => "There is no sample input for this puzzle";
    
    private static readonly Dictionary<string, int> Detections = new()
    {
        {"children", 3},
        {"cats", 7},
        {"samoyeds", 2},
        {"pomeranians", 3},
        {"akitas", 0},
        {"vizslas", 0},
        {"goldfish", 5},
        {"trees", 3},
        {"cars", 2},
        {"perfumes", 1},
    };
    
    private static int FindSue(
        IEnumerable<string> input, 
        Dictionary<string, Func<int, int, bool>> operators)
    {
        int bestSueNumber = 0;
        int bestSueScore = 0;
        int i = 0;
        foreach (string line in input)
        {
            i++;
            Dictionary<string, int> sue = SueFromLine(line);
            int score = 0;
            foreach ((string detection, int detectedValue) in Detections)
            {
                if (!sue.TryGetValue(detection, out int suesValue))
                    continue;

                bool checkResult = operators[detection](detectedValue, suesValue);
                if (checkResult)
                    score++;
            }

            if (score > bestSueScore)
            {
                bestSueScore = score;
                bestSueNumber = i;
            }
        }

        return bestSueNumber;
    }
    
    private static bool IsEqual(int a, int b) => a == b;
    private static bool SuesValueIsGreaterThan(int detected, int suesValue) => detected < suesValue;
    private static bool SuesValueIsLessThan(int detected, int suesValue) => detected > suesValue;
    
    private static Dictionary<string, int> SueFromLine(string line)
    {
        Dictionary<string, int> result = [];
        string[] properties = line[(line.IndexOf(": ", StringComparison.Ordinal) + 2)..].Split(", ");
        foreach (string property in properties)
        {
            string[] nameAndValue = property.Split(": ");
            string name = nameAndValue[0];
            int value = int.Parse(nameAndValue[1]);
            result[name] = value;
        }
        return result;
    }
}