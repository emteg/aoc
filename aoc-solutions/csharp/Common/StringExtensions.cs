namespace Common;

public static class StringExtensions
{
    extension(string s)
    {
        public IEnumerable<char> CharsAtEvenIndices() => s.Where((_, i) => i % 2 == 0);
        public IEnumerable<char> CharsAddOddIndices() => s.Where((_, i) => i % 2 != 0);
    }

    public static string AsString(this IEnumerable<char> chars) => new(chars.ToArray());

    public static IEnumerable<string> Lines(this string s)
    {
        TextReader reader = new StringReader(s);
        while (true)
        {
            string? line = reader.ReadLine();
            if (line is null)
                yield break;
            yield return line;
        }
    }
    
    public static bool IsDigit(this char c) => c is >= '0' and <= '9';
}