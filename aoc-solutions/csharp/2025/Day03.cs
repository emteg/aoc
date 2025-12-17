using Common;

namespace _2025;

public static class Day03
{
    public static string Part1(IEnumerable<string> input)
    {
        return input
            .Select(str => str.ToCharArray())
            .Select(chars => chars.Select(c => c - 48).ToArray())
            .Select(SumOfTwoLargestInOrderOfAppearance)
            .Sum()
            .ToString();
    }
    
    public static string Part2(IEnumerable<string> lines)
    {
        ulong totalJoltage = 0;
        foreach (string line in lines)
        {
            List<byte> currentJoltageDigits = [];
            ReadOnlySpan<byte> digits = line
                .ToCharArray()
                .Select(c => (byte)(c - 48))
                .ToArray()
                .AsSpan();
            while (currentJoltageDigits.Count < 12) 
                digits = PickHighestSequenceOf12(currentJoltageDigits, digits);

            string currentJoltageStr = new(currentJoltageDigits
                .Select(b => (char)(b + 48))
                .ToArray());
            ulong currentJoltage = ulong.Parse(currentJoltageStr);
            totalJoltage += currentJoltage;
        }

        return totalJoltage.ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private const string Sample = """
        987654321111111
        811111111111119
        234234234234278
        818181911112111
        """;
    
    private static int SumOfTwoLargestInOrderOfAppearance(int[] ints)
    {
        int first = 0;
        int second = 0;
        
        for (int i = 0; i < ints.Length; i++)
        {
            if (i < ints.Length - 1 && ints[i] > first) // never the last digit, only if bigger
            {
                first = ints[i]; // a new, larger first digit has been found
                second = 0; // second digit is reset as its value must come from after first's position
            }
            else if (i > 0 && ints[i] > second) // never the first digit, only if bigger
                second = ints[i];
        }

        int result = 10 * first + second;
        return result;
    }
    
    private static ReadOnlySpan<byte> PickHighestSequenceOf12(List<byte> result, ReadOnlySpan<byte> digits)
    {
        int digitsLeftToPick = 12 - result.Count;
        HashSet<byte> possibleNextDigits = new(digits[..(digits.Length - digitsLeftToPick + 1)].ToArray());
        byte max = possibleNextDigits.Max();
        result.Add(max);
        ReadOnlySpan<byte> remainder = digits[(digits.IndexOf(max) + 1)..];
        return remainder;
    }
}