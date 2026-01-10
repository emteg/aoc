using Common;

namespace _2024;

public static class Day03
{
    public static string Part1(IEnumerable<string> input)
    {
        long result = 0;
        
        foreach (string line in input)
        {
            ReadOnlySpan<char> span = line.AsSpan();
            const string beginOfMul = "mul(";
            while (span.IndexOf(beginOfMul) >= 0)
            {
                span = span[(span.IndexOf(beginOfMul) + 4)..];
                
                int indexOfClosingBraces = span.IndexOf(")");
                if (indexOfClosingBraces < 0)
                    continue;
                
                ReadOnlySpan<char> betweenBraces = span[..indexOfClosingBraces];
                if (betweenBraces.IndexOf(beginOfMul) >= 0)
                    continue;
                
                int indexOfComma = betweenBraces.IndexOf(",");
                if (indexOfComma < 0)
                    continue;
                
                ReadOnlySpan<char> rawA = betweenBraces[..indexOfComma];
                ReadOnlySpan<char> rawB = betweenBraces[(indexOfComma + 1)..];
                
                if (long.TryParse(rawA, out long a) && long.TryParse(rawB, out long b))
                    result += a * b;
                
                span = span[(indexOfClosingBraces + 1)..];
            }
        }

        return result.ToString();
    }

    public static string Part1Sample() => Part1("xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))".Lines());

    public static string Part2(IEnumerable<string> input)
    {
        long result = 0;
        const string beginOfMul = "mul(";
        const string doInstr = "do()";
        const string dontInstr = "don't()";
        State state = State.SearchMulOrDontToken;
        
        foreach (string line in input)
        {
            ReadOnlySpan<char> span = line.AsSpan();

            while (true)
            {
                int indexOfMul = span.IndexOf(beginOfMul);
                int indexOfDont = span.IndexOf(dontInstr);
                int indexOfDo = span.IndexOf(doInstr);
                
                if (state is State.SearchMulOrDontToken && 
                    indexOfMul < 0 && indexOfDont < 0) // nothing interesting left anymore
                    break;
                
                if (state is State.SearchMulOrDontToken && 
                    (indexOfMul < indexOfDont || (indexOfMul >= 0 && indexOfDont < 0))) // start of mul before dont statement, or only start of mul
                {
                    span = span[(indexOfMul + 4)..];
                    
                    int indexOfClosingBraces = span.IndexOf(")");
                    if (indexOfClosingBraces < 0)
                        continue;
                    
                    ReadOnlySpan<char> betweenBraces = span[..indexOfClosingBraces];
                    
                    int indexOfComma = betweenBraces.IndexOf(",");
                    if (indexOfComma < 0)
                        continue;
                    
                    ReadOnlySpan<char> rawA = betweenBraces[..indexOfComma];
                    ReadOnlySpan<char> rawB = betweenBraces[(indexOfComma + 1)..];

                    if (long.TryParse(rawA, out long a) && long.TryParse(rawB, out long b))
                    {
                        result += a * b;
                        span = span[(indexOfClosingBraces + 1)..];
                    }
                }
                else if (state is State.SearchMulOrDontToken && 
                         (indexOfDont < indexOfMul || indexOfMul < 0 && indexOfDont >= 0)) // dont statement before start of mul, or only dont statement left
                {
                    state = State.SearchDoToken;
                    span = span[(indexOfDont + 7)..];
                }
                else if (state is State.SearchDoToken && indexOfDo >= 0) // do statement
                {
                    state = State.SearchMulOrDontToken;
                    span = span[(indexOfDo + 4)..];
                }
                else if (state is State.SearchDoToken && indexOfDo < 0) // multiplication disabled and no do left anymore
                    break;
            }
        }

        return result.ToString();
    }

    private enum State
    {
        SearchMulOrDontToken,
        SearchDoToken
    }
    
    public static string Part2Sample() => Part2("xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))".Lines());
}