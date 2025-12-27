using Common;

namespace _2020;

public static class Day23
{
    public static string Part1(IEnumerable<string> input)
    {
        string line = input.First();
        Cup currentCup = PlaceCups(line);

        for (int move = 1; move < 101; move++)
        {
            Console.Error.WriteLine($"-- move {move} --");
            Console.Error.WriteLine($"cups: {PrintCups(currentCup)}");
            
            Cup pickedUp = currentCup.PickUpThree();
            Console.Error.WriteLine($"pick up: {PrintCups(pickedUp)}");
            
            int destination = currentCup.Value - 1;
            int lowest = currentCup.Lowest();
            Cup? destCup = null;
            while (destCup is null)
            {
                destCup = currentCup.Find(destination);
                
                if (destCup is not null && destCup != currentCup)
                    break;
                
                destination--;

                if (destination < lowest) 
                    destination = currentCup.Highest();
            }
            Console.Error.WriteLine($"destination: {destCup.Value}");
            destCup.InsertThree(pickedUp);
            currentCup.IsCurrentCup = false;
            currentCup = currentCup.Next!;
            currentCup.IsCurrentCup = true;
            Console.Error.WriteLine();
        }
        
        Console.Error.WriteLine($"cups: {PrintCups(currentCup)}");
        Cup one = currentCup.Find(1)!;
        string result = OrderString(one)[1..];
        return result;
    }

    private static string PrintCups(Cup start)
    {
        List<Cup> cups = [start];
        Cup? current = start.Next;
        while (current != start && current is not null)
        {
            cups.Add(current);
            current = current.Next;
        }
        return string.Join(" ", cups);
    }

    private static string OrderString(Cup start)
    {
        List<Cup> cups = [start];
        Cup? current = start.Next;
        while (current != start && current is not null)
        {
            cups.Add(current);
            current = current.Next;
        }
        return string.Join(string.Empty, cups.Select(it => it.Value));
    }

    private static Cup PlaceCups(string line)
    {
        Cup? firstCup = null;
        Cup? lastCup = null;
        
        foreach (char c in line)
        {
            int value = c - 48;
            
            if (firstCup is null)
            {
                firstCup = new Cup(value);
                lastCup = firstCup;
                continue;
            }
            
            Cup newCup = new(value, lastCup);
            lastCup = newCup;
        }
        lastCup!.Next = firstCup!;
        
        return firstCup!;
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        return string.Empty;
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());

    private sealed class Cup
    {
        public int Value { get; }
        public Cup? Next { get; set; }
        public bool IsCurrentCup { get; set; }

        public Cup(int value, Cup? previous = null)
        {
            Value = value;
            if (previous is null)
                IsCurrentCup = true;
            else
                previous.Next = this;

            Next = null!;
        }

        public Cup PickUpThree()
        {
            Cup start = Next!;
            Cup end = start.Next!.Next!;
            Next = end.Next;
            end.Next = null!;

            return start;
        }

        public void InsertThree(Cup firstOfThree)
        {
            Cup oldNext = Next!;
            Next = firstOfThree;
            firstOfThree.Next!.Next!.Next = oldNext;
        }

        public int Lowest()
        {
            int result = int.MaxValue;
            Cup current = this;
            
            do
            {
                if (current.Value < result)
                    result = current.Value;
                current = current.Next!;
            } while (current != this);
            
            return result;
        }

        public int Highest()
        {
            int result = int.MinValue;
            Cup current = this;

            do
            {
                if (current.Value > result)
                    result = current.Value;
                current = current.Next!;
            } while (current != this);
            
            return result;
        }

        public Cup? Find(int value)
        {
            Cup current = this;

            do
            {
                if (current.Value == value)
                    return current;
                current = current.Next!;
            } while (current != this);

            return null;
        }

        public override string ToString()
        {
            return IsCurrentCup
                ? $"({Value})"
                : Value.ToString();
        }
    }
    
    private const string Sample = "389125467";
}