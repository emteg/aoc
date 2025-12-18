using System.Globalization;
using Common;

namespace _2020;

public static class Day14
{
    public static string Part1(IEnumerable<string> input)
    {
        Dictionary<ulong, ulong> memory = [];

        BitMask mask = new();
        foreach (string line in input)
        {
            if (line.StartsWith("mask = "))
            {
                mask = BitMask.Parse(line[7..]);
                continue;
            }

            ulong address = ulong.Parse(line[4..line.IndexOf(']')]);
            ulong unmaskedValue = ulong.Parse(line[(line.IndexOf('=') + 1)..]);
            
            memory[address] = mask.Apply(unmaskedValue);
        }

        ulong result = memory.Values.Where(value => value > 0).Sum();
        
        return result.ToString();
    }

    public static string Part1Sample() => Part1(Sample1.Lines());
    
    private readonly struct BitMask
    {
        private readonly ulong zerosMask; // all bits are 1, except those that should be set to 0 when applying
        private readonly ulong onesMask;  // all bits are 0, except those that should be set to 1 when applying

        public static BitMask Parse(string mask)
        {
            string zerosMaskStr = new(mask.Select(c => c is 'X' or '1' ? '1' : '0').ToArray());
            string onesMaskStr  = new(mask.Select(c => c is 'X' or '0' ? '0' : '1').ToArray());

            return new BitMask(
                ulong.Parse(zerosMaskStr, NumberStyles.BinaryNumber), 
                ulong.Parse(onesMaskStr, NumberStyles.BinaryNumber));
        }

        public ulong Apply(ulong value)
        {
            value &= zerosMask; // bitwise AND of value with ZerosMask enforces 0s where ZerosMask is 0
            value |= onesMask;  // bitwise OR of value with OnesMask enforces 1s where OnesMask is 1
            return value;
        }

        private BitMask(ulong zerosMask, ulong onesMask)
        {
            this.zerosMask = zerosMask;
            this.onesMask = onesMask;
        }
    }
    
    public static string Part2(IEnumerable<string> input)
    {
        Dictionary<ulong, ulong> memory = [];
        string mask = string.Empty;
        
        foreach (string line in input)
        {
            if (line.StartsWith("mask = "))
            {
                mask = line[7..];
                continue;
            }

            string originalAddress = line[4..line.IndexOf(']')];
            ulong value = ulong.Parse(line[(line.IndexOf('=') + 1)..]);
            
            foreach (ulong address in DecodeAddresses(originalAddress, mask)) 
                memory[address] = value;
        }
        
        ulong result = memory.Values.Where(value => value > 0).Sum();
        
        return result.ToString();
    }

    public static string Part2Sample() => Part2(Sample2.Lines());

    private static IEnumerable<ulong> DecodeAddresses(string originalAddress, string mask)
    {
        originalAddress = Convert.ToString(long.Parse(originalAddress), 2);
        int leadingZeros = 36 - originalAddress.Length;
        originalAddress = new string('0', leadingZeros) + originalAddress;
        
        char[] floatingAddressChars = new char[mask.Length];
        for (int i = 0; i < originalAddress.Length; i++)
        {
            char c = originalAddress[i];
            floatingAddressChars[i] = mask[i] is '0' ? c : mask[i] is '1' ? '1' : 'X';
        }
        
        foreach (string floatingAddress in EnumerateFloatingAddresses(floatingAddressChars))
            yield return ulong.Parse(floatingAddress, NumberStyles.BinaryNumber);
    }

    private static IEnumerable<string> EnumerateFloatingAddresses(char[] floatingAddress)
    {
        int index = floatingAddress.IndexOf('X');
        
        if (index < 0)
        {
            string s = new(floatingAddress);
            yield return s;
            yield break;
        }

        char[] modified = new char[floatingAddress.Length];
        floatingAddress.CopyTo(modified);
        
        modified[index] = '0';
        foreach (string addr in EnumerateFloatingAddresses(modified))
            yield return addr;
        
        modified[index] = '1';
        foreach (string addr in EnumerateFloatingAddresses(modified))
            yield return addr;
    }
    
    private const string Sample1 = """
                                  mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
                                  mem[8] = 11
                                  mem[7] = 101
                                  mem[8] = 0
                                  """;
    private const string Sample2 = """
                           mask = 000000000000000000000000000000X1001X
                           mem[42] = 100
                           mask = 00000000000000000000000000000000X0XX
                           mem[26] = 1
                           """;
}