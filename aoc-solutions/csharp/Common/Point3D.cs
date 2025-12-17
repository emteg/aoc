namespace Common;

public class Point3D : IEquatable<Point3D>
{
    public readonly int X;
    public readonly int Y;
    public readonly int Z;
    
    public Point3D(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public double DistanceTo(Point3D other) => Math.Sqrt(
        Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2) + Math.Pow(Z - other.Z, 2));

    public override string ToString() => $"({X},{Y},{Z})";

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public bool Equals(Point3D? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Point3D)obj);
    }
}