using Common;

namespace _2020;

public static class Day11
{
    public static string Part1(IEnumerable<string> input)
    {
        return Execute(input, new Part1Strategy()).ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());

    public static string Part2(IEnumerable<string> input)
    {
        return Execute(input, new Part2Strategy()).ToString();
    }

    public static string Part2Sample() => Part2(Sample.Lines());

    private static int Execute(IEnumerable<string> input, ICalculateNextState strategy)
    {
        List<Seat> allSeats = BuildGrid(input, strategy).SelectMany(list => list).ToList();
        
        do
        {
            foreach (Seat seat in allSeats) 
                seat.CalculateNextState();
            
            foreach (Seat seat in allSeats) 
                seat.ApplyNextState();
            
        } while (allSeats.Any(seat => seat.HasChanged));

        return allSeats.Count(seat => seat.IsOccupied);
    }

    private static List<List<Seat>> BuildGrid(IEnumerable<string> input, ICalculateNextState strategy)
    {
        List<List<Seat>> grid = [];
        int y = 0;
        foreach (string line in input)
        {
            int x = 0;
            int width = line.Length;
            grid.Add([]);

            foreach (char c in line)
            {
                Seat seat = new(x, y, c, strategy);
                grid[y].Add(seat);
                Cell2D.ConnectEight(seat, grid, width);
                x++;
            }
            y++;
        }

        return grid;
    }

    private static void WriteGridToStandardError(List<List<Seat>> grid)
    {
        foreach (List<Seat> row in grid)
        {
            foreach (Seat seat in row)
            {
                Console.Error.Write(seat.ToChar());
            }
            Console.Error.WriteLine();
        }
        Console.Error.WriteLine();
    }

    private sealed class Seat : Cell2D
    {
        public readonly bool IsSeat;
        public bool IsEmpty => !IsOccupied;
        public bool IsOccupied { get; private set; }
        public bool HasChanged { get; private set; }

        public Seat? UpNeighbor() => (Seat?)this.Up;
        public Seat? UpRightNeighbor() => (Seat?)this.UpRight;
        public Seat? RightNeighbor() => (Seat?)this.Right;
        public Seat? DownRightNeighbor() => (Seat?)this.DownRight;
        public Seat? DownNeighbor() => (Seat?)this.Down;
        public Seat? DownLeftNeighbor() => (Seat?)this.DownLeft;
        public Seat? LeftNeighbor() => (Seat?)this.Left;
        public Seat? UpLeftNeighbor() => (Seat?)this.UpLeft;
        
        public Seat(int x, int y, char c, ICalculateNextState strategy) : base(x, y)
        {
            IsSeat = c == 'L';
            this.strategy = strategy;
        }

        public void CalculateNextState()
        {
            nextIsOccupied = strategy.CalculateNextState(this);
        }

        public void ApplyNextState()
        {
            HasChanged = IsOccupied != nextIsOccupied;
            IsOccupied = nextIsOccupied;
        }

        public char ToChar()
        {
            return (IsSeat, IsOccupied) switch
            {
                (true, true) => '#',
                (true, _) => 'L',
                _ => '.' 
            };
        }

        private bool nextIsOccupied = false;
        private readonly ICalculateNextState strategy;
    }

    private interface ICalculateNextState
    {
        public bool CalculateNextState(Seat seat);
    }

    private sealed class Part1Strategy : ICalculateNextState
    {
        public bool CalculateNextState(Seat seat)
        {
            if (!seat.IsSeat)
                return false;
            
            if (!seat.IsOccupied && seat.Neighbors.Cast<Seat>().All(it => !it.IsOccupied))
                return true;
            
            if (seat.IsOccupied && seat.Neighbors.Cast<Seat>().Count(it => it.IsOccupied) >= 4) 
                return false;
            
            return seat.IsOccupied;
        }
    }

    private sealed class Part2Strategy : ICalculateNextState
    {
        public bool CalculateNextState(Seat seat)
        {
            if (!seat.IsSeat)
                return false;

            Seat[] neighbors = FindClosestNeighborsInEightDirections(seat).ToArray();
            
            if (!seat.IsOccupied && neighbors.All(it => !it.IsOccupied))
                return true;
            
            if (seat.IsOccupied && neighbors.Count(it => it.IsOccupied) >= 5) 
                return false;
            
            return seat.IsOccupied;
        }

        private IEnumerable<Seat> FindClosestNeighborsInEightDirections(Seat seat)
        {
            // up
            Seat? current = seat.UpNeighbor();
            while (current is not null)
            {
                if (current.IsSeat)
                {
                    yield return current;
                    break;
                }

                current = current.UpNeighbor();
            }
            
            // up-right
            current = seat.UpRightNeighbor();
            while (current is not null)
            {
                if (current.IsSeat)
                {
                    yield return current;
                    break;
                }

                current = current.UpRightNeighbor();
            }
            
            // right
            current = seat.RightNeighbor();
            while (current is not null)
            {
                if (current.IsSeat)
                {
                    yield return current;
                    break;
                }

                current = current.RightNeighbor();
            }
            
            // down-right
            current = seat.DownRightNeighbor();
            while (current is not null)
            {
                if (current.IsSeat)
                {
                    yield return current;
                    break;
                }

                current = current.DownRightNeighbor();
            }
            
            // down
            current = seat.DownNeighbor();
            while (current is not null)
            {
                if (current.IsSeat)
                {
                    yield return current;
                    break;
                }

                current = current.DownNeighbor();
            }
            
            // down-left
            current = seat.DownLeftNeighbor();
            while (current is not null)
            {
                if (current.IsSeat)
                {
                    yield return current;
                    break;
                }

                current = current.DownLeftNeighbor();
            }
            
            // left
            current = seat.LeftNeighbor();
            while (current is not null)
            {
                if (current.IsSeat)
                {
                    yield return current;
                    break;
                }

                current = current.LeftNeighbor();
            }
            
            // up-left
            current = seat.UpLeftNeighbor();
            while (current is not null)
            {
                if (current.IsSeat)
                {
                    yield return current;
                    break;
                }

                current = current.UpLeftNeighbor();
            }
        }
    }
    
    private const string Sample = """
        L.LL.LL.LL
        LLLLLLL.LL
        L.L.L..L..
        LLLL.LL.LL
        L.LL.LL.LL
        L.LLLLL.LL
        ..L.L.....
        LLLLLLLLLL
        L.LLLLLL.L
        L.LLLLL.LL
        """;
}