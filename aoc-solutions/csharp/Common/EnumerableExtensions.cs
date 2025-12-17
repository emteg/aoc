namespace Common;

public static class EnumerableExtensions
{
    public static ulong Sum(this IEnumerable<ulong> enumerable)
    {
        return enumerable.Aggregate<ulong, ulong>(0, (current, value) => current + value);
    }

    public static uint Sum(this IEnumerable<uint> enumerable)
    {
        return enumerable.Aggregate<uint, uint>(0, (current, value) => current + value);
    }

    public static ushort Sum(this IEnumerable<ushort> enumerable)
    {
        return enumerable.Aggregate<ushort, ushort>(0, (current, value) => (ushort)(current + value));
    }
}