using Common;

namespace _2024;

public static class Day11
{
    public static string Part1(IEnumerable<string> input)
    {
        ulong[] puzzleInput = input.First().Split(' ').Select(ulong.Parse).ToArray();
        return Run(puzzleInput, 25).ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        ulong[] puzzleInput = input.First().Split(' ').Select(ulong.Parse).ToArray();
        return Run(puzzleInput, 75).ToString();
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private const string Sample = "125 17";
    
    private static ulong Run(ulong[] values, byte iterations)
    {
        ulong result = 0;
        Dictionary<(ulong, byte), ulong> cache = [];
        foreach (ulong value in values) 
            result += Blink(value, iterations, cache);

        return result;
    }

    private static ulong Blink(ulong value, byte iterations, Dictionary<(ulong, byte), ulong> cache)
    {
        ulong result;
        
        if (cache.ContainsKey((value, iterations)))
            return cache[(value, iterations)];
        
        if (iterations == 0)
            return 1;

        if (value == 0)
        {
            result = Blink(1, (byte)(iterations - 1), cache);
            cache.Add((value, iterations), result);
            return result;
        }

        string v = value.ToString();
        if (v.Length % 2 == 0)
        {
            string left = v[..(v.Length / 2)];
            string right = v[(v.Length / 2)..];
            ulong leftVal = ulong.Parse(left);
            ulong rightVal = ulong.Parse(right);
            result = Blink(leftVal, (byte)(iterations - 1), cache) + Blink(rightVal, (byte)(iterations - 1), cache);
            cache.Add((value, iterations), result);
            return result;
        }

        result = Blink(value * 2024, (byte)(iterations -1), cache);
        cache.Add((value, iterations), result);
        return result;
    }
}