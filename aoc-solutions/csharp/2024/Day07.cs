using Common;

namespace _2024;

public static class Day07
{
    public static string Part1(IEnumerable<string> input)
    {
        return Execute(input, [Operation.Add, Operation.Multiply]).ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        return Execute(input, [Operation.Add, Operation.Multiply, Operation.Concatenate]).ToString();
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private const string Sample = """
                                  190: 10 19
                                  3267: 81 40 27
                                  83: 17 5
                                  156: 15 6
                                  7290: 6 8 6 15
                                  161011: 16 10 13
                                  192: 17 8 14
                                  21037: 9 7 18 13
                                  292: 11 6 16 20
                                  """;
    
    private static ulong Execute(IEnumerable<string> input, Operation[] operations)
    {
        ulong sumOfValidEquations = 0;
        foreach (string line in input)
        {
            string[] goalAndOperands = line.Split(": ");
            ulong goal = ulong.Parse(goalAndOperands[0]);
            IEnumerable<ulong> operands = goalAndOperands[1].Split(" ").Select(ulong.Parse);
            if (CanBeTrue(goal, new Queue<ulong>(operands), operations))
            {
                sumOfValidEquations += goal;
            }
        }

        return sumOfValidEquations;
    }
    
    private static bool CanBeTrue(ulong goal, Queue<ulong> operands, Operation[] operations)
    {
        Node? node = new(operands.Dequeue(), goal);
        PriorityQueue<Node, ulong> priorityQueue = new();
        List<Node> newNodes = [];
        priorityQueue.Enqueue(node, node.DiffToGoal);
        while (operands.TryDequeue(out ulong nextOperand))
        {
            while (priorityQueue.TryDequeue(out node, out _))
            {
                foreach (Operation operation in operations)
                {
                    Node newNode = new(node, nextOperand, operation);
                    if (operands.Count == 0 && newNode.HasReachedGoal)
                        return true;
                    if (newNode.CanReachGoal)
                        newNodes.Add(newNode);
                }
            }

            if (newNodes.Count == 0)
                break;
            
            foreach (Node newNode in newNodes) 
                priorityQueue.Enqueue(newNode, newNode.DiffToGoal);
            newNodes.Clear();
        }

        return false;
    }
    
    private enum Operation
    {
        None,
        Add,
        Multiply,
        Concatenate,
    }

    private class Node
    {
        public readonly ulong Goal;
        public readonly Node? Previous;
        public bool HasPrevious => Previous is not null;
        public readonly ulong Value;
        public readonly Operation Operation = Operation.None;
        public readonly ulong Result;
        public bool HasReachedGoal => Result == Goal;
        public bool CanReachGoal => Result <= Goal;
        public ulong DiffToGoal => Goal - Result;

        public Node(ulong value, ulong goal)
        {
            Goal = goal;
            Value = value;
            Result = Value;
        }

        public Node(Node previous, ulong value, Operation operation)
        {
            Previous = previous;
            Value = value;
            Goal = previous.Goal;
            Operation = operation;

            Result = operation switch
            {
                Operation.Add => Value + previous.Result,
                Operation.Multiply => Value * previous.Result,
                Operation.Concatenate => ulong.Parse($"{previous.Result}{Value}"),
                _ => Result = value
            };
        }

        public override string ToString()
        {
            if (!HasPrevious)
                return Value.ToString();

            string opStr = Operation switch
            {
                Operation.Add => "+",
                Operation.Multiply => "*",
                _ => "||"
            };
            return $"{Previous} {opStr} {Value}";
        }
    }
}