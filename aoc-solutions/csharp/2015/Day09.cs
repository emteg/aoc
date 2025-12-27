using System.Diagnostics;
using Common;

namespace _2015;

public static class Day09
{
    public static string Part1(IEnumerable<string> input)
    {
        Route result = Execute(input);
        return result.TotalDistance.ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());
    
    public static string Part2(IEnumerable<string> input)
    {
        Route result = Execute(input, false);
        return result.TotalDistance.ToString();
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private static Route Execute(IEnumerable<string> lines, bool returnShortest = true)
    {
        List<Destination> destinations = ParseInput(lines).Values.ToList();
        List<Route> routes = [];
        
        foreach (Destination start in destinations)
        {
            Route route = new(start);
            FindRoutesFrom(route, destinations.Where(destination => destination != start).ToList(), routes);
        }

        return returnShortest 
            ? routes.OrderBy(it => it.TotalDistance).First() 
            : routes.OrderByDescending(it => it.TotalDistance).First();
    }
    
    private static void FindRoutesFrom(Route route, List<Destination> remainingDestinations, List<Route> routes)
    {
        if (remainingDestinations.Count == 0)
        {
            routes.Add(route);
            return;
        }

        foreach (Destination dest in remainingDestinations)
        {
            if (route.Destinations.Contains(dest))
                continue;
            if (!route.Last.IsConnectedTo(dest))
                continue;
            
            Route newRoute = route.Clone();
            newRoute.Append(dest);
            FindRoutesFrom(newRoute, remainingDestinations.Where(destination => destination != dest).ToList(), routes);
        }
    }
    
    private static Dictionary<string, Destination> ParseInput(IEnumerable<string> lines)
    {
        Dictionary<string, Destination> cities = [];
        foreach (string line in lines) 
            line.CreateConnection(cities);
        return cities;
    }
    
    private static void CreateConnection(this string line, Dictionary<string, Destination> cities)
    {
        string[] sides = line.Split(" = ");
        uint distance = uint.Parse(sides[1]);
        string[] cityNames = sides[0].Split(" to ");

        if (!cities.TryGetValue(cityNames[0], out Destination? cityA))
        {
            cityA = new Destination(cityNames[0]);
            cities[cityA.Name] = cityA;
        }
        
        if (!cities.TryGetValue(cityNames[1], out Destination? cityB))
        {
            cityB = new Destination(cityNames[1]);
            cities[cityB.Name] = cityB;
        }
        
        cityA.ConnectTo(cityB, distance);
    }
    
    private sealed class Route
    {
        public readonly Stack<Destination> Destinations = [];
        public uint TotalDistance { get; private set; }
        public Destination Last => Destinations.Peek();
    
        public Route(Destination origin)
        {
            Destinations.Push(origin);
        }

        private Route(IEnumerable<Destination> destinations, uint totalDistance)
        {
            Destinations = new Stack<Destination>(destinations.Reverse());
            TotalDistance = totalDistance;
        }

        public void Append(Destination newDestination)
        {
            if (Destinations.Contains(newDestination))
                throw new InvalidOperationException("Route already contains this destination");

            if (!Last.IsConnectedTo(newDestination))
                throw new InvalidOperationException("Last is not connected to the new destination");
        
            TotalDistance += Last.DistanceTo(newDestination);
            Destinations.Push(newDestination);
        }

        public override string ToString()
        {
            string cities = string.Join(" -> ", Destinations.Select(it => it.Name).Reverse());
            return $"{cities} = {TotalDistance}";
        }

        public Route Clone() => new(Destinations, TotalDistance);
    }

    [DebuggerDisplay("{Name}")]
    private sealed class Destination
    {
        public readonly string Name;
        public readonly Dictionary<Destination, uint> Distances = [];

        public Destination(string name)
        {
            Name = name;
        }

        public void ConnectTo(Destination destination, uint distance)
        {
            Distances[destination] = distance;
            if (!destination.Distances.ContainsKey(this))
                destination.ConnectTo(this, distance);
        }

        public bool IsConnectedTo(Destination other) => Distances.ContainsKey(other);
    
        public uint DistanceTo(Destination other)
        {
            return Distances.TryGetValue(other, out uint value) 
                ? value 
                : 0;
        }
    }
    
    private const string Sample = """
                                  London to Dublin = 464
                                  London to Belfast = 518
                                  Dublin to Belfast = 141
                                  """;
}