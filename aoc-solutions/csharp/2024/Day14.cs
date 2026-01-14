using System.Diagnostics;
using Common;

namespace _2024;

public static class Day14
{
    public static string Part1(IEnumerable<string> input)
    {
        List<List<Cell>> grid = CreateGrid(101, 103);
        List<Robot> robots = PlaceRobots(input, grid);
        UpdateRobots(robots);
        return SafetyFactor(grid).ToString();
    }

    public static string Part1Sample() => "No sample implemented for this puzzle";
    
    public static string Part2(IEnumerable<string> input)
    {
        bool canSetCursorPosition;
        try
        {
            Console.CursorLeft = 0;
            canSetCursorPosition = true;
        }
        catch (Exception)
        {
            canSetCursorPosition = false;
        }
        
        List<List<Cell>> grid = CreateGrid(101, 103);
        List<Robot> robots = PlaceRobots(input, grid);
        int iterations = 0;

        if (canSetCursorPosition)
        {
            Console.Clear();
            Console.Error.WriteLine();
            Console.Error.Write(ProgressBar(0));
            Console.CursorLeft = 0;
            Console.CursorVisible = false;
        }

        while (true)
        {
            foreach (Robot robot in robots)
                robot.Update();

            iterations++;
            if (iterations % 250 == 0)
            {
                if (canSetCursorPosition)
                {
                    Console.Error.Write(ProgressBar(iterations));
                    Console.CursorLeft = 0;
                }
                else
                    Console.Error.WriteLine($"{iterations,-4} of estimated 8000 iterations");
            }

            if (GridContainsConsecutiveCellWithRobots(grid, 20))
            {
                Console.Error.WriteLine(canSetCursorPosition
                    ? ProgressBar(iterations, iterations)
                    : $"Finished after {iterations} iterations.");
                break;
            }
        }

        if (canSetCursorPosition)
            Console.CursorVisible = true;
        return iterations.ToString();
    }

    private static string ProgressBar(int progress, double max = 8000, int barLength = 20)
    {
        double percent = progress / max;
        int filledBars = (int)(barLength * percent);
        int openBars = barLength - filledBars;
        return $"Progress: {new string('\u2588', filledBars)}{new string('\u2591', openBars)} {progress} iterations";
    }
    
    public static string Part2Sample() => "No sample implemented for this puzzle";
    
    private static bool GridContainsConsecutiveCellWithRobots(List<List<Cell>> grid, uint minLength)
    {
        foreach (List<Cell> row in grid)
        {
            uint consecutiveCells = 0;
            foreach (Cell cell in row)
            {
                if (consecutiveCells >= minLength)
                    return true;
                if (consecutiveCells == 0 && cell.Robots.Count == 0)
                    continue;
                
                if (cell.Robots.Count > 0)
                    consecutiveCells++;
                
                else if (consecutiveCells > 0 && cell.Robots.Count == 0)
                    consecutiveCells = 0;
            }
        }

        return false;
    }
    
    private static List<List<Cell>> CreateGrid(uint width, uint height)
    {
        List<List<Cell>> grid = [];

        for (uint y = 0; y < height; y++)
        {
            List<Cell> row = [];
            grid.Add(row);
            for (uint x = 0; x < width; x++)
            {
                Cell cell = new(x, y);
                row.Add(cell);
                if (x > 0)
                {
                    Cell left = row[(int)x - 1];
                    cell.ConnectLeft(left);
                }

                if (x == width - 1)
                {
                    Cell rowStart = row[0];
                    cell.ConnectRight(rowStart);
                }

                if (y > 0)
                {
                    Cell up = grid[(int)y - 1][(int)x];
                    cell.ConnectUp(up);
                }

                if (y == height - 1)
                {
                    Cell colStart = grid[0][(int)x];
                    cell.ConnectDown(colStart);
                }
            }
        }
        
        return grid;
    }
    
