using Common;

namespace _2024;

public static class Day02
{
    public static string Part1(IEnumerable<string> input)
    {
        return input.Select(Report.FromLine).Count(report => report.IsSafe).ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        Report[] reports = input.Select(Report.FromLine).ToArray();
        int safeReports = reports.Count(report => report.IsSafe);
        Report[] maybeSafeReports = reports.Where(report => !report.IsSafe).ToArray();
        foreach (Report report in maybeSafeReports)
        {
            string original = report.ToString();
            foreach (Level level in report.Levels())
            {
                if (level.Previous is null) // try and remove the first node to see if the report becomes safe
                {
                    report.FirstLevel = level.Next!;
                    
                    if (report.IsSafe)
                    {
                        safeReports++;
                        report.FirstLevel = level;
                        break;
                    }
                    
                    report.FirstLevel = level;
                }
                else if (level.Next is null) // try and remove the last node to see if the report becomes safe
                {
                    Difference? fromPrevious = level.FromPrevious;
                    level.Previous.ToNext = null;
                    
                    if (report.IsSafe)
                    {
                        safeReports++;
                        level.Previous.ToNext = fromPrevious;
                        break;
                    }
                    
                    level.Previous.ToNext = fromPrevious;
                }
                else // try and remove the current 'middle' node to see if the report becomes safe
                {
                    Difference? originalFromPrevious = level.FromPrevious;
                    Difference? originalToNext = level.ToNext;
                    Level? originalPrevious = level.Previous;
                    Level? originalNext = level.Next;
                    level.Previous.ToNext = new Difference(originalNext, originalPrevious) {ExpectedChange = report.IsIncreasing ? 1 : -1};
                    level.Next.FromPrevious = new Difference(originalNext, originalPrevious) {ExpectedChange = report.IsIncreasing ? 1 : -1};

                    if (report.IsSafe)
                    {
                        safeReports++;
                        level.Previous.ToNext = originalFromPrevious;
                        level.Next.FromPrevious = originalToNext;
                        break;
                    }
                    
                    level.Previous.ToNext = originalFromPrevious;
                    level.Next.FromPrevious = originalToNext;
                }
            }
            Console.Error.WriteLine();
        }
        return safeReports.ToString();
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private const string Sample = """
                                  7 6 4 2 1
                                  1 2 7 8 9
                                  9 7 6 2 1
                                  1 3 2 4 5
                                  8 6 4 4 1
                                  1 3 6 7 9
                                  """;

    private sealed class Report
    {
        public bool IsIncreasing { get; }
        
        public Level FirstLevel { get; set; }

        public bool IsSafe
        {
            get
            {
                foreach (Level level in Levels())
                {
                    if (level.ToNext is not null && !level.ToNext.IsSafe)
                        return false;
                }

                return true;
            }
        }
        
        public static Report FromLine(string line)
        {
            int[] values = line.Split(' ').Select(int.Parse).ToArray();
            Level? firstLevel = null;
            Level? previous = null;
            foreach (int value in values)
            {
                Level level = new(value, previous);
                if (firstLevel is null)
                    firstLevel = level;
                previous = level;
            }
            
            return new Report(firstLevel!);
        }

        public override string ToString()
        {
            return Levels().Aggregate(string.Empty, (current, level) => current + level);
        }

        public IEnumerable<Level> Levels()
        {
            Level? level = FirstLevel;
            while (level is not null)
            {
                yield return level;
                level = level.Next;
            }
        }

        private Report(Level firstLevel)
        {
            FirstLevel = firstLevel;
            
            int increasingDifferences = 0;
            int decreasingDifferences = 0;
            foreach (Level level in Levels())
            {
                if (level.ToNext?.Change > 0)
                    increasingDifferences++;
                else if (level.ToNext?.Change < 0)
                    decreasingDifferences++;
            }

            int expectedChange = -1;
            if (increasingDifferences > decreasingDifferences)
            {
                IsIncreasing = true;
                expectedChange = 1;
            }
            
            foreach (Level level in Levels())
            {
                level.ToNext?.ExpectedChange = expectedChange;
            }
        }
    }

    private sealed class Difference
    {
        public int Value { get; }
        public int AbsoluteValue { get; }
        public int Change { get; }
        public Level? Left { get; }
        public Level? Right { get; }
        public int ExpectedChange { get; set; }
        public bool IsSafe => AbsoluteValue is >= 1 and <= 3 && Change != 0 && Change == ExpectedChange;

        public Difference(Level current, Level previous)
        {
            Left = previous;
            Right = current;
            Value = current.Value - previous.Value;
            AbsoluteValue = Math.Abs(Value);
            Change = Value != 0 ? Value / AbsoluteValue : 0;
        }

        public override string ToString()
        {
            char sym = !IsSafe ? '#' : ' ';
            string sign = Change > 0 ? "+" : string.Empty;
            return $" {sym}{sign}{Value}{sym} ";
        }
    }
    
    private sealed class Level
    {
        public int Value { get; }
        public Difference? FromPrevious { get; set; }
        public Difference? ToNext { get; set; }
        public Level? Previous => FromPrevious?.Left;
        public Level? Next => ToNext?.Right;

        public Level(int value, Level? previous = null)
        {
            Value = value;
            if (previous is null)
                return;

            FromPrevious = new Difference(this, previous);
            previous.ToNext = FromPrevious;
        }

        public override string ToString() => $"({Value}){ToNext}";
    }
}