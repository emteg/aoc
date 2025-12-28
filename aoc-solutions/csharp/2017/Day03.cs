using Common;

namespace _2017;

public static class Day03
{
    /* 65  64  63  62  61  60  59  58  57
     * 66  37  36  35  34  33  32  31  56
     * 67  38  17  16  15  14  13  30  55
     * 68  39  18   5   4   3  12  29  54
     * 69  40  19   6   1   2  11  28  53
     * 70  41  20   7   8   9  10  27  52
     * 71  42  21  22  23  24  25  26  51
     * 72  43  44  45  46  47  48  49  50
     * 73  74  75  76  77  78  79  80  81
     */
    
    public static string Part1(IEnumerable<string> input)
    {
        int desiredNumber = int.Parse(input.First());
        const int numberGrowthPerSpiral = 8;
        int additionalNumbersPerSpiral = 0;
        int lowerSpiralStartingBottomRightValue = 0;
        int upperSpiralStartingBottomRightValue = 1;
        int lowerSpiralStartingX = -1;
        int lowerSpiralStartingY = -1;
        int spiralWidth = 1;

        if (desiredNumber == 1)
        {
            return "0";
        }
        
        while (upperSpiralStartingBottomRightValue < desiredNumber)
        {
            additionalNumbersPerSpiral += numberGrowthPerSpiral;
            lowerSpiralStartingBottomRightValue = upperSpiralStartingBottomRightValue;
            upperSpiralStartingBottomRightValue += additionalNumbersPerSpiral;
            lowerSpiralStartingX++;
            lowerSpiralStartingY++;
            spiralWidth += 2;
        }
        
        int current = lowerSpiralStartingBottomRightValue;
        int side = 0;
        int goal = current + 1;
        int x = lowerSpiralStartingX;
        int y = lowerSpiralStartingY;
        while (current < desiredNumber)
        {
            current++;
            
            if (side == 0) // at first we have to move one to the right
            {
                x++;
                side = 1;
                goal = current + spiralWidth - 1;
                continue;
            }

            if (side == 1) // on the right edge we move upwards until goal
            {
                y--;
                if (current == goal)
                {
                    side = 2;
                    goal += spiralWidth - 1;
                }
                continue;
            }

            if (side == 2) // on the top edge we move to the left until goal
            {
                x--;
                if (current == goal)
                {
                    side = 3;
                    goal += spiralWidth - 1;
                }
                continue;
            }

            if (side == 3) // on the left edge we move down until goal
            {
                y++;
                if (current == goal)
                {
                    side = 4;
                    goal = upperSpiralStartingBottomRightValue;
                }
                continue;
            }
            // on the bottom edge we move to the right until goal
            x++;
        }
        
        int distance = Math.Abs(x) + Math.Abs(y);
        return distance.ToString();
    }

    public static string Part1Sample() => Part1(["1024"]);

    public static string Part2(IEnumerable<string> input)
    {
        Dictionary<(int x, int y), SpiralCell> cells = [];
        SpiralCell last = new((0, 0)) { Value = 1 };
        cells.Add(last.Location, last);
        
        int maxCoordinate = 0;
        bool newLayer = true;
        long desiredValue = long.Parse(input.First());
        Direction direction = Direction.Right;
        int x = 0;
        int y = 0;

        while (last.Value <= desiredValue)
        {
            if (direction is Direction.Right && newLayer)
            {
                maxCoordinate++;
                newLayer = false;
                x++;
                direction = Direction.Up;
            }
            else if (direction is Direction.Up)
            {
                y++;
                if (Math.Abs(y) >= maxCoordinate) 
                    direction = Direction.Left;
            }
            else if (direction is Direction.Left)
            {
                x--;
                if (Math.Abs(x) >= maxCoordinate) 
                    direction = Direction.Down;
            }
            else if (direction is Direction.Down)
            {
                y--;
                if (Math.Abs(y) >= maxCoordinate) 
                    direction = Direction.Right;
            }
            else // right
            {
                x++;
                if (Math.Abs(x) >= maxCoordinate) 
                    newLayer = true;
            }
            SpiralCell newCell = new(x, y);
            newCell.ConnectAuto(cells);
            cells.Add(newCell.Location, newCell);
            newCell.Value = newCell.Neighbors.Cast<SpiralCell>().Select(cell => cell.Value).Sum();
            
            last = newCell;
        }
        
        return last.Value.ToString();
    }

    public static string Part2Sample() => Part2(["54"]);

    private sealed class SpiralCell : Cell2D
    {
        public long Value { get; set; }
        
        public int ManhattanDistanceFromCenter => Math.Abs(X) + Math.Abs(Y);
        
        public SpiralCell(int x, int y) : base(x, y) { }

        public SpiralCell((int x, int y) location) : base(location.x, location.y) { }

        public void ConnectAuto(SpiralCell other)
        {
            if (other.X > X && other.Y == Y) // x > x -> right
            {
                ConnectRight(other);
                return;
            }
            if (other.X > X && other.Y > Y) // x > x -> right, y > y -> up
            {
                ConnectUpRight(other);
                return;
            }
            if (other.X == X && other.Y > Y) // y > y -> up
            {
                ConnectUp(other);
                return;
            }
            if (other.X < X && other.Y > Y) // x < x -> left, y > y -> up
            {
                ConnectUpLeft(other);
                return;
            }
            if (other.X < X && other.Y == Y) // x < x -> left
            {
                ConnectLeft(other);
                return;
            }
            if (other.X < X && other.Y < Y) // x < x -> left, y < y -> down
            {
                ConnectDownLeft(other);
                return;
            }
            if (other.X == X && other.Y < Y) // y < y -> down
            {
                ConnectDown(other);
                return;
            }
            if (other.X > X && other.Y < Y) // x > x -> right, y < y -> down
            {
                ConnectDownRight(other);
                return;
            }
        }

        public void ConnectAuto(Dictionary<(int x, int y), SpiralCell> cells)
        {
            foreach ((int x, int y) location in NeighborCoordinates(true, true))
            {
                if (!cells.TryGetValue(location, out SpiralCell? other))
                    continue;
                
                ConnectAuto(other);
            }
        }

        public override string ToString() => $"({X},{Y}) - {Value}";
    }
}