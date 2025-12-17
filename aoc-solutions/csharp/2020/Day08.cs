using Common;

namespace _2020;

public static class Day08
{
    public static string Part1(IEnumerable<string> input)
    {
        GameConsole console = new(input.Select(Instruction.Parse));

        console.Boot();
        
        return console.Accumulator.ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        GameConsole console = new(input.Select(Instruction.Parse));
        console.Boot();

        Queue<int> possiblyCorruptedInstructionIndices = [];
        for (int i = 0; i < console.Instructions.Count; i++)
        {
            if (console.Instructions[i].TimesExecuted > 0 && console.Instructions[i].MayHaveBeenCorrupted)
                possiblyCorruptedInstructionIndices.Enqueue(i);
        }
        
        while (possiblyCorruptedInstructionIndices.Count > 0)
        {
            int index = possiblyCorruptedInstructionIndices.Dequeue();
            console.Reset();
            console.Uncorrupt(index);
            console.Boot();

            if (console.TerminatedDueToEndOfBoot)
                break;
        }

        return console.Accumulator.ToString();
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private const string Sample = """
        nop +0
        acc +1
        jmp +4
        acc +3
        jmp -3
        acc -99
        acc +1
        jmp -4
        acc +6
        """;

    private sealed class GameConsole
    {
        public int Accumulator { get; private set; }
        public uint InstructionPointer { get; private set; }
        public IReadOnlyList<Instruction> Instructions => instructions;
        public bool TerminatedDueToInfiniteLoop { get; private set; }
        public bool TerminatedDueToEndOfBoot { get; private set; }

        public GameConsole(IEnumerable<Instruction> instructions)
        {
            this.instructions = instructions.ToArray();
        }

        public void Uncorrupt(int instructionIndex)
        {
            instructions[instructionIndex] = Instructions[instructionIndex].Uncorrupt();
        }

        public void Boot()
        {
            while (Next())
                ;
        }

        public void Reset()
        {
            Accumulator = 0;
            InstructionPointer = 0;
            
            for (int i = 0; i < instructions.Length; i++) 
                instructions[i] = instructions[i].Reset();

            TerminatedDueToInfiniteLoop = false;
            TerminatedDueToEndOfBoot = false;
        }

        private bool Next()
        {
            if (InstructionPointer >= instructions.Length)
            {
                TerminatedDueToEndOfBoot = true;
                return false;
            }
            
            Instruction instruction = Instructions[(int)InstructionPointer];

            if (instruction.TimesExecuted > 0)
            {
                TerminatedDueToInfiniteLoop = true;
                return false;
            }

            (InstructionPointer, Accumulator, instructions[(int)InstructionPointer]) = instruction.Type switch
            {
                InstructionType.Acc => (InstructionPointer +1, Accumulator + instruction.Value, instruction.HasBeenExecuted()),
                InstructionType.Jmp => (Jump(instruction),     Accumulator,                     instruction.HasBeenExecuted()),
                _ =>                   (InstructionPointer +1, Accumulator,                     instruction.HasBeenExecuted()),
            };

            return true;
        }

        private uint Jump(Instruction instruction)
        {
            if (instruction.Type is not InstructionType.Jmp)
                throw new InvalidOperationException();

            return (uint)(InstructionPointer + instruction.Value);
        }

        private readonly Instruction[] instructions;
    }

    private struct Instruction
    {
        public InstructionType Type { get; private set; }
        public readonly int Value;
        public uint TimesExecuted { get; private set; }
        public bool MayHaveBeenCorrupted => Type is InstructionType.Nop or InstructionType.Jmp;

        private Instruction(InstructionType type, int value)
        {
            Type = type;
            originalType = type;
            Value = value;
            TimesExecuted = 0;
        }

        public static Instruction Parse(string line)
        {
            string[] typeAndValue = line.Split(' ');
            InstructionType type = Enum.Parse<InstructionType>(typeAndValue[0], true);
            typeAndValue[1] = typeAndValue[1].Contains('+')
                ? typeAndValue[1][1..]
                : typeAndValue[1];
            int value = int.Parse(typeAndValue[1]);
            
            return new Instruction(type, value);
        }
        
        public Instruction HasBeenExecuted()
        {
            TimesExecuted++;
            return this;
        }

        public Instruction Uncorrupt()
        {
            Type = Type switch
            {
                InstructionType.Nop => InstructionType.Jmp,
                InstructionType.Jmp => InstructionType.Nop,
                _ => Type
            };

            return this;
        }

        public Instruction Reset()
        {
            TimesExecuted = 0;
            Type = originalType;
            return this;
        }

        public override string ToString() => $"{Type} {Value} [{TimesExecuted}]";

        private readonly InstructionType originalType;
    }

    private enum InstructionType
    {
        Nop,
        Acc,
        Jmp
    }
}