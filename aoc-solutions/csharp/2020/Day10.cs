using Common;

namespace _2020;

public static class Day10
{
    public static string Part1(IEnumerable<string> input)
    {
        List<int> values = input.Select(int.Parse).ToList();
        values.Add(0);
        int max = values.Max();
        values.Add(max + 3);
        values.Sort();

        int differencesOfOne = 0;
        int differencesOfThree = 0;
        
        for (int i = 0; i < values.Count - 1; i++)
        {
            int difference = values[i + 1] - values[i];
            
            if (difference == 1)
                differencesOfOne++;
            
            if (difference == 3)
                differencesOfThree++;
        }

        int result = differencesOfOne * differencesOfThree;
        return result.ToString();
    }

    public static string Part1Sample()
    {
        string sample1Result = Part1(Sample.Lines());
        Console.Error.WriteLine(sample1Result);
        string sample2Result = Part1(Sample2.Lines());
        return sample2Result;
    }

    public static string Part2(IEnumerable<string> input)
    {
        List<int> values = input.Select(int.Parse).ToList();
        values.Add(0);
        int max = values.Max();
        values.Add(max + 3);
        values.Sort();

        Dictionary<int, AdapterNode> graph = BuildGraph(values);
        
        foreach (AdapterNode node in graph.Values)
        {
            if (node.IncomingNodes.Count == 0)
            {
                node.Paths = 1;
                continue;
            }

            foreach (AdapterNode incoming in node.IncomingNodes)
            {
                node.Paths += incoming.Paths;
            }
        }

        AdapterNode device = graph[graph.Keys.Last()];
        return device.Paths.ToString();
    }

    private static Dictionary<int, AdapterNode> BuildGraph(List<int> values)
    {
        Dictionary<int, AdapterNode> adapters = [];

        foreach (int value in values)
        {
            if (!adapters.TryGetValue(value, out AdapterNode? node))
            {
                node = new AdapterNode(value);
                adapters.Add(value, node);
            }

            foreach (int previousValue in adapters.Keys.Where(it => it < value).Where(it => it >= value - 3))
            {
                AdapterNode previousNode = adapters[previousValue];
                previousNode.ConnectFrom(node);
            }
        }

        return adapters;
    }

    public static string Part2Sample()
    {
        string sample1Result = Part2(Sample.Lines());
        Console.Error.WriteLine(sample1Result);
        string sample2Result = Part2(Sample2.Lines());
        return sample2Result;
    }

    private sealed class AdapterNode
    {
        public readonly int Value;
        public readonly List<AdapterNode> IncomingNodes = [];
        public ulong Paths;
        
        public AdapterNode(int value)
        {
            Value = value;
        }

        public void ConnectFrom(AdapterNode outgoing)
        {
            outgoing.IncomingNodes.Add(this);
        }
        
        public override string ToString() => Value.ToString();
    }
    
    private const string Sample = """
                                  16
                                  10
                                  15
                                  5
                                  1
                                  11
                                  7
                                  19
                                  6
                                  12
                                  4
                                  """;
    
    private const string Sample2 = """
                                   28
                                   33
                                   18
                                   42
                                   31
                                   14
                                   46
                                   20
                                   48
                                   47
                                   24
                                   23
                                   49
                                   45
                                   19
                                   38
                                   39
                                   11
                                   1
                                   32
                                   25
                                   35
                                   8
                                   17
                                   7
                                   9
                                   4
                                   2
                                   34
                                   10
                                   3
                                   """;
}