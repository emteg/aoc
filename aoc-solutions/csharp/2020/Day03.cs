using Common;

namespace _2020;

public static class Day03
{
    public static string Part1(IEnumerable<string> input)
    {
        List<List<MapCell2D>> grid = BuildGrid(input);
        int treesEncountered = TraverseGrid(3, 1, grid);
        return treesEncountered.ToString();
    }

    public static string Part2(IEnumerable<string> input)
    {
        List<List<MapCell2D>> grid = BuildGrid(input);
        List<(int right, int down)> slopes =
        [
            (1, 1),
            (3, 1),
            (5, 1),
            (7, 1),
            (1, 2)
        ];
        int result = 1;

        foreach ((int right, int down) in slopes) 
            result *= TraverseGrid(right, down, grid);
        
        return result.ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());

    private static void ResetGrid(List<List<MapCell2D>> grid)
    {
        foreach (MapCell2D cell in grid.SelectMany(row => row))
            cell.Visited = false;
    }

    private static int TraverseGrid(int right, int down, List<List<MapCell2D>> grid)
    {
        ResetGrid(grid);
        
        MapCell2D? current = grid[0][0];
        int treesEncountered = 0;
        while (current is not null)
        {
            
            current = current.Traverse(right, down);
            current?.Visited = true;
            if (current?.HasTree is true)
                treesEncountered++;
        }

        return treesEncountered;
    }

    private static List<List<MapCell2D>> BuildGrid(IEnumerable<string> input)
    {
        List<List<MapCell2D>> grid = [];
        int y = 0;
        foreach (string line in input)
        {
            grid.Add([]);
            int x = 0;
            foreach (char c in line)
            {
                _ = new MapCell2D(x, y, c, line.Length, grid);
                x++;
            }
            y++;
        }

        return grid;
    }

    private const string Sample = """
                                  ..##.......
                                  #...#...#..
                                  .#....#..#.
                                  ..#.#...#.#
                                  .#...##..#.
                                  ..#.##.....
                                  .#.#.#....#
                                  .#........#
                                  #.##...#...
                                  #...##....#
                                  .#..#...#.#
                                  """;

    private sealed class MapCell2D : Cell2D
    {
        public bool Visited { get; set; }
        public bool HasTree { get; }

        public MapCell2D(int x, int y, char c, int width, List<List<MapCell2D>> grid) : base(x, y)
        {
            HasTree = c == '#';
            
            grid[y].Add(this);
            
            if (y > 0) // connect up, if not first row
                ConnectUp(grid[y - 1][x]);
            
            if (x > 0) // connect left, if not first column
                ConnectLeft(grid[y][x - 1]);
            
            if (x == width - 1) // connect "wrap around" from last column to first column, if last column
                ConnectRight(grid[y][0]);
        }

        public MapCell2D? ThreeRightOneDown() => Traverse(3, 1);

        public MapCell2D? Traverse(int right, int down)
        {
            if (Down is null) // we are done, if the bottom row has been reached
                return null;

            Cell2D? result = this;
            for (int i = 0; i < right; i++) 
                result = result?.Right;

            for (int i = 0; i < down; i++) 
                result = result?.Down;

            return (MapCell2D?)result;
        }

        public override string ToString()
        {
            return (HasTree, Visited) switch
            {
                (true, true) => "X",
                (true, false) => "#",
                (false, true) => "O",
                (false, false) => "."
            };
        }
    }
}