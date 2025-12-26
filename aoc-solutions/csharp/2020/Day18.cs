using Common;

namespace _2020;

public static class Day18
{
    public static string Part1(IEnumerable<string> input)
    {
        long result = 0;

        foreach (string line in input)
        {
            string[] values = $"({line})".Replace("(", "( ").Replace(")", " )").Split(' ');

            Operation? last = null;
            Stack<Braces> openedBraces = [];
            foreach (string value in values)
            {
                if (value is "(")
                {
                    openedBraces.Push(new Braces(last));
                    last = null;
                    continue;
                }

                if (value is ")")
                {
                    Braces brace = openedBraces.Pop();
                    brace.LastInnerOperation = last;
                    brace.Close();
                    last = brace;
                    continue;
                }

                if (int.TryParse(value, out int number))
                {
                    last = new Number(number, last);
                    continue;
                }

                if (value is "+" or "*")
                {
                    last = AddOrMultiply.Create(value, last);
                    continue;
                }
            }
            Number lineResult = (Number)last!.Solve()!;
            result += lineResult.Value;
        }

        return result.ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        long result = 0;

        foreach (string line in input)
        {
            string[] values = $"({line})".Replace("(", "( ").Replace(")", " )").Split(' ');

            Operation? last = null;
            Stack<Braces> openedBraces = [];
            foreach (string value in values)
            {
                if (value is "(")
                {
                    openedBraces.Push(new Braces(last));
                    last = null;
                    continue;
                }

                if (value is ")")
                {
                    Braces brace = openedBraces.Pop();
                    brace.LastInnerOperation = last;
                    brace.Close();
                    last = brace;
                    continue;
                }

                if (int.TryParse(value, out int number))
                {
                    last = new Number(number, last);
                    continue;
                }

                if (value is "+" or "*")
                {
                    last = AddOrMultiply.Create(value, last, true);
                    continue;
                }
            }
            Number lineResult = (Number)last!.Solve()!;
            result += lineResult.Value;
        }

        return result.ToString();
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());

    private abstract class Operation
    {
        public Operation? Left { get; set; }
        public Operation? Right { get; set; }
        public int Priority { get; }
        public virtual bool IsOperator => false;
        public virtual Operation? Solve() => null;
        
        protected Operation(Operation? left, int priority)
        {
            Left = left;
            Left?.Right = this;
            Priority = priority;
        }
    }

    private sealed class Braces : Operation
    {
        public Operation? LastInnerOperation { get; set; }
        public override bool IsOperator => true;

        public Braces(Operation? left) : base(left, 10) { }

        public void Close()
        {
            isClosed = true;
        }

        public override Operation? Solve()
        {
            if (!isClosed)
                return null;

            while (LastInnerOperation?.Left is not null)
            {
                Operation? highestLeftestOperator = FindHighestPriorityLeftmostOperator();
                Operation result = highestLeftestOperator!.Solve()!;
                
                if (highestLeftestOperator is Braces && result is Number)
                {
                    result.Left = highestLeftestOperator.Left;
                    result.Left?.Right = result;
                    highestLeftestOperator.Left = null;
                    
                    result.Right = highestLeftestOperator.Right;
                    result.Right?.Left = result;
                    highestLeftestOperator.Right = null;
                }
                else
                {
                    result.Left = highestLeftestOperator.Left?.Left;
                    result.Left?.Right = result;
                    result.Right = highestLeftestOperator.Right?.Right;
                    result.Right?.Left = result;
                
                    highestLeftestOperator.Left?.Left = null;
                    highestLeftestOperator.Left?.Right = null;
                
                    highestLeftestOperator.Right?.Right = null;
                    highestLeftestOperator.Right?.Left = null;

                    highestLeftestOperator.Left = null;
                    highestLeftestOperator.Right = null;
                }

                if (result.Right is null)
                    LastInnerOperation = result;
            }

            return LastInnerOperation;
        }

        private Operation? FindHighestPriorityLeftmostOperator()
        {
            Operation? highestLeftestOperator = null;
            Operation? last = LastInnerOperation;
            
            while (last is not null)
            {
                if (last.IsOperator && (highestLeftestOperator is null || last.Priority >= highestLeftestOperator.Priority)) 
                    highestLeftestOperator = last;
                last = last.Left;
            }

            return highestLeftestOperator;
        }

        public override string ToString()
        {
            Stack<char> result = [];
            result.Push(')');
            Operation? last = LastInnerOperation;
            while (last is not null)
            {
                string lastStr = last.ToString()!;
                if (last is Braces)
                {
                    foreach (char c in lastStr.Reverse()) 
                        result.Push(c);
                }
                else
                {
                    foreach (char c in lastStr) 
                        result.Push(c);
                }
                last = last.Left;
            }
            result.Push('(');
            string s = new(result.ToArray());
            return s;
        }
        
        private bool isClosed;
    }

    private sealed class AddOrMultiply : Operation
    {
        public override bool IsOperator => true;
        
        public static AddOrMultiply Create(string op, Operation? left, bool additionHigher = false)
        {
            bool isAddition = op == "+";
            int priority = isAddition && additionHigher ? 5 : 3;
            return new AddOrMultiply(isAddition, priority, left);
        }
        
        private AddOrMultiply(bool isAddition, int priority, Operation? left) : base(left, priority)
        {
            this.isAddition = isAddition;
        }
        
        public override Operation? Solve()
        {
            if (Left is not Number a)
                return null;
            
            if (Right is not Number b)
                return null;

            return isAddition
                ? new Number(a.Value + b.Value)
                : new Number(a.Value * b.Value);
        }
        
        public override string ToString() => isAddition ? "+" : "*";
        
        private readonly bool isAddition;
    }
    
    private sealed class Number : Operation
    {
        public long Value { get; }

        public Number(long value, Operation? left = null) : base(left, 0)
        {
            Value = value;
        }

        public override string ToString() => Value.ToString();
    }
    
    private const string Sample = """
                                  1 + 2 * 3 + 4 * 5 + 6
                                  1 + (2 * 3) + (4 * (5 + 6))
                                  2 * 3 + (4 * 5)
                                  5 + (8 * 3 + 9 + 3 * 4 * 3)
                                  5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))
                                  ((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2
                                  """;
}