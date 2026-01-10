using System.Diagnostics;
using Common;

namespace _2015;

public static class Day18
{
    public static string Part1(IEnumerable<string> input) => Part1(input, 100).ToString();

    public static string Part1Sample() => Part1(Sample.Lines(), 4).ToString();
    
    private static int Part1(IEnumerable<string> input, int steps, bool printAfterEachStep = false)
    {
        List<List<LightCell2D>> grid = ReadInput(input);
        if (printAfterEachStep)
            PrintGrid(grid, "Initial state:");

        for (int i = 0; i < steps; i++)
        {
            Update(grid);
            if (printAfterEachStep)
                PrintGrid(grid, $"After {i + 1} step(s):");
        }

        int lightsOn = grid.SelectMany(row => row).Count(it => it.IsOn);
        return lightsOn;
    }
    
    public static string Part2(IEnumerable<string> input)
    {
        return Part2(input, 100).ToString();
    }
    
    public static string Part2Sample() => Part2(Sample.Lines(), 4).ToString();
    
    private static int Part2(IEnumerable<string> lines, int steps, bool printAfterEachStep = false)
    {
        List<List<LightCell2D>> grid = ReadInput(lines);
        if (printAfterEachStep)
            PrintGrid(grid, "Initial state:");

        for (int i = 0; i < steps; i++)
        {
            Update(grid, true);
            if (printAfterEachStep)
                PrintGrid(grid, $"After {i + 1} step(s):");
        }

        int lightsOn = grid.SelectMany(row => row).Count(it => it.IsOn);
        return lightsOn;
    }
    
    private const string Sample = """
                                  .#.#.#
                                  ...##.
                                  #....#
                                  ..#...
                                  #.#..#
                                  ####.
                                  """;

    private static List<List<LightCell2D>> ReadInput(IEnumerable<string> lines)
    {
        List<List<LightCell2D>> grid = [];
        int x = 0;
        int y = 0;
        foreach (string line in lines)
        {
            grid.Add(new List<LightCell2D>(line.Length));
            foreach (char c in line)
            {
                grid[y].Add(LightCell2D.FromChar(x, y, c));
                grid[y][x].Connect(grid);
                x++;
            }

            y++;
            x = 0;
        }

        return grid;
    }
    
    private static void PrintGrid(List<List<LightCell2D>> grid, string s)
    {
        Console.Error.WriteLine(s);
        foreach (List<LightCell2D> row in grid)
        {
            foreach (LightCell2D cell in row)
                Console.Error.Write(cell.Char);
            Console.Error.WriteLine();
        }
        Console.Error.WriteLine();
    }
    
    private static void Update(List<List<LightCell2D>> grid, bool part2 = false)
    {
        foreach (LightCell2D cell in grid.SelectMany(row => row)) 
            cell.CalculateNewState(part2);
        
        foreach (LightCell2D cell in grid.SelectMany(row => row)) 
            cell.ApplyNewState();
    }
    
    private static bool IsInRange(this int i, int lowerBound, int upperBound)
    {
        return i >= lowerBound && i <= upperBound;
    }
    
    [DebuggerDisplay("{X}|{Y}: {IsOn}")]
    private sealed class LightCell2D : Cell2D
    {
        public bool IsOn { get; private set; }
        public bool NewIsOn { get; private set; }
        public char Char => IsOn ? '#' : '.';

        public LightCell2D(int x, int y, bool isOn) : base(x, y)
        {
            IsOn = isOn;
        }
        
        public static LightCell2D FromChar(int x, int y, char c) => new(x, y, c == '#');

        public void Connect(List<List<LightCell2D>> grid)
        {
            if (X > 0) 
                ConnectLeft(grid[Y][X - 1]);
            
            if (X > 0 && Y > 0)
                ConnectUpLeft(grid[Y - 1][X - 1]);
            
            if (Y > 0)
                ConnectUp(grid[Y - 1][X]);
            
            if (Y > 0 && X < grid[Y - 1].Count - 1)
                ConnectUpRight(grid[Y - 1][X + 1]);
        }

        public void CalculateNewState(bool part2 = false)
        {
            if (part2)
            {
                bool isTopLeft = Left is null && Up is null;
                bool isTopRight = Right is null && Up is null;
                bool isBottomRight = Right is null && Down is null;
                bool isBottomLeft = Left is null && Down is null;

                if (isTopLeft || isTopRight || isBottomRight || isBottomLeft)
                {
                    NewIsOn = true;
                    return;
                }
            }
            
            NewIsOn = IsOn
                ? Neighbors.Cast<LightCell2D>().Count(it => it.IsOn).IsInRange(2, 3)
                : Neighbors.Cast<LightCell2D>().Count(it => it.IsOn) == 3;
        }

        public void ApplyNewState()
        {
            IsOn = NewIsOn;
        }
    }
}