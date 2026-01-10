using Common;

namespace _2024;

public static class Day05
{
    public static string Part1(IEnumerable<string> input)
    {
        long result = ParseInput(input)
            .UpdatesInTheRightOrder()
            .Select(it => it[MiddleOf(it)])
            .Sum(it => it);
        
        return result.ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        Update update = ParseInput(input);
        
        long result = update
            .UpdatesNotInTheRightOrder()
            .Select(it => update.Fix(it))
            .Select(it => it[MiddleOf(it)])
            .Sum(it => it);
        
        return result.ToString();
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private const string Sample = """
                                  47|53
                                  97|13
                                  97|61
                                  97|47
                                  75|29
                                  61|13
                                  75|53
                                  29|13
                                  97|29
                                  53|29
                                  61|53
                                  97|53
                                  61|29
                                  47|13
                                  75|47
                                  97|75
                                  47|61
                                  75|61
                                  47|29
                                  75|13
                                  53|13
                                  
                                  75,47,61,53,29
                                  97,61,53,29,13
                                  75,29,13
                                  75,97,47,61,53
                                  61,13,29
                                  97,13,75,29,47
                                  """;
    
    private static int MiddleOf(List<uint> list) => (int)Math.Floor(list.Count / 2.0);

    private static Update ParseInput(IEnumerable<string> input)
    {
        Update update = new();
        Action<string> action = update.ParsePageOrderingRule;
        foreach (string line in input)
        {
            if (line.Length == 0)
            {
                action = update.ParsePagesToProduce;
                continue;
            }
            
            action(line);
        }
        
        return update;
    }
    
    private sealed class Update
    {
        // list of pages that must come after page <key>
        private readonly Dictionary<uint, List<uint>> pageOrderingRules = [];
        private readonly List<List<uint>> pagesToProduce = [];

        public void ParsePageOrderingRule(string line)
        {
            uint[] items = line.Split("|").Select(uint.Parse).ToArray();
            if (pageOrderingRules.ContainsKey(items[0]))
                pageOrderingRules[items[0]].Add(items[1]);
            else
                pageOrderingRules.Add(items[0], [items[1]]);
        }

        public void ParsePagesToProduce(string line)
        {
            uint[] items = line.Split(",").Select(uint.Parse).ToArray();
            pagesToProduce.Add([..items]);
        }

        public IEnumerable<List<uint>> UpdatesInTheRightOrder()
        {
            return pagesToProduce.Where(UpdateIsInRightOrder);
        }

        public IEnumerable<List<uint>> UpdatesNotInTheRightOrder()
        {
            return pagesToProduce.Where(it => !UpdateIsInRightOrder(it));
        }

        private bool UpdateIsInRightOrder(List<uint> pagesToUpdate)
        {
            for (int i = 0; i < pagesToUpdate.Count; i++)
            {
                uint currentPage = pagesToUpdate[i];
                uint? nextPage = i < pagesToUpdate.Count - 1 ? pagesToUpdate[i + 1] : null;

                if (nextPage is not null && PairIsNotInRightOrder(currentPage, nextPage.Value))
                {
                    return false;
                }
            }

            return true;
        }

        public List<uint> Fix(List<uint> pagesToUpdate)
        {
            while (true)
            {
                bool swapped = false;
                for (int i = 0; i < pagesToUpdate.Count; i++)
                {
                    uint currentPage = pagesToUpdate[i];
                    uint? nextPage = i < pagesToUpdate.Count - 1 ? pagesToUpdate[i + 1] : null;
                    if (nextPage is not null && PairIsNotInRightOrder(currentPage, nextPage.Value))
                    {
                        pagesToUpdate[i] = nextPage.Value;
                        pagesToUpdate[i + 1] = currentPage;
                        swapped = true;
                    }
                }

                if (!swapped)
                    break;
            }
            
            return pagesToUpdate;
        }

        private bool PairIsNotInRightOrder(uint currentPage, uint nextPage)
        {
            return pageOrderingRules.TryGetValue(nextPage, out List<uint>? pagesThatMustComeAfter) && pagesThatMustComeAfter.Contains(currentPage);
        }
    }
}