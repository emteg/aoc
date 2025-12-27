using System.Text;

namespace _2017;

public static class Day01
{
    public static string Part1(IEnumerable<string> input)
    {
        string captcha = input.First();
        captcha += captcha[0];
        return SolvePart1(captcha).ToString();
    }

    public static string Part1Sample()
    {
        StringBuilder builder = new();
        builder.AppendLine(Part1(["1122"]));
        builder.AppendLine(Part1(["1111"]));
        builder.AppendLine(Part1(["1234"]));
        builder.AppendLine(Part1(["91212129"]));
        return builder.ToString();
    }

    public static string Part2(IEnumerable<string> input)
    {
        string captcha = input.First();
        captcha += captcha;
        return SolvePart2(captcha).ToString();
    }

    public static string Part2Sample()
    {
        StringBuilder builder = new();
        builder.AppendLine(Part2(["1212"]));
        builder.AppendLine(Part2(["1221"]));
        builder.AppendLine(Part2(["123425"]));
        builder.AppendLine(Part2(["123123"]));
        builder.AppendLine(Part2(["12131415"]));
        return builder.ToString();
    }

    private static int SolvePart1(string captcha)
    {
        int result = 0;

        for (int i = 0; i < captcha.Length - 1; i++)
        {
            char c1 = captcha[i];
            char c2 = captcha[i + 1];
            
            if (c1 == c2)
                result += c1 - 48;
        }
        
        return result;
    }
    
    private static int SolvePart2(string captcha)
    {
        int result = 0;
        int stepSize = captcha.Length / 4;
        for (int i = 0; i < captcha.Length / 2; i++)
        {
            char c1 = captcha[i];
            char c2 = captcha[i + stepSize];
            
            if (c1 == c2)
                result += c1 - 48;
        }
        
        return result;
    }
}