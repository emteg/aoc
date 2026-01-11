using System.Diagnostics;
using Common;

namespace _2024;

public static class Day08
{
    public static string Part1(IEnumerable<string> input)
    {
        List<List<AntennaCell>> grid = ReadMap(input);
        Dictionary<char, List<AntennaCell>> antennas = GetAntennas(grid);

        int antinodes = 0;
        
        foreach ((_, List<AntennaCell> cells) in antennas)
        {
            foreach (AntennaCell currentCell in cells)
            {
                foreach (AntennaCell otherCell in cells.Where(cell => cell != currentCell))
                {
                    (int deltaX, int deltaY) delta = currentCell.DistanceTo(otherCell);
                    AntennaCell? antinode = otherCell.RelativeNeighbor(delta);
                    if (antinode is null)
                        continue;

                    if (!antinode.HasAntinode)
                    {
                        antinode.HasAntinode = true;
                        antinodes++;
                    }
                }
            }
        }
        
        return antinodes.ToString();
    }

    public static string Part1Sample() => Part1(Sample1.Lines());

    public static string Part2(IEnumerable<string> input)
    {
        List<List<AntennaCell>> grid = ReadMap(input);
        Dictionary<char, List<AntennaCell>> antennas = GetAntennas(grid);

        foreach ((_, List<AntennaCell> cells) in antennas)
        {
            foreach (AntennaCell currentCell in cells)
            {
                currentCell.HasAntinode = true;
                foreach (AntennaCell otherCell in cells.Where(cell => cell != currentCell))
                {
                    otherCell.HasAntinode = true;
                    (int deltaX, int deltaY) delta = currentCell.DistanceTo(otherCell);
                    AntennaCell cell = otherCell;
                    while (true)
                    {
                        AntennaCell? antinode = cell.RelativeNeighbor(delta);
                        if (antinode is null)
                            break;
                        
                        antinode.HasAntinode = true;
                        
                        cell = antinode;
                    }
                }
            }
        }
        
        PrintGrid(grid);
        
        return grid.SelectMany(row => row).Count(cell => cell.HasAntinode).ToString();
    }

    public static string Part2Sample() => Part2(Sample2.Lines());

    private const string Sample1 = """
                                   ............
                                   ........0...
                                   .....0......
                                   .......0....
                                   ....0.......
                                   ......A.....
                                   ............
                                   ............
                                   ........A...
                                   .........A..
                                   ............
                                   ............
                                   """;

    private const string Sample2 = """
                                   T.........
                                   ...T......
                                   .T........
                                   ..........
                                   ..........
                                   ..........
                                   ..........
                                   ..........
                                   ..........
                                   ..........
                                   """;

    private static void PrintGrid(IEnumerable<IEnumerable<AntennaCell>> grid)
    {
        foreach (IEnumerable<AntennaCell> row in grid) 
            PrintGridRow(row);
    }

    private static void PrintGridRow(IEnumerable<AntennaCell> row)
    {
        foreach (AntennaCell antennaCell in row) 
            PrintCell(antennaCell);
        Console.Error.WriteLine();
    }

    private static Dictionary<char, List<AntennaCell>> GetAntennas(List<List<AntennaCell>> grid)
    {
        Dictionary<char, List<AntennaCell>> antennas = [];
        foreach (List<AntennaCell> row in grid)
        {
            foreach (AntennaCell cell in row.Where(cell => cell.HasAntenna))
            {
                if (antennas.TryGetValue(cell.Frequency, out List<AntennaCell>? cells))
                    cells.Add(cell);
                else
                    antennas.Add(cell.Frequency, [cell]);
            }
        }

        return antennas;
    }

    private static void PrintCell(AntennaCell cell)
    {
        Console.Error.Write(!cell.HasAntenna && cell.HasAntinode ? '#' : cell.Frequency);
    }
    
    private static List<List<AntennaCell>> ReadMap(IEnumerable<string> input)
    {
        List<List<AntennaCell>> grid = [];

        int y = 0;
        foreach (string line in input)
        {
            int x = 0;
            grid.Add([]);
 
            foreach (char c in line)
            {
                AntennaCell cell = new(c, x, y);
                grid[y].Add(cell);
                Cell2D.ConnectFour(cell, grid);
                x++;
            }
            y++;
        }
        
        return grid;
    }

    [DebuggerDisplay("Y: {Y} | X: {X} - {Frequency}")]
    private sealed class AntennaCell : Cell2D
    {
        public char Frequency { get; }
        public bool HasAntenna => Frequency != '.';
        public bool HasAntinode { get; set; }

        public AntennaCell(char frequency, int x, int y) : base(x, y)
        {
            Frequency = frequency;
        }

        public AntennaCell? RelativeNeighbor((int x, int y) delta)
        {
            AntennaCell? result = this;
            
            while (delta.x < 0)
            {
                result = (AntennaCell?)result?.Left;
                delta.x++;
            }
            
            while (delta.x > 0)
            {
                result = (AntennaCell?)result?.Right;
                delta.x--;
            }
            
            while (delta.y < 0)
            {
                result = (AntennaCell?)result?.Up;
                delta.y++;
            }
            
            while (delta.y > 0)
            {
                result = (AntennaCell?)result?.Down;
                delta.y--;
            }

            return result;
        }
    }
}