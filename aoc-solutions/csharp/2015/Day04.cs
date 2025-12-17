using System.Security.Cryptography;
using System.Text;

namespace _2015;

public static class Day04
{
    public static string Part1(IEnumerable<string> input) 
    {
        return Execute(input.First(), 30_000, HashStartsWithFiveZeros).ToString();
    }
    
    public static string Part2(IEnumerable<string> input) 
    {
        return Execute(input.First(), 1_000_000, HashStartsWithSixZeros).ToString();
    }
    
    private static uint Execute(string secretKey, int tenPercentIterations, Func<string, uint, bool> action)
    {
        uint i = 0;
        Console.Error.WriteLine("Hashing...");
        Console.Error.WriteLine("----------");
        while (!action(secretKey, i))
        {
            if (i % tenPercentIterations == 0) 
                Console.Error.Write('#');
            
            if (i == uint.MaxValue)
                break;
            
            i++;
        }
        Console.Error.WriteLine();
        return i;
    }

    public static bool HashStartsWithFiveZeros(string secretKey, uint n)
    {
        byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes($"{secretKey}{n}"));
        return hash[0] == 0 && hash[1] == 0 && hash[2] <= 15;
    }
    
    public static bool HashStartsWithSixZeros(string secretKey, uint n)
    {
        byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes($"{secretKey}{n}"));
        return hash[0] == 0 && hash[1] == 0 && hash[2] == 0;
    }
}