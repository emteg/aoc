using Common;

namespace _2025;

public static class Day04
{
    public static string Part1(IEnumerable<string> input)
    {
        List<List<RollCell2D>> cells = BuildGrid(input);
        int count = cells.SelectMany(list => list).Count(it => it.AccessibleByForklift);
        return count.ToString();
    }
    
    public static string Part2(IEnumerable<string> input)
    {
        int numberOfRollsRemoved = 0;
        
        List<List<RollCell2D>> cells = BuildGrid(input);
        
        while (true)
        {
            int rollsRemovedThisRound = 0;
            RollCell2D[] accessibleCells = cells
                .SelectMany(list => list)
                .Where(cell => cell.AccessibleByForklift)
                .ToArray();
            foreach (RollCell2D cell in accessibleCells)
            {
                cell.RollRemoved();
                numberOfRollsRemoved++;
                rollsRemovedThisRound++;
            }

            if (rollsRemovedThisRound == 0)
                break;
        }
        
        return numberOfRollsRemoved.ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private const string Sample = """
        ..@@.@@@@.
        @@@.@.@.@@
        @@@@@.@.@@
        @.@@@@..@.
        @@.@@@@.@@
        .@@@@@@@.@
        .@.@.@.@@@
        @.@@@.@@@@
        .@@@@@@@@.
        @.@.@@@.@.
        """;
    
    private static List<List<RollCell2D>> BuildGrid(IEnumerable<string> lines)
    {
        List<List<RollCell2D>> cells = [];
        int y = 0;
        foreach (string line in lines)
        {
            int x = 0;
            int width = line.Length;
            cells.Add([]);

            foreach (char c in line)
            {
                RollCell2D cell = new(x, y, c);
                cells[y].Add(cell);
                if (x > 0)
                    cell.ConnectLeft(cells[y][x - 1]);
                if (y > 0)
                    cell.ConnectUp(cells[y - 1][x]);
                if (x > 0 && y > 0)
                    cell.ConnectUpLeft(cells[y - 1][x - 1]);
                if (y > 0 && x < width - 1)
                    cell.ConnectUpRight(cells[y - 1][x + 1]);
                x++;
            }
            y++;
        }

        return cells;
    }
    
    private sealed class RollCell2D : Cell2D
    {
        public bool HasRoll { get; private set; }

        public bool AccessibleByForklift => HasRoll && AdjacentRollsOfPaper < 4;
    
        public int AdjacentRollsOfPaper => HasRoll 
            ? Neighbors
                .Cast<RollCell2D>()
                .Count(rollCell => rollCell.HasRoll)
            : 0;

        public RollCell2D(int x, int y, char c)
            : base(x, y)
        {
            HasRoll = c == '@';
        }

        public void RollRemoved()
        {
            HasRoll = false;
        }
    }
}