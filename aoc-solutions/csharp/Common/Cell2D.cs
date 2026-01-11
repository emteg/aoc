namespace Common;

public class Cell2D
{
    public readonly int X;
    public readonly int Y;
    public readonly (int x, int y) Location;
    public Cell2D? Up { get; private set; }
    public Cell2D? UpRight { get; private set; }
    public Cell2D? Right { get; private set; }
    public Cell2D? DownRight { get; private set; }
    public Cell2D? Down { get; private set; }
    public Cell2D? DownLeft { get; private set; }
    public Cell2D? Left { get; private set; }
    public Cell2D? UpLeft { get; private set; }

    /// <summary>
    /// Enumerates all non-null neighbors starting at 'up' and going clockwise
    /// </summary>
    public IEnumerable<Cell2D> Neighbors
    {
        get
        {
            if (Up is not null)
                yield return Up;
            if (UpRight is not null)
                yield return UpRight;
            if (Right is not null)
                yield return Right;
            if (DownRight is not null)
                yield return DownRight;
            if (Down is not null)
                yield return Down;
            if (DownLeft is not null)
                yield return DownLeft;
            if (Left is not null)
                yield return Left;
            if (UpLeft is not null)
                yield return UpLeft;
        }
    }
    
    public Cell2D() {}

    public Cell2D(int x, int y)
    {
        X = x;
        Y = y;
        Location = (x, y);
    }

    public void ConnectUp(Cell2D up, bool bothWays = true)
    {
        Up = up;
        if (bothWays)
            up.Down = this;
    }
    
    public void ConnectUpRight(Cell2D upRight, bool bothWays = true)
    {
        UpRight = upRight;
        if (bothWays)
            upRight.DownLeft = this;
    }
    
    public void ConnectRight(Cell2D right, bool bothWays = true)
    {
        Right = right;
        if (bothWays)
            right.Left = this;
    }
    
    public void ConnectDownRight(Cell2D downRight, bool bothWays = true)
    {
        DownRight = downRight;
        if (bothWays)
            downRight.UpLeft = this;
    }
    
    public void ConnectDown(Cell2D down, bool bothWays = true)
    {
        Down = down;
        if (bothWays)
            down.Up = this;
    }
    
    public void ConnectDownLeft(Cell2D downLeft, bool bothWays = true)
    {
        DownLeft = downLeft;
        if (bothWays)
            downLeft.UpRight = this;
    }
    
    public void ConnectLeft(Cell2D left, bool bothWays = true)
    {
        Left = left;
        if (bothWays)
            left.Right = this;
    }
    
    public void ConnectUpLeft(Cell2D upLeft, bool bothWays = true)
    {
        UpLeft = upLeft;
        if (bothWays)
            upLeft.DownRight = this;
    }

    /// <summary>
    /// Establishes a two-way connection for 4 neighbors when building a grid of cells. The cell must already be
    /// part of the grid. Uses the cell's X and Y to connect to the cell's left and up neighbor both ways. Cells in the
    /// middle of the grid will be connected with their 4 neighbors.
    /// </summary>
    /// <example>
    /// <code>
    /// List&lt;List&lt;TCell&gt;&gt; grid = [];
    /// int y = 0;
    /// foreach (string line in input)
    /// {
    ///     int x = 0;
    ///     grid.Add([]);
    /// 
    ///     foreach (char c in line)
    ///     {
    ///         TCell cell = ... // create a new cell
    ///         grid[y].Add(cell);
    ///         Cell2D.ConnectFour(cell, grid);
    ///         x++;
    ///     }
    ///     y++;
    /// }
    /// </code>
    /// </example>
    public static void ConnectFour<TCell>(TCell cell, List<List<TCell>> grid) where TCell : Cell2D
    {
        if (cell.X > 0)
            cell.ConnectLeft(grid[cell.Y][cell.X - 1]);
        if (cell.Y > 0)
            cell.ConnectUp(grid[cell.Y - 1][cell.X]);
    }   
    
    /// <summary>
    /// Establishes a two-way connection for 8 neighbors when building a grid of cells. The cell must already be
    /// part of the grid. Uses the cell's X and Y to connect to the cell's left, left-up, up, and up-right neighbor
    /// both ways. Cells in the middle of the grid will be connected with their 8 neighbors.
    /// </summary>
    /// <example>See description of ConnectFour and use this method instead</example>
    public static void ConnectEight<TCell>(TCell cell, List<List<TCell>> grid, int width) where TCell : Cell2D
    {
        ConnectFour(cell, grid);
        
        if (cell.X > 0 && cell.Y > 0)
            cell.ConnectUpLeft(grid[cell.Y - 1][cell.X - 1]);
        if (cell.Y > 0 && cell.X < width - 1)
            cell.ConnectUpRight(grid[cell.Y - 1][cell.X + 1]);
    }
    
    /// <summary>
    /// Enumerates the coordinates of all (4 or 8) hypothetical neighbors. Assumes a screen-coordinate
    /// system where X goes to the right, and Y goes to the bottom. The Y-Direction can be inverted.
    /// Order of neighbors: right, up-right, up, up-left, left, down-left, down, down-right
    /// </summary>
    public IEnumerable<(int x, int y)> NeighborCoordinates(bool eightNeighbors = false, bool invertY = false)
    {
        int invert = invertY ? -1 : 1;
        yield return (X + 1, Y);
        
        if (eightNeighbors)
            yield return (X + 1, Y - 1 * invert);
        
        yield return (X, Y - 1 * invert);
        
        if (eightNeighbors)
            yield return (X - 1, Y - 1 * invert);
        
        yield return (X - 1, Y);
        
        if (eightNeighbors)
            yield return (X - 1, Y + 1 * invert);
        
        yield return (X, Y + 1 * invert);
        
        if (eightNeighbors)
            yield return (X + 1, Y + 1 * invert);
    }

    public (int deltaX, int deltaY) DistanceTo(Cell2D other) => (other.X - X, other.Y - Y);
}