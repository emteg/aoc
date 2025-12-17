using Common;

namespace _2025;

public static class Day08
{
    static Day08()
    {
        //PuzzleRegistry.Add(2025, 8, 1, Part1);
        //PuzzleRegistry.Add(2025, 8, 2, Part2);
    }
    
    public static string Part1(IEnumerable<string> input)
    {
        List<Point3D> points = input.Select(ToPoint).ToList();
        
        Point3D? a = null;
        Point3D? b = null;
        double shortestDistance = double.MaxValue;
        
        for (int i = 0; i < points.Count - 1; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                double newDist = points[i].DistanceTo(points[j]);
                if (newDist > shortestDistance)
                    continue;

                if (a is null) 
                    a = points[i];
                
                b = points[j];
                shortestDistance = newDist;
            }
        }

        points.Remove(a!);
        points.Remove(b!);
        return string.Empty;
    }
    
    
    
    public static string Part1Sample()
    {
        const string sample = """
            162,817,812
            57,618,57
            906,360,560
            592,479,940
            352,342,300
            466,668,158
            542,29,236
            431,825,988
            739,650,466
            52,470,668
            216,146,977
            819,987,18
            117,168,530
            805,96,715
            346,949,466
            970,615,88
            941,993,340
            862,61,35
            984,92,344
            425,690,689
            """;
        return Part1(sample.Lines());
    }

    private static Point3D ToPoint(string line)
    {
        int[] values = line.Split(',').Select(int.Parse).ToArray();
        return new Point3D(values[0], values[1], values[2]);
    }
}

