using Common;

namespace _2020;

public static class Day22
{
    public static string Part1(IEnumerable<string> input)
    {
        Span<string> lines = input.ToArray().AsSpan();

        GameOfCombat game = new(lines[1..lines.IndexOf("")].ToArray().Select(int.Parse),
            lines[(lines.IndexOf("") + 2)..].ToArray().Select(int.Parse));

        while (!game.GameOver) 
            game.PlayRound();
        
        Console.Error.WriteLine("== Post-game results ==");
        Console.Error.WriteLine($"Player 1's deck: {string.Join(", ", game.Player1Deck)}");
        Console.Error.WriteLine($"Player 2's deck: {string.Join(", ", game.Player2Deck)}");
        
        return game.WinningScore.ToString();
    }

    public static string Part1Sample() => Part1(Sample2.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        Span<string> lines = input.ToArray().AsSpan();

        GameOfCombat game = new(lines[1..lines.IndexOf("")].ToArray().Select(int.Parse),
            lines[(lines.IndexOf("") + 2)..].ToArray().Select(int.Parse), true);

        while (!game.GameOver) 
            game.PlayRound();
        
        Console.Error.WriteLine("== Post-game results ==");
        Console.Error.WriteLine($"Player 1's deck: {string.Join(", ", game.Player1Deck)}");
        Console.Error.WriteLine($"Player 2's deck: {string.Join(", ", game.Player2Deck)}");
        
        return game.WinningScore.ToString();
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());

    private const string Sample2 = """
                                   Player 1:
                                   43
                                   19
                                   
                                   Player 2:
                                   2
                                   29
                                   14
                                   """;
    
    private sealed class GameOfCombat
    {
        public int Number { get; }
        public int Round { get; private set; }
        public IEnumerable<int> Player1Deck => player1Deck;
        public IEnumerable<int> Player2Deck => player2Deck;
        public int Player1Card { get; private set; }
        public int Player2Card { get; private set; }
        public GameOfCombat? SubGame { get; private set; }
        public int WinningScore { get; private set; }
        public int WinningPlayer { get; private set; }
        public bool GameOver { get; private set; }

        public GameOfCombat(IEnumerable<int> player1Deck, IEnumerable<int> player2Deck, bool recursively = false)
        {
            Number = gameCounter++;
            this.recursively = recursively;
            this.player1Deck = new Queue<int>(player1Deck);
            this.player2Deck = new Queue<int>(player2Deck);
        }

        public void PlayRound()
        {
            if (GameOver)
                return;
            
            if (recursively)
            {
                int arrayHash1 = string.Join(string.Empty, player1Deck).GetHashCode();
                int arrayHash2 = string.Join(string.Empty, player2Deck).GetHashCode();
                int combined = HashCode.Combine(arrayHash1, arrayHash2);

                if (!deckHashes.Add(combined))
                {
                    WinningScore = CalculateWinningScore(player1Deck.ToArray());
                    GameOver = true;
                    WinningPlayer = 1;
                    return;
                }
            }
            
            Player1Card = player1Deck.Dequeue();
            Player2Card = player2Deck.Dequeue();

            if (recursively && player1Deck.Count >= Player1Card && player2Deck.Count >= Player2Card)
            {
                SubGame = new  GameOfCombat(player1Deck.ToArray().Take(Player1Card), 
                    player2Deck.ToArray().Take(Player2Card), 
                    recursively);
                
                while (!SubGame.GameOver) 
                    SubGame.PlayRound();
                
                if (SubGame.WinningPlayer == 1)
                {
                    player1Deck.Enqueue(Player1Card);
                    player1Deck.Enqueue(Player2Card);
                }
                else
                {
                    player2Deck.Enqueue(Player2Card);
                    player2Deck.Enqueue(Player1Card);
                }

                SubGame = null;
            }
            else
            {
                if (Player1Card > Player2Card)
                {
                    player1Deck.Enqueue(Player1Card);
                    player1Deck.Enqueue(Player2Card);
                }
                else if (Player2Card > Player1Card)
                {
                    player2Deck.Enqueue(Player2Card);
                    player2Deck.Enqueue(Player1Card);
                }
                else
                {
                    Console.Error.WriteLine("Draw!");
                    throw new InvalidOperationException("Draw!");
                }
            }
            
            GameOver = player1Deck.Count == 0 || player2Deck.Count == 0;

            if (!GameOver) 
                return;

            WinningPlayer = player1Deck.Count > 0 ? 1 : 2;
            int[] winningDeck = player1Deck.Count > 0 ? player1Deck.ToArray() : player2Deck.ToArray();
            WinningScore = CalculateWinningScore(winningDeck);
        }

        private static int CalculateWinningScore(int[] winningDeck)
        {
            int weight = winningDeck.Length;
            int score = 0;
            foreach (int card in winningDeck)
            {
                score += card * weight;
                weight--;
            }

            return score;
        }

        private readonly HashSet<int> deckHashes = [];
        private static int gameCounter = 1;
        private readonly bool recursively;
        private readonly Queue<int> player1Deck;
        private readonly Queue<int> player2Deck;
    }
    
    private const string Sample = """
                                  Player 1:
                                  9
                                  2
                                  6
                                  3
                                  1
                                  
                                  Player 2:
                                  5
                                  8
                                  4
                                  7
                                  10
                                  """;
}