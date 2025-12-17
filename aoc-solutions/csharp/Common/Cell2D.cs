namespace Common;

public class Cell2D
{
    public readonly int X;
    public readonly int Y;
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
}