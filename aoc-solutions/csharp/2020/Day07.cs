using Common;

namespace _2020;

public static class Day07
{
    public static string Part1(IEnumerable<string> input)
    {
        Dictionary<string, Bag> allBags = ParseBags(input);
        Bag shinyGoldBag = allBags["shiny gold"];

        int bagsThatEventuallyContainAShinyGoldBag = 0;
        foreach ((_, Bag bag) in allBags.Where(valuePair => valuePair.Value != shinyGoldBag))
        {
            if (bag.ContainedBags.Count == 0)
                continue;
            
            Queue<Bag> toExplore = new(bag.ContainedBags.Keys);

            while (toExplore.Count > 0)
            {
                Bag current = toExplore.Dequeue();
                
                if (current == shinyGoldBag)
                {
                    bagsThatEventuallyContainAShinyGoldBag++;
                    break;
                }
                
                foreach (Bag containedBag in current.ContainedBags.Keys) 
                    toExplore.Enqueue(containedBag);
            }
        }
        
        return bagsThatEventuallyContainAShinyGoldBag.ToString();
    }

    public static string Part1Sample() => Part1(Sample1.Lines());

    public static string Part2(IEnumerable<string> input)
    {
        Dictionary<string, Bag> allBags = ParseBags(input);
        int containedBags = 0;
        Queue<(Bag bag, int multiplyer)> toDo = [];

        Bag start = allBags["shiny gold"];
        foreach ((Bag bag, int weight) in start.ContainedBags)
        {
            toDo.Enqueue((bag, weight));
            containedBags += weight;
        }
        
        while (toDo.Count > 0)
        {
            (Bag bag, int multiplyer) = toDo.Dequeue();
            foreach ((Bag containedBag, int weight) in bag.ContainedBags)
            {
                toDo.Enqueue((containedBag, weight * multiplyer));
                containedBags += weight * multiplyer;
            }
        }

        return containedBags.ToString();
    }

    public static string Part2Sample() => Part2(Sample2.Lines());

    private static Dictionary<string, Bag> ParseBags(IEnumerable<string> input)
    {
        Dictionary<string, Bag> allBags = [];

        foreach (string rule in input)
        {
            string ruleMut = rule;
            
            string name = ruleMut[..ruleMut.IndexOf(" bags", StringComparison.Ordinal)];
            if (!allBags.TryGetValue(name, out Bag? bag))
            {
                bag = new Bag(name);
                allBags.Add(name, bag);
            }
            
            if (ruleMut.Contains("no other bags."))
                continue;

            ruleMut = ruleMut[(ruleMut.IndexOf("contain", StringComparison.Ordinal) + 8)..];
            while (ruleMut.Contains(',') || ruleMut.Contains('.'))
            {
                string containedRule;

                if (ruleMut.Contains(','))
                {
                    containedRule = ruleMut[..ruleMut.IndexOf(',')];
                    ruleMut = ruleMut[(ruleMut.IndexOf(',') + 2)..];
                }
                else
                {
                    containedRule = ruleMut;
                    ruleMut = "";
                }
                
                string weightStr = containedRule[..containedRule.IndexOf(' ')];
                int weight = int.Parse(weightStr);
                containedRule = containedRule[(containedRule.IndexOf(' ') +1)..];
                
                name = containedRule[..containedRule.IndexOf(" bag", StringComparison.Ordinal)];

                if (!allBags.TryGetValue(name, out Bag? other))
                {
                    other = new Bag(name);
                    allBags.Add(name, other);
                    bag.Add(other, weight);
                }
                else
                {
                    bag.Add(other, weight);
                }
            }
        }

        return allBags;
    }

    private const string Sample1 = """
                                   light red bags contain 1 bright white bag, 2 muted yellow bags.
                                   dark orange bags contain 3 bright white bags, 4 muted yellow bags.
                                   bright white bags contain 1 shiny gold bag.
                                   muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.
                                   shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
                                   dark olive bags contain 3 faded blue bags, 4 dotted black bags.
                                   vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.
                                   faded blue bags contain no other bags.
                                   dotted black bags contain no other bags.
                                   """;
    
    private const string Sample2 = """
        shiny gold bags contain 2 dark red bags.
        dark red bags contain 2 dark orange bags.
        dark orange bags contain 2 dark yellow bags.
        dark yellow bags contain 2 dark green bags.
        dark green bags contain 2 dark blue bags.
        dark blue bags contain 2 dark violet bags.
        dark violet bags contain no other bags.
        """;

    private sealed class Bag
    {
        public readonly string Name;
        public IReadOnlyDictionary<Bag, int> ContainedBags => containedBags;

        public Bag(string name)
        {
            Name = name;
        }

        public void Add(Bag bag, int weight)
        {
            containedBags[bag] = weight;
            toStringResult = null;
        }

        public override string ToString()
        {
            if (toStringResult is not null)
                return toStringResult;
            
            toStringResult = $"{Name} bags contain";

            if (ContainedBags.Count == 0)
            {
                toStringResult += " no other bags."; 
                return toStringResult;
            }

            string[] otherBagValues = ContainedBags
                .Select(keyValuePair =>
                {
                    (Bag otherBag, int weight) = keyValuePair;
                    return $" {weight} {otherBag.Name} bag{(weight > 1 ? "s" : "")}";
                })
                .ToArray();
            
            toStringResult += string.Join(",", otherBagValues);
            
            return toStringResult;
        }

        private string? toStringResult;
        private readonly Dictionary<Bag, int> containedBags = [];
    }
}