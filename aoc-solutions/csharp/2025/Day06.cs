using Common;

namespace _2025;

public static class Day06
{
    public static string Part1(IEnumerable<string> input)
    {
        return ParseInput(input, true)
            .Select(problem => problem.Solve())
            .Sum()
            .ToString();
    }

    public static string Part2(IEnumerable<string> input)
    {
        return ParseInput(input, false)
            .Select(problem => problem.Solve())
            .Sum()
            .ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private const string Sample = """
        123 328  51 64 
         45 64  387 23 
          6 98  215 314
        *   +   *   +  
        """;
    
    private static IEnumerable<Problem> ParseInput(IEnumerable<string> input, bool horizontalNumbers)
    {
        string[] lines = input.ToArray();
        int columnIndex = 0;
        int maxColumnIndex = lines.Select(line => line.Length).Max() - 1;
        while (columnIndex <= maxColumnIndex)
        {
            char operationSymbol = lines[^1][columnIndex];
            int nextOperationIndex = lines[^1].AsSpan(columnIndex + 1).IndexOfAny(['*', '+']);
            int nextColumnIndex = nextOperationIndex > 0
                ? columnIndex + nextOperationIndex
                : lines[0].Length;
            List<string> column = [];
            for (int i = 0; i < lines.Length - 1; i++)
            {
                string line = lines[i];
                column.Add(line[columnIndex..nextColumnIndex]);
            }

            yield return new Problem(column.ToArray(), operationSymbol, horizontalNumbers);
            columnIndex = nextColumnIndex + 1;
        }
    }
    
    private readonly struct Problem
    {
        private readonly string[] rawLines;
        private readonly char operation;
        private readonly bool useHorizontalNumbers;
    
        public Problem(string[] rawLines, char operation, bool useHorizontalNumbers)
        {
            this.rawLines = rawLines;
            this.operation = operation;
            this.useHorizontalNumbers = useHorizontalNumbers;
        }

        public ulong Solve()
        {
            return useHorizontalNumbers
                ? SolveWithHorizontalNumbers()
                : SolveWithVerticalNumbers();
        }

        private ulong SolveWithHorizontalNumbers()
        {
            return Solve(rawLines
                .Select(line => line.Trim())
                .Select(ulong.Parse)
            );
        }

        private ulong SolveWithVerticalNumbers()
        {
            List<ulong> numbers = [];
            int width = rawLines[0].Length - 1;
            for (int i = width; i >= 0; i--)
            {
                string numberStr = new(rawLines.Select(rawLine => rawLine[i]).ToArray());
                numbers.Add(ulong.Parse(numberStr));
            }

            return Solve(numbers);
        }

        private ulong Solve(IEnumerable<ulong> numbers)
        {
            (Func<ulong, ulong, ulong> operation, ulong result) = InitializeSolver();

            foreach (ulong number in numbers)
                result = operation(result, number);
        
            return result;
        }

        private (Func<ulong, ulong, ulong> operation, ulong initialValue) InitializeSolver()
        {
            return operation == '+'
                ? (Add, 0)
                : (Multiply, 1);
        }
    
        private static ulong Multiply(ulong soFar, ulong next) => soFar * next;
        private static ulong Add(ulong soFar, ulong next) => soFar + next;
    }
}