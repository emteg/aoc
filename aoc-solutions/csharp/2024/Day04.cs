using Common;

namespace _2024;

public static class Day04
{
    public static string Part1(IEnumerable<string> input)
    {
        uint horizontalMatches = 0;
        uint verticalMatches = 0;
        uint topLeftDiagonalMatches = 0;
        uint topRightDiagonalMatches = 0;
        List<string> window = [];
        
        foreach (string line in input)
        {
            window.Add(line);
            if (window.Count > 4)
                window.RemoveAt(0);
            
            horizontalMatches += CountXmas(line);
            
            if (window.Count != 4) 
                continue;
            
            foreach (string verticalLine in GetVerticalLines(window)) 
                verticalMatches += CountXmas(verticalLine);

            foreach (string topLeftDiagonalLine in GetTopLeftDiagonalLines(window)) 
                topLeftDiagonalMatches += CountXmas(topLeftDiagonalLine);
                
            foreach (string topRightDiagonalLine in GetTopRightDiagonalLines(window)) 
                topLeftDiagonalMatches += CountXmas(topRightDiagonalLine);
        }
        
        uint totalMatches = horizontalMatches + verticalMatches + topLeftDiagonalMatches + topRightDiagonalMatches;
        return totalMatches.ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    private static IEnumerable<string> GetTopRightDiagonalLines(List<string> lines)
    {
        for (int x = 0; x <= lines[0].Length -4; x++)
        {
            string s = new([lines[0][x + 3], lines[1][x + 2], lines[2][x + 1], lines[3][x]]);
            yield return s;
        }
    }

    private static IEnumerable<string> GetTopLeftDiagonalLines(List<string> lines)
    {
        for (int x = 0; x <= lines[0].Length -4; x++)
        {
            string s = new([lines[0][x], lines[1][x + 1], lines[2][x + 2], lines[3][x + 3]]);
            yield return s;
        }
    }

    private static IEnumerable<string> GetVerticalLines(List<string> lines)
    {
        for (int x = 0; x < lines[0].Length; x++)
        {
            string s = new([lines[0][x], lines[1][x], lines[2][x], lines[3][x]]);
            yield return s;
        }
    }

    private static uint CountXmas(string line)
    {
        uint result = 0;
        
        for (int i = 0; i <= line.Length - 4; i++)
        {
            string s = line.Substring(i, 4);
            if (s is "XMAS" or "SAMX") 
                result++;
        }

        return result;
    }
    
    public static string Part2(IEnumerable<string> input)
    {
        uint result = 0;
        List<string> window = [];
        
        foreach (string line in input)
        {
            window.Add(line);
            if (window.Count > 3)
                window.RemoveAt(0);

            if (window.Count == 3)
                result += CountMasX(window);
        }
        
        return result.ToString();
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private static uint CountMasX(List<string> lines)
    {
        uint result = 0;
        
        for (int x = 0; x <= lines[0].Length - 3; x++)
        {
            string diagonal1 = new([lines[0][x],     lines[1][x + 1], lines[2][x + 2]]);
            string diagonal2 = new([lines[0][x + 2], lines[1][x + 1], lines[2][x]]);
            if ((diagonal1 == "MAS" || diagonal1 == "SAM") && (diagonal2 == "MAS" || diagonal2 == "SAM"))
                result++;
        }
        
        return result;
    }
    
    private const string Sample = """
                                  MMMSXXMASM
                                  MSAMXMSMSA
                                  AMXSXMAAMM
                                  MSAMASMSMX
                                  XMASAMXAMM
                                  XXAMMXXAMA
                                  SMSMSASXSS
                                  SAXAMASAAA
                                  MAMMMXMMMM
                                  MXMXAXMASX
                                  """;
}