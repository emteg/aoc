using Common;

namespace _2015;

public static class Day03
{
    public static string Part1(IEnumerable<string> input) 
    {
        Dictionary<(int x, int y), Cell2D> houses = [];
        Cell2D santa = new(0, 0);
        houses.Add((0 ,0), santa);
        
        DeliverPresents(input.First(), santa, houses);

        return houses.Count.ToString();
    }
    
    public static string Part2(IEnumerable<string> input) 
    {
        Dictionary<(int x, int y), Cell2D> houses = [];
        Cell2D santa = new(0, 0);
        houses.Add((0 ,0), santa);
        Cell2D roboSanta = santa;
        
        DeliverPresents(input.First().CharsAtEvenIndices().AsString(), santa, houses);
        DeliverPresents(input.First().CharsAddOddIndices().AsString(), roboSanta, houses);

        return houses.Count.ToString();
    }
    
    private static void DeliverPresents(string input, Cell2D currentHouse, Dictionary<(int x, int y), Cell2D> houses)
    {
        foreach (char c in input)
        {
            (Cell2D? neighbor, Action<Cell2D> connect, int x, int y) = CheckInput(currentHouse, c);

            if (neighbor is null) 
                houses.TryGetValue((x, y), out neighbor);
            
            if (neighbor is null)
            {
                neighbor = new Cell2D(x, y);
                houses.Add((x, y), neighbor);
                connect(neighbor);
            }

            currentHouse = neighbor;
        }
    }
    
    private static (Cell2D? neighbor, Action<Cell2D> connect, int x, int y) CheckInput(
        Cell2D currentHouse, char c)
    {
        return c switch
        {
            '^' => (currentHouse.Up,    cell => currentHouse.ConnectUp(cell),    currentHouse.X,     currentHouse.Y - 1),
            '>' => (currentHouse.Right, cell => currentHouse.ConnectRight(cell), currentHouse.X + 1, currentHouse.Y),
            'v' => (currentHouse.Down,  cell => currentHouse.ConnectDown(cell),  currentHouse.X,     currentHouse.Y + 1),
            '<' => (currentHouse.Left,  cell => currentHouse.ConnectLeft(cell),  currentHouse.X - 1, currentHouse.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, "Invalid direction!")
        };
    }
}