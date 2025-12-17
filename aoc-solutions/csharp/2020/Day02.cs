using Common;

namespace _2020;

public static class Day02
{
    public static string Part1(IEnumerable<string> input)
    {
        return input
            .Select(PolicyAndPassword.Parse)
            .Count(PasswordIsValidPart1)
            .ToString();
    }
    
    public static string Part2(IEnumerable<string> input)
    {
        return input
            .Select(PolicyAndPassword.Parse)
            .Count(PasswordIsValidPart2)
            .ToString();
    }
    
    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private const string Sample = """
                                  1-3 a: abcde
                                  1-3 b: cdefg
                                  2-9 c: ccccccccc
                                  """;

    private static bool PasswordIsValidPart1(PolicyAndPassword policy)
    {
        int count = policy.Password.Count(it => it == policy.Char);
        return policy.FirstNumber <= count && count <= policy.SecondNumber;
    }
    
    private static bool PasswordIsValidPart2(PolicyAndPassword policy)
    {
        return policy.Password[policy.FirstNumber] == policy.Char ^
               policy.Password[policy.SecondNumber] == policy.Char;
    }

    private readonly struct PolicyAndPassword
    {
        public readonly int FirstNumber;
        public readonly int SecondNumber;
        public readonly char Char;
        public readonly string Password;

        public PolicyAndPassword(int firstNumber, int secondNumber, char c, string password)
        {
            FirstNumber = firstNumber;
            SecondNumber = secondNumber;
            Char = c;
            Password = password;
        }

        public static PolicyAndPassword Parse(string input)
        {
            int indexOfDash = input.IndexOf('-');
            string firstNumStr = input[..indexOfDash];
            input = input[(indexOfDash + 1)..];
            
            int indexOfSpace = input.IndexOf(' ');
            string secondNumStr = input[..indexOfSpace];
            input = input[(indexOfSpace + 1)..];

            char c = input[0];
            string password = input[2..];
            
            return new PolicyAndPassword(int.Parse(firstNumStr), int.Parse(secondNumStr), c, password);
        }
        
        public override string ToString() => $"{FirstNumber}-{SecondNumber} {Char}: {Password}";
    }
}