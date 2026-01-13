using System.Diagnostics;
using Common;

namespace _2024;

public static class Day12
{
    public static string Part1(IEnumerable<string> input)
    {
        (List<List<GardenPlot>> grid, HashSet<Region> regions) = BuildGrid(input);
        PrintGrid(grid);
        return regions.Sum(region => region.Price1).ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        (List<List<GardenPlot>> grid, HashSet<Region> regions) = BuildGrid(input);
        PrintGrid(grid);
        return regions.Sum(region => region.Price2).ToString();
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private const string Sample = """
                                  RRRRIICCFF
                                  RRRRIICCCF
                                  VVRRRCCFFF
                                  VVRCCCJFFF
                                  VVVVCJJCFE
                                  VVIVCCJJEE
                                  VVIIICJJEE
                                  MIIIIIJJEE
                                  MIIISIJEEE
                                  MMMISSJEEE
                                  """;

    private static (List<List<GardenPlot>>, HashSet<Region>) BuildGrid(IEnumerable<string> input)
    {
        List<List<GardenPlot>> grid = [];
        int y = 0;
        foreach (string line in input)
        {
            int x = 0;
            grid.Add([]);
 
            foreach (char c in line)
            {
                GardenPlot cell = new(c, x, y);
                grid[y].Add(cell);
                Cell2D.ConnectFour(cell, grid);
                x++;
            }
            y++;
        }

        HashSet<Region> regions = [];
        foreach (GardenPlot plot in grid.SelectMany(row => row))
        {
            plot.BuildRegion();
            _ = regions.Add(plot.Region);
        }
        
        return (grid, regions);
    }
    
    private static void PrintGrid(List<List<GardenPlot>> grid)
    {
        ConsoleColor previousColor = Console.ForegroundColor;
        foreach (List<GardenPlot> row in grid)
        {
            foreach (GardenPlot plot in row)
            {
                Console.ForegroundColor = plot.Region.Color;
                Console.Error.Write(plot.PlantType);
            }
            Console.Error.WriteLine();
        }
        Console.ForegroundColor = previousColor;
    }

    [DebuggerDisplay("{PlantType} (A: {Area}, P: {Perimeter})")]
    private sealed class Region
    {
        public required char PlantType { get; init; }
        public readonly List<GardenPlot> Plots = [];
        public int Area => Plots.Count;
        public int Perimeter => Plots.Sum(plot => plot.Perimeter);
        public int Price1 => Area  * Perimeter;
        public int Price2 => Area * Sides;
        public readonly ConsoleColor Color = GetNewColor();

        public int Sides
        {
            get
            {
                int upSides = Plots.Sum(plot => plot.UpCorners);
                int downSides = Plots.Sum(plot => plot.DownCorners);
                int leftSides = Plots.Sum(plot => plot.LeftCorners);
                int rightSides = Plots.Sum(plot => plot.RightCorners);
                return (upSides + downSides + leftSides + rightSides) / 2;
            }
        }

        private static ConsoleColor GetNewColor()
        {
            ConsoleColor result = Enum.GetValues<ConsoleColor>()[lastColorIndex];
            lastColorIndex++;
            if (lastColorIndex >= ConsoleColorCount)
                lastColorIndex = 1; // we dont want black
            return result;
        }
        private static readonly int ConsoleColorCount = Enum.GetValues<ConsoleColor>().Length;
        private static int lastColorIndex = 1; // we dont want black
    }

    [DebuggerDisplay("{PlantType} (Y: {Y}. X: {X}, P: {Perimeter}, B: {BorderString})")]
    private sealed class GardenPlot : Cell2D
    {
        public char PlantType { get; }
        public IReadOnlySet<Direction> Borders => borders;
        public Region Region => region!;
        public int Perimeter => Borders.Count;

        public int UpCorners => (HasUpLeftCorner ? 1 : 0) + (HasUpRightCorner ? 1 : 0);
        public int DownCorners => (HasDownLeftCorner ? 1 : 0) + (HasDownRightCorner ? 1 : 0);
        public int LeftCorners => (HasUpLeftCorner ? 1 : 0) + (HasDownLeftCorner ? 1 : 0);
        public int RightCorners => (HasUpRightCorner ? 1 : 0) + (HasDownRightCorner ? 1 : 0);
        
        public bool HasUpLeftCorner => DefinesUpLeftCorner || HasLeftUpCornerBecauseNeighborIsNotInRegion;
        public bool HasUpRightCorner => DefinesUpRightCorner || HasRightUpCornerBecauseNeighborIsNotInRegion;
        public bool HasDownRightCorner => DefinesDownRightCorner || HasRightDownCornerBecauseNeighborIsNotInRegion;
        public bool HasDownLeftCorner => DefinesDownLeftCorner || HasLeftDownCornerBecauseNeighborIsNotInRegion;
        
        // Corners being defined by the walls of THIS cell
        public bool DefinesUpLeftCorner => borders.Contains(Direction.Up) && borders.Contains(Direction.Left);
        public bool DefinesUpRightCorner => borders.Contains(Direction.Up) && borders.Contains(Direction.Right);
        public bool DefinesDownLeftCorner => borders.Contains(Direction.Down) && borders.Contains(Direction.Left);
        public bool DefinesDownRightCorner => borders.Contains(Direction.Down) && borders.Contains(Direction.Right);
        
        // Corners being defined by this cell's diagonal neighbors
        public bool HasLeftUpCornerBecauseNeighborIsNotInRegion
        {
            get
            {
                if (Left is not GardenPlot left)
                    return false;

                if (Up is not GardenPlot up)
                    return false;

                if (left.Up is not GardenPlot leftUp)
                    return false;

                return left.Region == Region && up.Region == Region && leftUp.Region != Region;
            }
        }
        public bool HasRightUpCornerBecauseNeighborIsNotInRegion
        {
            get
            {
                if (Right is not GardenPlot right)
                    return false;
                
                if (Up is not GardenPlot up)
                    return false;

                if (right.Up is not GardenPlot rightUp)
                    return false;
                
                return right.Region == Region && up.Region == Region && rightUp.Region != Region;
            }
        }
        public bool HasRightDownCornerBecauseNeighborIsNotInRegion
        {
            get
            {
                if (Right is not GardenPlot right)
                    return false;
                
                if (Down is not GardenPlot down)
                    return false;

                if (right.Down is not GardenPlot rightDown)
                    return false;
                
                return right.Region == Region && down.Region == Region && rightDown.Region != Region;
            }
        }
        public bool HasLeftDownCornerBecauseNeighborIsNotInRegion
        {
            get
            {
                if (Left is not GardenPlot left)
                    return false;
                
                if (Down is not GardenPlot down)
                    return false;

                if (left.Down is not GardenPlot leftDown)
                    return false;
                
                return left.Region == Region && down.Region == Region && leftDown.Region != Region;
            }
        }

        public string BorderString
        {
            get
            {
                char[] chars =
                [
                    borders.Contains(Direction.Left) ? '|' : ' ',
                    borders.Contains(Direction.Up) ? '-' : ' ',
                    borders.Contains(Direction.Down) ? '_' : ' ',
                    borders.Contains(Direction.Right) ? '|' : ' '
                ];
                return new string(chars);
            }
        }

        public GardenPlot(char c, int x, int y) : base(x, y)
        {
            PlantType = c;
        }

        public void BuildRegion(Region? reg = null)
        {
            if (region is null)
            {
                region = reg ?? new Region { PlantType = PlantType };
                Region.Plots.Add(this);
            }
            
            foreach (GardenPlot plot in Neighbors.Cast<GardenPlot>().Where(plot => plot.PlantType == PlantType && plot.region is null))
            {
                plot.BuildRegion(Region);
            }

            if (Up is null || ((GardenPlot)Up).Region != Region)
                borders.Add(Direction.Up);
            
            if (Right is null || ((GardenPlot)Right).Region != Region)
                borders.Add(Direction.Right);
            
            if (Down is null || ((GardenPlot)Down).Region != Region)
                borders.Add(Direction.Down);
            
            if (Left is null || ((GardenPlot)Left).Region != Region)
                borders.Add(Direction.Left);
        }

        private readonly HashSet<Direction> borders = [];
        private Region? region = null;
    }
}