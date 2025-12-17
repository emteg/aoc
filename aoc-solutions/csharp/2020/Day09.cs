using Common;

namespace _2020;

public static class Day09
{
    public static string Part1(IEnumerable<string> input) 
        => Part1Execute(input.Select(long.Parse).ToArray(), 25)
            .invalidNumber
            .ToString();

    public static string Part1Sample() 
        => Part1Execute(Sample.Lines().Select(long.Parse).ToArray(), 5)
            .invalidNumber
            .ToString();

    private static (long invalidNumber, int index) Part1Execute(long[] sequence, int preambleSize)
    {
        int currentIndex = preambleSize;
        long currentNumber = 0;
        while (currentIndex < sequence.Length)
        {
            currentNumber = sequence[currentIndex];
            int windowStartIndex = currentIndex - preambleSize;
            int windowEndIndex = windowStartIndex + preambleSize;
            bool isValid = false;

            for (int i = windowStartIndex; i < windowEndIndex - 1; i++)
            {
                for (int j = i + 1; j < windowEndIndex; j++)
                {
                    long previousA = sequence[i];
                    long previousB = sequence[j];
                    long sum = previousA + previousB;
                    isValid = currentNumber == sum;

                    if (isValid) // we can stop if we have found that the currentNumber is valid
                        break;
                }

                if (isValid) // we can stop if we have found that the currentNumber is valid
                    break;
            }

            if (!isValid) // if current number is not sum of any 2 previous $preambleSize numbers
                break;    // we have found an invalid number and stop
            
            currentIndex++;
        }
        
        return (currentNumber, currentIndex);
    }
    
    public static string Part2(IEnumerable<string> input) 
        => Part2Execute(input.Select(long.Parse).ToArray(), 25);
    
    public static string Part2Sample() 
        => Part2Execute(Sample.Lines().Select(long.Parse).ToArray(), 5);

    private static string Part2Execute(long[] sequence, int preambleSize)
    {
        (long invalidNumber, int index) = Part1Execute(sequence, preambleSize);

        long smallestNumber = 0;
        long highestNumber = 0;
        for (int i = index - 1; i >= 0; i--)
        {
            highestNumber = sequence[i];
            smallestNumber = sequence[i];
            
            if (highestNumber > invalidNumber)
                continue;

            long sum = highestNumber;
            int lowerIndex = i - 1;

            while (lowerIndex >= 0 && sum < invalidNumber)
            {
                long currentNumber = sequence[lowerIndex];
                sum += currentNumber;
                
                if (currentNumber > highestNumber)
                    highestNumber = currentNumber;
                if (currentNumber < smallestNumber)
                    smallestNumber = currentNumber;
                
                if (sum >= invalidNumber)
                    break;
                
                lowerIndex--;
            }

            if (sum == invalidNumber)
                break;
        }

        long result = highestNumber + smallestNumber;
        return result.ToString();
    }
    
    private const string Sample = """
        35
        20
        15
        25
        47
        40
        62
        55
        65
        95
        102
        117
        150
        182
        127
        219
        299
        277
        309
        576
        """;
}