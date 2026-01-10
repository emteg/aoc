using System.Diagnostics;
using Common;

namespace _2024;

public static class Day06
{
    public static string Part1(IEnumerable<string> input)
    {
        (List<List<GuardedCell>> grid, Guard guard) = ParseInput(input);
        while (guard.NextCell() is not null) 
            guard.Move();
        return grid.SelectMany(row => row).Count(cell => cell.HasBeenVisited).ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        (List<List<GuardedCell>> grid, Guard guard) = ParseInput(input);
        int movesToMakeBeforePlacingObstacle = 0;
        HashSet<(int x, int y)> uniqueObstacleLocations = [];
        
        while (true)
        {
            // reset grid and guard
            foreach (GuardedCell guardedCell in grid.SelectMany(row => row)) 
                guardedCell.Reset();
            guard.Reset();
            
            // try to make the requested number of moves
            bool guardLeftMapBeforeRequestedMovesMade = false;
            for (int i = 0; i < movesToMakeBeforePlacingObstacle; i++)
            {
                if (guard.NextCell() is null)
                {
                    guardLeftMapBeforeRequestedMovesMade = true;
                    break;
                }
                    
                guard.Move();
            }
            
            // if the guard left the map before the requested number of moves, or
            // if after that number of moves the guard ends up right at the edge...
            if (guard.NextCell() is null || guardLeftMapBeforeRequestedMovesMade)
            {
                break; // we are done.
            }
            
            // we now place an obstacle right in front of the guard
            GuardedCell nextCell = guard.NextCell()!;
            if (nextCell.HasBeenVisited) // unless that cell has already been visited by the guard in his walk so far
            {
                movesToMakeBeforePlacingObstacle++;
                continue;
            }
            nextCell.PlaceObstacle();
            guard.TurnUntilPathIsClear(); // we let the guard turn right (usually done when guard moves)

            // we let the guard walk until it either leaves the map, or an infinite loop is detected
            if (!GuardLeavesMap(guard, grid)) 
                uniqueObstacleLocations.Add((nextCell.X, nextCell.Y)); // we found an infinite loop
            
            movesToMakeBeforePlacingObstacle++; // we now try again, but with one more move before placing an obstacle
        }
        
        return uniqueObstacleLocations.Count.ToString();
    }

    private static bool GuardLeavesMap(Guard guard, List<List<GuardedCell>> grid)
    {
        while (guard.NextCell() is not null)
        {
            if (guard.Move()) // true: infinite loop detected
            {
                return false;
            }
        }

        return true;
    }

