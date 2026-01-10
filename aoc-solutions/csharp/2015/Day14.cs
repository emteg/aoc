using System.Diagnostics;
using Common;

namespace _2015;

public static class Day14
{
    public static string Part1(IEnumerable<string> input) => Part1(input, 2503).ToString();

    public static string Part1Sample() => Part1(Sample.Lines(), 1000).ToString();

    private static int Part1(IEnumerable<string> input, int raceDurationSeconds)
    {
        List<Reindeer> reindeers = input.Select(line => line.ToReindeer()).ToList();
        foreach (Reindeer reindeer in reindeers)
            for (int i = 0; i < raceDurationSeconds; i++) 
                reindeer.Update();
        
        Reindeer best = reindeers.First();
        foreach (Reindeer reindeer in reindeers)
        {
            if (reindeer.DistanceTravelled > best.DistanceTravelled)
                best = reindeer;
        }

        return best.DistanceTravelled;
    }

    public static string Part2(IEnumerable<string> input) => Part2(input, 2503).ToString();
    
    public static string Part2Sample() => Part2(Sample.Lines(), 1000).ToString();
    
    private static int Part2(IEnumerable<string> inputLines, int raceDurationSeconds)
    {
        List<Reindeer> reindeers = inputLines.Select(line => line.ToReindeer()).ToList();
        int bestDistance = 0;
        
        for (int i = 0; i < raceDurationSeconds; i++)
        {
            foreach (Reindeer reindeer in reindeers)
            {
                reindeer.Update();
                if (reindeer.DistanceTravelled > bestDistance) 
                    bestDistance = reindeer.DistanceTravelled;
            }
            
            foreach (Reindeer reindeer in reindeers.Where(reindeer => reindeer.DistanceTravelled == bestDistance))
                reindeer.AwardPoint();
        }
        
        Reindeer best = reindeers.First();
        foreach (Reindeer reindeer in reindeers)
        {
            if (reindeer.Points > best.Points)
                best = reindeer;
        }

        return best.Points;
    }
    
    private const string Sample = """
                                  Comet can fly 14 km/s for 10 seconds, but then must rest for 127 seconds.
                                  Dancer can fly 16 km/s for 11 seconds, but then must rest for 162 seconds.
                                  """;
    
    private static Reindeer ToReindeer(this string line)
    {
        string[] words = line.Split(" ");
        string name = words[0];
        int speed = int.Parse(words[3]);
        int flightDuration = int.Parse(words[6]);
        int restDuration = int.Parse(words[13]);
        
        return new Reindeer(name, speed, flightDuration, restDuration);
    }
    
    [DebuggerDisplay("{Name} {DistanceTravelled}")]
    private sealed class Reindeer
    {
        public readonly string Name;
        public readonly int SpeedKmPerSecond;
        public readonly int FlightDuration;
        public readonly int RestDuration;
        public int DistanceTravelled { get; private set; }
        public int Points { get; private set; }

        public Reindeer(string name, int speedKmPerSecond, int flightDuration, int restDuration)
        {
            Name = name;
            SpeedKmPerSecond = speedKmPerSecond;
            FlightDuration = flightDuration;
            RestDuration = restDuration;
            counter = FlightDuration;
        }

        public void Update()
        {
            counter--;

            if (isFlying)
                DistanceTravelled += SpeedKmPerSecond;
        
            if (counter > 0) 
                return;
        
            counter = isFlying ? RestDuration : FlightDuration;
            isFlying = !isFlying;
        }

        public void AwardPoint() => Points++;

        private bool isFlying = true;
        private int counter;
    }
}