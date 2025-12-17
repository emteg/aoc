using Common;

namespace _2020;

public static class Day05
{
    public static string Part1(IEnumerable<string> input)
    {
        int highestSeatId = int.MinValue;
        
        foreach (string line in input)
        {
            int id = ParseSeatId(line);
            if (id > highestSeatId)
                highestSeatId = id;
        }
        
        return highestSeatId.ToString();
    }

    public static string Part2(IEnumerable<string> input)
    {
        byte minRow = byte.MaxValue;
        byte maxRow = byte.MinValue;

        List<int> actualSeatIds = [];
        foreach ((byte row, byte col) in input.Select(ParseSeatRowAndCol))
        {
            if (row > maxRow)
                maxRow = row;
            if (row < minRow)
                minRow = row;
            
            actualSeatIds.Add(CalculateSeatId(row, col));
        }

        List<int> allExpectedSeatIds = [];
        for (byte row = minRow; row < maxRow; row++) // anything before/after minRow/maxRow doesnt seem to exist
        {
            for (byte col = 0; col < 8; col++)
            {
                if (row == minRow && col < 4) // in minRow, the first 3 seats dont seem to exist
                    continue;
                allExpectedSeatIds.Add(CalculateSeatId(row, col));
            }
        }
        
        foreach (int seatId in allExpectedSeatIds)
        {
            if (!actualSeatIds.Contains(seatId))
                return seatId.ToString(); // the only missing expected seat id
        }

        throw new InvalidOperationException("Seat not found"); // shouldn't happen
    }

    private static int ParseSeatId(string line)
    {
        (byte row, byte col) = ParseSeatRowAndCol(line);
        int seatId = CalculateSeatId(row, col);
        return seatId;
    }

    private static int CalculateSeatId(byte row, byte col)
    {
        return (row << 3) + col;
    }

    private static (byte row, byte col) ParseSeatRowAndCol(string line)
    {
        byte row = Parse(line[..7], 128, 'F', 'B');
        byte col = Parse(line[7..],   8, 'L', 'R');
        return (row, col);
    }

    private static byte Parse(string input, byte size, char goLower, char goUpper)
    {
        byte upper = (byte)(size -1);
        byte lower = 0;
        foreach (char c in input)
        {
            size >>= 1;
            if (c == goLower)
                upper -= size;
            else if (c == goUpper)
                lower += size;
        }
        return lower;
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    private const string Sample = """
        FBFBBFFRLR
        BFFFBBFRRR
        FFFBBBFRRR
        BBFFBBFRLL
        """;
}