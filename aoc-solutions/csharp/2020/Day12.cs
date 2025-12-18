using Common;

namespace _2020;

public static class Day12
{
    public static string Part1(IEnumerable<string> input)
    {
        Ship ship = new();
        ship.Apply(input.Select(Instruction.Parse));
        int result = Math.Abs(ship.X) + Math.Abs(ship.Y);
        return result.ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        ShipWithWaypoint ship = new();
        ship.Apply(input.Select(Instruction.Parse));
        int result = Math.Abs(ship.ShipX) + Math.Abs(ship.ShipY);
        return result.ToString();
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());

    private sealed class Ship
    {
        public int X { get; private set; } = 0;
        public int Y { get; private set; } = 0;
        public Direction Direction { get; private set; } = Direction.Right;

        public void Apply(IEnumerable<Instruction> instructions)
        {
            foreach (Instruction instruction in instructions) 
                Apply(instruction);
        }

        private void Apply(Instruction instruction)
        {
            (X, Y, Direction) = instruction switch
            {
                { Action: Action.StrafeNorth } => (X,                      Y + instruction.Amount, Direction),
                { Action: Action.StrafeEast  } => (X + instruction.Amount, Y,                      Direction),
                { Action: Action.StrafeSouth } => (X,                      Y - instruction.Amount, Direction),
                { Action: Action.StrafeWest  } => (X - instruction.Amount, Y,                      Direction),
                { Action: Action.RotateLeft  } => RotateLeft(instruction.Amount),
                { Action: Action.RotateRight } => RotateRight(instruction.Amount),
                { Action: Action.Forward     } => MoveForward(instruction.Amount),
                _ => throw new InvalidOperationException()
            };
        }

        private (int x, int y, Direction direction) MoveForward(int amount)
        {
            return Direction switch
            {
                Direction.Up => (X, Y + amount, Direction),
                Direction.Right => (X + amount, Y, Direction),
                Direction.Down => (X, Y - amount, Direction),
                Direction.Left => (X - amount, Y, Direction),
                _ => throw new InvalidOperationException()
            };
        }

        private (int x, int y, Direction direction) RotateLeft(int amount)
        {
            Direction direction = Direction;

            while (amount > 0)
            {
                direction = direction switch
                {
                    Direction.Up => Direction.Left,
                    Direction.Right => Direction.Up,
                    Direction.Down => Direction.Right,
                    Direction.Left => Direction.Down,
                    _ => throw new InvalidOperationException()
                };
                amount -= 90;
            }
            
            return (X, Y, direction);
        }
        
        private (int x, int y, Direction direction) RotateRight(int amount)
        {
            Direction direction = Direction;
            
            while (amount > 0)
            {
                direction = direction switch
                {
                    Direction.Up => Direction.Right,
                    Direction.Right => Direction.Down,
                    Direction.Down => Direction.Left,
                    Direction.Left => Direction.Up,
                    _ => throw new InvalidOperationException()
                };
                amount -= 90;
            }
            
            return (X, Y, direction);
        }
    }

    private sealed class ShipWithWaypoint
    {
        public int ShipX { get; private set; } = 0;
        public int ShipY { get; private set; } = 0;
        public int WaypointRelativeX { get; private set; } = 10;
        public int WaypointRelativeY { get; private set; } = 1;
        
        public void Apply(IEnumerable<Instruction> instructions)
        {
            foreach (Instruction instruction in instructions) 
                Apply(instruction);
        }

        private void Apply(Instruction instruction)
        {
            if (instruction.Action is Action.StrafeNorth)
                WaypointRelativeY += instruction.Amount;
            else if (instruction.Action is Action.StrafeEast)
                WaypointRelativeX += instruction.Amount;
            else if (instruction.Action is Action.StrafeSouth)
                WaypointRelativeY-= instruction.Amount;
            else if (instruction.Action is Action.StrafeWest)
                WaypointRelativeX -= instruction.Amount;
            else if (instruction.Action is Action.RotateLeft)
            {
                int degrees = instruction.Amount;
                while (degrees > 0)
                {
                    (WaypointRelativeX, WaypointRelativeY) = (-WaypointRelativeY, WaypointRelativeX);
                    degrees -= 90;
                }
            }
            else if (instruction.Action is Action.RotateRight)
            {
                int degrees = instruction.Amount;
                while (degrees > 0)
                {
                    (WaypointRelativeX, WaypointRelativeY) = (WaypointRelativeY, -WaypointRelativeX);
                    degrees -= 90;
                }
            }
            else
            {
                ShipX += WaypointRelativeX * instruction.Amount;
                ShipY += WaypointRelativeY * instruction.Amount;
            }
        }
    }

    private readonly struct Instruction
    {
        public readonly Action Action;
        public readonly ushort Amount;

        public static Instruction Parse(string s)
        {
            Action action = s[0] switch
            {
                'N' => Action.StrafeNorth,
                'E' => Action.StrafeEast,
                'S' => Action.StrafeSouth,
                'W' => Action.StrafeWest,
                'L' => Action.RotateLeft,
                'R' => Action.RotateRight,
                _ => Action.Forward
            };
            ushort amount = ushort.Parse(s[1..]);
            return new Instruction(action, amount);
        }

        public override string ToString()
        {
            char c = Action switch
            {
                Action.StrafeNorth => 'N', 
                Action.StrafeEast  => 'E',
                Action.StrafeSouth => 'S',
                Action.StrafeWest  => 'W',
                Action.RotateLeft  => 'L',
                Action.RotateRight => 'R',
                _ => 'F',
            };
            return $"{c}{Amount}";
        }

        private Instruction(Action action, ushort amount)
        {
            Action = action;
            Amount = amount;
        }
    }

    private enum Action
    {
        StrafeNorth,
        StrafeEast,
        StrafeSouth,
        StrafeWest,
        RotateLeft,
        RotateRight,
        Forward
    }
    
    private const string Sample = """
                                  F10
                                  N3
                                  F7
                                  R90
                                  F11
                                  """;
}