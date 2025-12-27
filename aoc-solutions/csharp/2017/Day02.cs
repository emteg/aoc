using Common;

namespace _2017;

public static class Day02
{
    public static string Part1(IEnumerable<string> input)
    {
        long checksum = 0;
        foreach (string line in input)
        {
            long[] values = line
                .Split('\t')
                .Select(long.Parse)
                .ToArray();
            long min = values.Min();
            long max = values.Max();
            checksum += max - min;
        }

        return checksum.ToString();
    }

    public static string Part1Sample() => Part1(Sample1.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        long sum = 0;
        foreach (string line in input)
        {
            long[] values = line
                .Split('\t')
                .Select(long.Parse)
                .ToArray();
            bool lineFinished = false;
            for (int i = 0; i < values.Length; i++)
            {
                long v1 = values[i];
                for (int j = 0; j < values.Length; j++)
                {
                    if (i == j)
                        continue;
                    
                    long v2 = values[j];
                    if (v1 % v2 != 0) 
                        continue;
                    
                    sum += v1 / v2;
                    lineFinished = true;
                    break;
                }

                if (lineFinished)
                    break;
            }
        }

        return sum.ToString();
    }
    
    public static string Part2Sample() => Part2(Sample2.Lines());

    private const string Sample1 = "5\t1\t9\t5\n" +
                                  "7\t5\t3\n" +
                                  "2\t4\t6\t8";
    
    private const string Sample2 = "5\t9\t2\t8\n" +
                                   "9\t4\t7\t3\n" +
                                   "3\t8\t6\t5";
}