using System.Collections;

namespace _2015;

public static class Day06
{
    public static string Part1(IEnumerable<string> input) 
    {
        BitArray[] lightGrid = new BitArray[1000];
        for (int i = 0; i < lightGrid.Length; i++) 
            lightGrid[i] = new BitArray(1000);

        IEnumerable<Instruction> instructions = input.Select(ParseInstruction);
        
        foreach (Instruction instruction in instructions) 
            instruction.Apply(lightGrid);

        return lightGrid.CountLitLights().ToString();
    }
    
    public static string Part2(IEnumerable<string> input) 
    {
        ushort[][] lightGrid = new ushort[1000][];
        for (int i = 0; i < lightGrid.Length; i++)
            lightGrid[i] = new ushort[1000];
        
        IEnumerable<Instruction> instructions = input.Select(ParseInstruction);
        
        foreach (Instruction instruction in instructions) 
            instruction.Apply(lightGrid);

        return lightGrid.CountLitLights().ToString();
    }
    
    private static void Apply(this Instruction instruction, BitArray[] grid)
    {
        for (int y = instruction.StartY; y <= instruction.EndY; y++)
        {
            for (int x = instruction.StartX; x <= instruction.EndX; x++)
            {
                switch (instruction.Action)
                {
                    case Action.TurnOn:
                    case Action.TurnOff:
                        grid[y].Set(x, instruction.Action is Action.TurnOn);
                        break;
                    case Action.Toggle:
                        grid[y].Set(x, !grid[y][x]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    private static void Apply(this Instruction instruction, ushort[][] grid)
    {
        for (int y = instruction.StartY; y <= instruction.EndY; y++)
        {
            for (int x = instruction.StartX; x <= instruction.EndX; x++)
            {
                switch (instruction.Action)
                {
                    case Action.TurnOn:
                        grid[y][x] += 1;
                        break;
                    case Action.TurnOff:
                        grid[y][x] -= grid[y][x] > 0 ? (ushort)1 : (ushort)0;
                        break;
                    case Action.Toggle:
                        grid[y][x] += 2;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
    
    private static int CountLitLights(this BitArray[] grid)
    {
        int result = 0;
        
        foreach (BitArray row in grid) 
            result += row.Cast<bool>().Count(it => it);

        return result;
    }
    
    private static int CountLitLights(this ushort[][] grid)
    {
        return grid.Sum(row => row.Select(it => (int)it).Sum());
    }

    private static Instruction ParseInstruction(this string s)
    {
        string[] words = s.Split(' ');
            
        (Action action, int nextWordIndex) = (words[0], words[1]) switch
        {
            ("turn", "on") => (Action.TurnOn, 2),
            ("turn", "off") => (Action.TurnOff, 2),
            ("toggle", _) => (Action.Toggle, 1),
            _ => throw new ArgumentException($"Unknown action: {words[0]} {words[1]}")
        };
            
        ushort[] start = words[nextWordIndex].Split(',').Select(ushort.Parse).ToArray();
        ushort[] end = words[nextWordIndex + 2].Split(',').Select(ushort.Parse).ToArray();
            
        return new Instruction(action, start[0], start[1], end[0], end[1]);
    }
    
    private enum Action
    {
        TurnOn,
        Toggle,
        TurnOff
    }

    private readonly struct Instruction
    {
        public readonly Action Action;
        public readonly ushort StartX;
        public readonly ushort StartY;
        public readonly ushort EndX;
        public readonly ushort EndY;

        public Instruction(Action action, ushort startX, ushort startY, ushort endX, ushort endY)
        {
            Action = action;
            StartX = startX;
            StartY = startY;
            EndX = endX;
            EndY = endY;
        }
    }
}