    private static List<Robot> PlaceRobots(IEnumerable<string> input, List<List<Cell>> grid)
    {
        List<Robot> robots = [];
        foreach (string line in input)
        {
            string[] properties = line.Split(' ');
            short[] position = properties[0].Substring(2).Split(',').Select(short.Parse).ToArray();
            short[] speed = properties[1].Substring(2).Split(',').Select(short.Parse).ToArray();
            
            Cell cell = grid[position[1]][position[0]];
            Robot robot = new(cell, speed[0], speed[1]);
            robots.Add(robot);
        }

        return robots;
    }

    private static void UpdateRobots(List<Robot> robots)
    {
        for (int i = 0; i < 100; i++)
        {
            foreach (Robot robot in robots) 
                robot.Update();
        }
    }
    
    private static uint SafetyFactor(List<List<Cell>> grid)
    {
        uint result = 1;
        
        result *= CountRobots(grid, 0, grid.Count / 2, 0, grid[0].Count / 2);
        result *= CountRobots(grid, grid.Count / 2 + 1, grid.Count, 0, grid[0].Count / 2);
        result *= CountRobots(grid, 0, grid.Count / 2, grid[0].Count / 2 + 1, grid[0].Count);
        result *= CountRobots(grid, grid.Count / 2 + 1, grid.Count, grid[0].Count / 2 + 1, grid[0].Count);
        
        return result;
    }

    private static uint CountRobots(List<List<Cell>> grid, int yMin, int yMax, int xMin, int xMax)
    {
        uint result = 0;
        
        for (int y = yMin; y < yMax; y++)
        for (int x = xMin; x < xMax; x++) 
            result += (uint)grid[y][x].Robots.Count;

        return result;
    }
    
    [DebuggerDisplay("p={Cell.X},{Cell.Y} v={DeltaX},{DeltaY}")]
    private sealed class Robot
    {
        public readonly short DeltaX;
        public readonly short DeltaY;
        public Cell Cell { get; private set; }

        public Robot(Cell cell, short deltaX, short deltaY)
        {
            Cell = cell;
            DeltaX = deltaX;
            DeltaY = deltaY;
            cell.AddRobot(this);
        }

        public void Update()
        {
            short toDo = DeltaX;
            while (toDo > 0)
            {
                MoveTo(Cell.Right);
                toDo--;
            }

            while (toDo < 0)
            {
                MoveTo(Cell.Left);
                toDo++;
            }

            toDo = DeltaY;
            while (toDo > 0)
            {
                MoveTo(Cell.Down);
                toDo--;
            }

            while (toDo < 0)
            {
                MoveTo(Cell.Up);
                toDo++;
            }
        }

        private void MoveTo(Cell cell)
        {
            Cell.RemoveRobot(this);
            Cell = cell;
            Cell.AddRobot(this);
        }
    }

    [DebuggerDisplay("{X},{Y}: {Robots.Count}")]
    private sealed class Cell
    {
        public readonly uint X;
        public readonly uint Y;
        public Cell Right { get; private set; } = null!;
        public Cell Left { get; private set; } = null!;
        public Cell Up { get; private set; } = null!;
        public Cell Down { get; private set; } = null!;
        public readonly List<Robot> Robots = [];

        public Cell(uint x, uint y)
        {
            X = x;
            Y = y;
        }

        public void ConnectLeft(Cell left)
        {
            Left = left;
            left.Right = this;
        }

        public void ConnectUp(Cell up)
        {
            Up = up;
            up.Down = this;
        }

        public void ConnectRight(Cell right)
        {
            Right = right;
            right.Left = this;
        }

        public void ConnectDown(Cell down)
        {
            Down = down;
            down.Up = this;
        }

        public void AddRobot(Robot robot) => Robots.Add(robot);

        public void RemoveRobot(Robot robot) => Robots.Remove(robot);

        public override string ToString()
        {
            return Robots.Count == 0 
                ? "." 
                : Robots.Count.ToString();
        }
    }
}