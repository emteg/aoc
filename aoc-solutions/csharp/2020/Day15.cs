using Common;

namespace _2020;

public static class Day15
{
    public static string Part1(IEnumerable<string> input)
    {
        Game game = new(input.First().Split(',').Select(int.Parse));
        
        while (game.Turn < 2020)
            game.SpeakNext();
        
        return game.LastNumber.ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        Game game = new(input.First().Split(',').Select(int.Parse));
        
        while (game.Turn < 30_000_000)
            game.SpeakNext();
        
        return game.LastNumber.ToString();
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());

    private sealed class Game
    {
        public int Turn { get; private set; }
        public int LastNumber { get; private set; }
        public bool LastNumberWasNew { get; private set; } = true;

        public Game(IEnumerable<int> startingNumbers)
        {
            foreach (int startingNumber in startingNumbers)
            {
                Turn++;
                Queue<int> queue = new(2);
                queue.Enqueue(Turn);
                spokenNumbers.Add(startingNumber, queue);
                LastNumber = startingNumber;
            }
        }

        public void SpeakNext()
        {
            Turn++;
            
            if (LastNumberWasNew)
            {
                LastNumberWasNew = false;
                NumberWasSaid(0);
                return;
            }
            
            int firstTurn = spokenNumbers[LastNumber].Dequeue();
            int secondTurn = spokenNumbers[LastNumber].Peek();
            NumberWasSaid(secondTurn - firstTurn);
        }

        private void NumberWasSaid(int newNumber)
        {
            LastNumber = newNumber;

            if (spokenNumbers.TryGetValue(newNumber, out Queue<int>? queue))
            {
                if (queue.Count >= 2)
                    _ = queue.Dequeue();
                queue.Enqueue(Turn);
                return;
            }

            LastNumberWasNew = true;
            queue = new Queue<int>(2);
            queue.Enqueue(Turn);
            spokenNumbers.Add(newNumber, queue);
        }

        private readonly Dictionary<int, Queue<int>> spokenNumbers = [];
    }
    
    private const string Sample = """
                                  0,3,6
                                  """;
}