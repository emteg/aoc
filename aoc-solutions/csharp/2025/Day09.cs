using System.Drawing;
using Common;

namespace _2025;

public static class Day09
{
    public static string Part1(IEnumerable<string> input)
    {
        Point[] points = ReadPoints(input);

        long maxArea = 0;
        for (int i = 0; i < points.Length - 1; i++)
        {
            Point a = points[i];
            for (int j = i + 1; j < points.Length; j++)
            {
                Point b = points[j];
                Rectangle rect = Rectangle.New(a, b);
                long area = rect.Area;
                if (area > maxArea) 
                    maxArea = area;
            }
        }

        return maxArea.ToString();
    }

    public static string Part2(IEnumerable<string> input)
    {
        Point[] points = ReadPoints(input);
        
        return string.Empty;
    }

    private static Point[] ReadPoints(IEnumerable<string> input)
    {
        Point[] points = input
            .Select(line => line.Split(',').Select(int.Parse).ToArray())
            .Select(ints => new Point(ints[0], ints[1]))
            .ToArray();
        return points;
    }

    public static string Part1Sample()
    {
        return Part1(Sample.Lines());
    }

    public static string Part2Sample()
    {
        return Part2(Sample.Lines());
    }
    
    private const string Sample = """
                          7,1
                          11,1
                          11,7
                          9,7
                          9,5
                          2,5
                          2,3
                          7,3
                          """;
    
    extension(Rectangle rect)
    {
        private long Area => (long)(rect.Size.Height + 1) * (rect.Size.Width + 1);

        private static Rectangle New(Point a, Point b)
        {
            Size size = new(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
            return new Rectangle(a, size);
        }
    }
}