using Common;

namespace _2024;

public static class Day13
{
    public static string Part1(IEnumerable<string> input)
    {
        return Execute(input).ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        return Execute(input, 10000000000000).ToString();
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private const string Sample = """
                                  Button A: X+94, Y+34
                                  Button B: X+22, Y+67
                                  Prize: X=8400, Y=5400
                                  
                                  Button A: X+26, Y+66
                                  Button B: X+67, Y+21
                                  Prize: X=12748, Y=12176
                                  
                                  Button A: X+17, Y+86
                                  Button B: X+84, Y+37
                                  Prize: X=7870, Y=6450
                                  
                                  Button A: X+69, Y+23
                                  Button B: X+27, Y+71
                                  Prize: X=18641, Y=10279
                                  """;
    
    private static long Execute(IEnumerable<string> input, long addToPrizeLocation = 0)
    {
        List<ClawMachine> clawMachines = ReadClawMachines(input, addToPrizeLocation);
        long tokensSpent = 0;
        foreach (ClawMachine machine in clawMachines)
        {
            (long aPresses, long bPresses) = machine.GetButtonPresses();
            if (aPresses == 0 && bPresses == 0)
                continue;
            
            tokensSpent += 3 * aPresses + bPresses;
        }
        return tokensSpent;
    }

    private static List<ClawMachine> ReadClawMachines(IEnumerable<string> input, long addToPrizeLocation)
    {
        List<ClawMachine> clawMachines = [];
        (long x, long y) a = (0, 0);
        (long x, long y) b = (0, 0);
        int counter = 0;
        foreach (string line in input)
        {
            if (line.Length == 0)
                continue;
            
            string[] values = line.Split(": ")[1].Split(", ");
            if (counter == 0)
            {
                a = (long.Parse(values[0][2..]), long.Parse(values[1][2..]));
                counter++;
                continue;
            }

            if (counter == 1)
            {
                b = (long.Parse(values[0][2..]), long.Parse(values[1][2..]));
                counter++;
                continue;
            }
            
            counter = 0;
            (long x, long y) prize = (long.Parse(values[0][2..]) + addToPrizeLocation, long.Parse(values[1][2..]) + addToPrizeLocation);
            clawMachines.Add(new ClawMachine(a, b, prize));
        }

        return clawMachines;
    }
    
    private sealed class ClawMachine
    {
        public readonly (long X, long Y) Prize;
        public readonly (long X, long Y) ButtonA;
        public readonly (long X, long Y) ButtonB;

        public ClawMachine((long x, long y) a, (long x, long y) b, (long x, long y) prize)
        {
            Prize = prize;
            ButtonA = a;
            ButtonB = b;
        }

        public (long aPresses, long bPresses) GetButtonPresses()
        {
            // a: number of times a button is pressed; b: number of times b button is pressed
            //
            // 1) a * ButtonA.X + b * ButtonB.X = Prize.X
            // 2) a * ButtonA.Y + b * ButtonB.Y = Prize.Y
            // 
            // Multiply 1) by ButtonB.Y and 2) by ButtonB.X to allow elimination of B
            //
            // 3) a * ButtonA.X * ButtonB.Y + b * ButtonB.X * ButtonB.Y = ButtonB.Y * Prize.X
            // 4) a * ButtonA.Y * ButtonB.X + b * ButtonB.Y * ButtonB.X = ButtonB.X * Prize.Y
            //
            // Subtract 4) from 3) to get 5) (and eliminate b)
            //
            // 3) - 4) = 5) a * (ButtonA.X * ButtonB.Y - ButtonA.Y * ButtonB.X) = ButtonB.Y * Prize.X - ButtonB.X * Prize.Y
            //
            // Solve 5) for a
            //        ButtonB.Y * Prize.X - ButtonB.X * Prize.Y
            // a = -----------------------------------------------
            //     (ButtonA.X * ButtonB.Y - ButtonA.Y * ButtonB.X)

            long aNumerator = ButtonB.Y * Prize.X - ButtonB.X * Prize.Y;
            long aDenominator = ButtonA.X * ButtonB.Y - ButtonA.Y * ButtonB.X;

            if (aNumerator % aDenominator != 0)
                return (0, 0);

            long a = aNumerator / aDenominator;

            // Insert a into 2) to solve for b
            //
            // 2) a * ButtonA.Y + b * ButtonB.Y = Prize.Y
            //    b * ButtonB.Y = Prize.Y - a * ButtonA.Y
            //         Prize.Y - a * ButtonA.Y
            //    b = -------------------------
            //               ButtonB.Y

            long bNumerator = Prize.Y - a * ButtonA.Y;

            if (bNumerator % ButtonB.Y != 0)
                return (0, 0);

            long b = bNumerator / ButtonB.Y;

            return (a, b);
        }
    }
}