    private static void WriteGrid(List<List<GuardedCell>> grid, GuardedCell? highlight = null)
    {
        ConsoleColor normalColor = Console.ForegroundColor;
        foreach (List<GuardedCell> row in grid)
        {
            foreach (GuardedCell guardedCell in row)
            {
                if (guardedCell == highlight) 
                    Console.ForegroundColor = ConsoleColor.Red;
                else if (guardedCell.HasGuard)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else if (guardedCell.HasBeenVisited)
                    Console.ForegroundColor = ConsoleColor.Green;
                
                Console.Error.Write(guardedCell);
                
                if (guardedCell == highlight || guardedCell.HasBeenVisited || guardedCell.HasGuard)
                    Console.ForegroundColor = normalColor;
            }
            Console.Error.WriteLine();
        }
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private const string Sample = """
                                  ....#.....
                                  .........#
                                  ..........
                                  ..#.......
                                  .......#..
                                  ..........
                                  .#..^.....
                                  ........#.
                                  #.........
                                  ......#...
                                  """;

    private static (List<List<GuardedCell>> grid, Guard guard) ParseInput(IEnumerable<string> input)
    {
        List<List<GuardedCell>> grid = [];
        int y = 0;
        Guard guard = null!;
        
        foreach (string line in input)
        {
            int x = 0;
            grid.Add([]);
            foreach (char c in line)
            {
                GuardedCell cell = new(c, x, y);
                grid[y].Add(cell);
                Cell2D.ConnectFour(cell, grid);
                x++;
                
                if (c == '^')
                    guard = new Guard(cell);
            }
            y++;
        }
        
        return (grid, guard);
    }

    private static string DirectionToChar(Direction? direction)
    {
        return direction switch
        {
            Direction.Up => "^",
            Direction.Right => ">",
            Direction.Down => "v",
            Direction.Left => "<",
            _ => "",
        };
    }
    
    [DebuggerDisplay("{ToString()} Y: {Y} | X: {X} ({DirectionToChar(LastVisitDirection)})")]
    private sealed class GuardedCell : Cell2D
    {
        public bool HasObstacle { get; private set; }
        public bool HasBeenVisited { get; private set; }
        public bool HasGuard => Guard is not null;
        public Guard? Guard { get; private set; }
        public HashSet<Direction> LastVisitedDirections = [];
        public bool ObstaclePlaced { get; private set; }
        
        public GuardedCell(char c, int x, int y) : base(x, y)
        {
            HasObstacle = c == '#';
        }
        
        public void Visit(Guard guard)
        {
            HasBeenVisited = true;
            LastVisitedDirections.Add(guard.Direction);
            Guard = guard;
        }

        public void Leave()
        {
            Guard = null;
        }

        public void Reset()
        {
            HasBeenVisited = false;
            LastVisitedDirections = [];
            Guard = null;
            if (HasObstacle && ObstaclePlaced)
                HasObstacle = false;
            ObstaclePlaced = false;
        }

        public void PlaceObstacle()
        {
            HasObstacle = true;
            ObstaclePlaced = true;
        }

        public override string ToString()
        {
            return HasGuard
                ? Guard!.ToString()
                : HasObstacle
                    ? "#"
                    : HasBeenVisited
                        ? VisitedCellChar()
                        : ".";
        }

        private string VisitedCellChar()
        {
            if (LastVisitedDirections.Count == 1)
                return DirectionToChar(LastVisitedDirections.First());
            
            bool horizontal = LastVisitedDirections.Contains(Direction.Right) ||
                              LastVisitedDirections.Contains(Direction.Left);
            bool vertical = LastVisitedDirections.Contains(Direction.Up) ||
                            LastVisitedDirections.Contains(Direction.Down);

            if (horizontal && !vertical)
                return "-";

            if (vertical && !horizontal)
                return "|";

            return "+";
        }
    }

    [DebuggerDisplay("{ToString()} Y: {Location.Y} | X: {Location.X}")]
    private sealed class Guard
    {
        public Direction Direction { get; private set; } = Direction.Up;
        public GuardedCell Location { get; private set; }

        public Guard(GuardedCell location)
        {
            Location = location;
            location.Visit(this);
            originalLocation = location;
        }

        public void Reset()
        {
            Location = originalLocation;
            Direction = Direction.Up;
            Location.Visit(this);
        }

        public bool Move()
        {
            GuardedCell? next = NextCell();
            
            if (next is null)
                return false;

            if (next.HasBeenVisited && next.LastVisitedDirections.Contains(Direction))
                return true; // infinite loop detected!
            
            Location.Leave();
            Location = next;
            Location.Visit(this);

            TurnUntilPathIsClear();

            return false;
        }

        public void TurnUntilPathIsClear()
        {
            while (NextCell()?.HasObstacle is true)
            {
                Direction = DirectionAfterRightTurn();
                Location.LastVisitedDirections.Add(Direction);
            }
        }

        public Direction DirectionAfterRightTurn()
        {
            return Direction switch
            {
                Direction.Up => Direction.Right,
                Direction.Right => Direction.Down,
                Direction.Down => Direction.Left,
                Direction.Left => Direction.Up,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override string ToString() => DirectionToChar(Direction);

        public GuardedCell? NextCell()
        {
            return Direction switch
            {
                Direction.Up => (GuardedCell?)Location.Up,
                Direction.Right => (GuardedCell?)Location.Right,
                Direction.Down => (GuardedCell?)Location.Down,
                Direction.Left => (GuardedCell?)Location.Left,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private readonly GuardedCell originalLocation;
    }
}