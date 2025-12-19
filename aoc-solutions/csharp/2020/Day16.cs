using Common;

namespace _2020;

public static class Day16
{
    public static string Part1(IEnumerable<string> input)
    {
        ReadOnlySpan<string> span = new(input.ToArray());
        List<NamedRange> ranges = ParseRanges(span[..span.IndexOf("")]);
        int sumOfInvalidValues = 0;
        foreach (string nearbyTicket in span[(span.IndexOf("nearby tickets:") +1)..]) 
            sumOfInvalidValues += nearbyTicket.Split(',').Select(int.Parse).Where(it => !ValueContainedInAnyRange(it, ranges)).Sum();
        return sumOfInvalidValues.ToString();
    }

    public static string Part1Sample() => Part1(Sample1.Lines());

    public static string Part2(IEnumerable<string> input)
    {
        ReadOnlySpan<string> span = new(input.ToArray());
        List<NamedRange> ranges = ParseRanges(span[..span.IndexOf("")]);
        ulong[] myTicketValues = span[(span.IndexOf("your ticket:") + 1)..][0].Split(',').Select(ulong.Parse).ToArray();
        List<int[]> validTickets = ParseValidTickets(span, ranges);
        List<List<int>> columnValues = TransposeTicketsAsColumns(validTickets);

        Dictionary<int, List<NamedRange>> columnRangeCandidates = [];
        List<NamedRange> determinedColumns = [];
        
        FindColumnCandidates(columnValues, columnRangeCandidates, ranges, determinedColumns);
        RemoveDuplicateCandidates(columnRangeCandidates, determinedColumns);

        ulong result = 1;
        for (int index = 0; index < columnRangeCandidates.Count; index++)
        {
            if (!columnRangeCandidates[index][0].Name.StartsWith("departure"))
                continue;
            
            result *= myTicketValues[index];
        }
        
        return result.ToString();
    }

    private static void RemoveDuplicateCandidates(Dictionary<int, List<NamedRange>> columnRangeCandidates, List<NamedRange> determinedColumns)
    {
        bool anythingRemoved;
        do
        {
            anythingRemoved = false;
            foreach (KeyValuePair<int, List<NamedRange>> pair in columnRangeCandidates.Where(pair => pair.Value.Count > 1))
            {
                for (int i = pair.Value.Count - 1; i >= 0; i--)
                {
                    if (determinedColumns.Contains(pair.Value[i]))
                    {
                        pair.Value.RemoveAt(i);
                        anythingRemoved = true;
                    }
                }

                if (pair.Value.Count == 1) 
                    determinedColumns.Add(pair.Value[0]);
            }
        } while (anythingRemoved);
    }

    private static void FindColumnCandidates(List<List<int>> columnValues, Dictionary<int, List<NamedRange>> columnRangeCandidates, List<NamedRange> ranges,
        List<NamedRange> determinedColumns)
    {
        for (int colIndex = 0; colIndex < columnValues.Count; colIndex++)
        {
            columnRangeCandidates.Add(colIndex, ranges.Where(range => range.ContainsAll(columnValues[colIndex])).ToList());
            if (columnRangeCandidates[colIndex].Count == 1)
            {
                NamedRange namedRange = columnRangeCandidates[colIndex][0];
                ranges.Remove(namedRange);
                determinedColumns.Add(namedRange);
            }
        }
    }

    private static List<List<int>> TransposeTicketsAsColumns(List<int[]> validTickets)
    {
        List<List<int>> columnValues = [];
        for (int rowIndex = 0; rowIndex < validTickets.Count; rowIndex++)
        {
            for (int colIndex = 0; colIndex < validTickets[rowIndex].Length; colIndex++)
            {
                if (rowIndex == 0) // establish columns
                    columnValues.Add([]);
                
                columnValues[colIndex].Add(validTickets[rowIndex][colIndex]);
            }
        }

        return columnValues;
    }

    private static List<int[]> ParseValidTickets(ReadOnlySpan<string> span, List<NamedRange> ranges)
    {
        List<int[]> validTickets = [];
        foreach (string nearbyTicket in span[(span.IndexOf("nearby tickets:") +1)..])
        {
            validTickets.Add(nearbyTicket
                .Split(',')
                .Select(int.Parse)
                .Where(it => ValueContainedInAnyRange(it, ranges))
                .ToArray());
        }
        return validTickets;
    }

    public static string Part2Sample() => Part2(Sample2.Lines());

    private static bool ValueContainedInAnyRange(int value, IEnumerable<NamedRange> ranges)
    {
        return ranges.Any(range => range.ContainsValue(value));
    }

    private static List<NamedRange> ParseRanges(ReadOnlySpan<string> classes)
    {
        List<NamedRange> result = [];
        foreach (string cls in classes) 
            result.Add(NamedRange.Parse(cls));
        return result;
    }

    private readonly struct NamedRange : IEquatable<NamedRange>
    {
        public readonly string Name;
        public readonly Range[] Ranges;

        public static NamedRange Parse(string line)
        {
            string name = line[..line.IndexOf(':')];
            List<Range> ranges = [];
            
            string s = line[(line.IndexOf(": ", StringComparison.Ordinal) + 2)..];
            string[] rangeValues = s.Split(" or ");
            ranges.AddRange(rangeValues
                .Select(range => range
                    .Split('-')
                    .Select(int.Parse)
                    .ToArray())
                .Select(numbers => new Range(numbers[0], numbers[1])));

            return new NamedRange(name, ranges.ToArray());
        }

        public bool ContainsValue(int value)
        {
            return Ranges.Any(range => value >= range.Start.Value && value <= range.End.Value);
        }

        public bool ContainsAll(List<int> values)
        {
            Range[] ranges = Ranges;
            bool result = values.All(value => ranges.Any(range => value >= range.Start.Value && value <= range.End.Value));
            return result;
        }

        public override string ToString()
        {
            return $"{Name}: {string.Join(" or ", Ranges.Select(it => $"{it.Start.Value}-{it.End.Value}"))}";
        }

        private NamedRange(string name, Range[] ranges)
        {
            Name = name;
            Ranges = ranges;
        }

        public bool Equals(NamedRange other)
        {
            return Name == other.Name && Ranges.Equals(other.Ranges);
        }

        public override bool Equals(object? obj)
        {
            return obj is NamedRange other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Ranges);
        }
    }
    
    private const string Sample1 = """
                                  class: 1-3 or 5-7
                                  row: 6-11 or 33-44
                                  seat: 13-40 or 45-50

                                  your ticket:
                                  7,1,14

                                  nearby tickets:
                                  7,3,47
                                  40,4,50
                                  55,2,20
                                  38,6,12
                                  """;
    
    private const string Sample2 = """
                                   class: 0-1 or 4-19
                                   row: 0-5 or 8-19
                                   seat: 0-13 or 16-19
                                   
                                   your ticket:
                                   11,12,13
                                   
                                   nearby tickets:
                                   3,9,18
                                   15,1,5
                                   5,14,9
                                   """;
}