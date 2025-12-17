using System.Diagnostics;
using Common;

namespace _2015;

public static class Day07
{
    public static string Part1(IEnumerable<string> input)
    {
        return Execute(input, []).ToString();
    }

    public static string Part2(IEnumerable<string> input)
    {
        ushort a = Execute(input, []);
        return Execute(input, [], a).ToString();
    }
    
    public static string Part1Sample()
    {
        Dictionary<string, ushort> wires = [];

        const string input = """
                             123 -> x
                             456 -> y
                             x AND y -> d
                             x OR y -> e
                             x LSHIFT 2 -> f
                             y RSHIFT 2 -> g
                             NOT x -> h
                             NOT y -> i
                             """;

        _ = Execute(input.Lines(), wires);

        List<string> wireNames = wires.Keys.ToList();
        wireNames.Sort();
        foreach (string wireName in wireNames) 
            Console.Error.WriteLine($"{wireName}: {wires[wireName]}");

        return string.Empty;
    }
    
    private static ushort Execute(IEnumerable<string> input, Dictionary<string, ushort> wires, ushort? overrideValueB = null)
    {
        List<Instruction> instructions = input.Select(ParseInstruction).ToList();

        if (overrideValueB.HasValue)
        {
            Instruction oldB = instructions.First(it => it.OutputWireName == "b");
            Instruction newB = new Instruction($"{overrideValueB.Value} -> b", oldB.Action, overrideValueB.Value, oldB.InputWireName1, oldB.InputWireName2, oldB.OutputWireName);
            int index = instructions.IndexOf(oldB);
            instructions.RemoveAt(index);
            instructions.Insert(index, newB);
        }
        

        int i = 0;
        while (instructions.Count > 0)
        {
            Instruction inst = instructions[i];
            if (TryExecute(inst, wires))
                instructions.RemoveAt(i);
            else
                i++;

            if (i == instructions.Count)
                i = 0;
        }

        return wires.GetValueOrDefault("a", (ushort)0);
    }
    
    private static bool TryExecute(Instruction instruction, Dictionary<string, ushort> wires)
    {
        if (instruction.Action is Action.SetValue)
        {
            wires[instruction.OutputWireName] = instruction.ConstValue;
            return true;
        }

        if (instruction.Action is Action.StoreValue)
        {
            if (!wires.TryGetValue(instruction.InputWireName1, out ushort value)) 
                return false;
            
            wires[instruction.OutputWireName] = value;
            return true;
        }

        if (instruction.Action is Action.Not)
        {
            if (!wires.TryGetValue(instruction.InputWireName1, out ushort value))
                return false;

            unchecked
            {
                wires[instruction.OutputWireName] = (ushort)~value;
            }
            return true;
        }

        if (instruction.Action is Action.LShift or Action.RShift)
        {
            if (!wires.TryGetValue(instruction.InputWireName1, out ushort value))
                return false;

            if (instruction.Action is Action.LShift)
                wires[instruction.OutputWireName] = (ushort)(value << instruction.ConstValue);
            else
                wires[instruction.OutputWireName] = (ushort)(value >> instruction.ConstValue);
            return true;
        }
        
        bool inputWire1Exists = wires.TryGetValue(instruction.InputWireName1, out ushort value1);
        if (instruction.InputWire1IsValue)
        {
            value1 = instruction.InputWire1Value;
            inputWire1Exists = true;
        }
        bool inputWire2Exists = wires.TryGetValue(instruction.InputWireName2, out ushort value2);
        if (!inputWire1Exists || !inputWire2Exists)
            return false;
        
        wires[instruction.OutputWireName] = instruction.Action is Action.And
            ? (ushort)(value1 & value2)
            : (ushort)(value1 | value2);

        return true;
    }
    
    private static Instruction ParseInstruction(this string line)
    {
        string[] sides = line.Split(" -> ");
        string outputWireName = sides[1];
        
        if (ushort.TryParse(sides[0], out ushort constValue))
            return Instruction.SetValue(line, constValue, outputWireName);

        if (sides[0].StartsWith("NOT"))
        {
            string inputWireName = sides[0].Split(' ')[1];
            return Instruction.Not(line, inputWireName, outputWireName);
        }
        
        string[] leftSide = sides[0].Split(' ');
        string inputWireName1 = leftSide[0];
        if (leftSide.Length == 1)
            return Instruction.StoreValue(line, inputWireName1, outputWireName);
        
        Action action = leftSide[1] switch
        {
            "AND" => Action.And,
            "OR" => Action.Or,
            "LSHIFT" => Action.LShift,
            "RSHIFT" => Action.RShift,
            _ => throw new InvalidOperationException()
        };
        string inputWireName2 = action switch
        {
            Action.And => leftSide[2],
            Action.Or => leftSide[2],
            _ => string.Empty
        };
        ushort leftShiftValue = action switch
        {
            Action.LShift or Action.RShift => ushort.Parse(leftSide[2]),
            _ => 0
        };
        
        return new Instruction(line, action, leftShiftValue, inputWireName1, inputWireName2, outputWireName);
    }

    private enum Action
    {
        SetValue,
        StoreValue,
        And,
        Or,
        LShift,
        RShift,
        Not
    }

    [DebuggerDisplay("{OriginalValue}")]
    private readonly struct Instruction
    {
        public readonly string OriginalValue;
        public readonly Action Action;
        public readonly ushort ConstValue;
        public readonly string InputWireName1;
        public readonly string InputWireName2;
        public readonly string OutputWireName;
        public readonly bool InputWire1IsValue;
        public readonly ushort InputWire1Value;

        public Instruction(string originalValue, Action action, ushort constValue, string inputWireName1, string inputWireName2,
            string outputWireName)
        {
            OriginalValue = originalValue;
            Action = action;
            ConstValue = constValue;
            InputWireName1 = inputWireName1;
            InputWireName2 = inputWireName2;
            OutputWireName = outputWireName;
            InputWire1IsValue = ushort.TryParse(inputWireName1, out ushort inputWire1Value);
            InputWire1Value = inputWire1Value;
        }

        public static Instruction SetValue(string originalValue, ushort constValue, string outputWireName)
        {
            return new Instruction(originalValue, Action.SetValue, constValue, string.Empty, string.Empty, outputWireName);
        }

        public static Instruction StoreValue(string originalValue, string inputWireName, string outputWireName)
        {
            return new Instruction(originalValue, Action.StoreValue, 0, inputWireName, string.Empty, outputWireName);
        }

        public static Instruction Not(string originalValue, string inputWireName, string outputWireName)
        {
            return new Instruction(originalValue, Action.Not, 0, inputWireName, string.Empty, outputWireName);
        }
    }
}