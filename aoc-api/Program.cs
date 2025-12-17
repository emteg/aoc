using System.Diagnostics.CodeAnalysis;

namespace AocApi;

internal static class Program
{
    private const string HelpText = """
        An API to for Advent of Code which allows you to:
        - fetch (and cache) puzzle inputs
        - submit (and cache) puzzle answers
        - check your status on which puzzles you have already solved
        
        The CLI uses the Aoc Website using your personal session cookie that you can
        extract from your browser after you're logged in.
        
        The following positional arguments are available:
        
        help
            Shows this help. Also shown if no arguments are passed. Exist code always 0.
        
        cookie <session cookie>
            Sets and stores the given session cookie in a file, overwriting an existing
            value.
            
            Exit code 0 if the cookie was stored, -1 on any error
        
        fetch <year> <day> [<part>]
            Downloads the puzzle input for the given puzzle. The input is stored as
            /<year>/<day>/input<part>.txt. Returns the contents of the file on
            Standard Out.
            
            Year must be an unsigned short. Day and part must be an unsigned byte.
            
            When asked again for the same puzzle input, the contents of the file
            are returned without sending another request to AOC.
            
            Exit code 0 if no error occurred, -1 on any error.
        
        submit <year> <day> <part> [<answer>] [--offline]
            Submits the given answer for the given puzzle to AOC for validation.
            The answer can be supplied as an optional 5th argument, or via Standard
            Input.
            
            If the optional --offline flag is provied, the given answer is stored into
            a file without making a request to AOC and assumed correct. Use this if you
            have previously solved this puzzle and you know that this is the correct
            answer. Will fail if the answer file for this puzzle already exists.
            
            Year must be an unsigned short. Day and part must be an unsigned byte.
            
            If the answer is correct, the value is stored as
            /<year>/<day>/answer<answer>.txt
            
            If the answer to the given puzzle is already stored, the stored value is
            compared for equality without making another request to AOC.
            
            Exist code 0 if the answer is correct, 1 if the answer is wrong, -1 on
            any error
            
        status [<year>]
            Writes a list of how many Gold Stars you have already received (based on the
            stored answers).
            
            If you provide a year, you get a list of all partially solved puzzles for
            the given year and how many Gold Stars that is in per puzzle and in total.
            
            If you dont provide a year, you get a list of all locally stored years and
            how many Gold Stars you have received for each year.
            
            Year must be an unsigned short.
            
            Writes the list to Standard Out.
            
            Exit code 0 if no error occurred, -1 on any error.
        """;
    
    private static int Main(string[] args)
    {
        try
        {
            return args.AsSpan() switch
            {
                [] => Help(true),
                ["help", ..] => Help(),
                ["cookie", var cookie, ..] => Cookie(cookie),
                ["fetch", var yearStr, var dayStr, var partStr, ..]
                    => Fetch(yearStr, dayStr, partStr),
                ["fetch", var yearStr, var dayStr, ..]
                    => Fetch(yearStr, dayStr),
                ["submit", var yearStr, var dayStr, var partStr, var answer, "--offline", ..]
                    => Submit(yearStr, dayStr, partStr, answer, true),
                ["submit", var yearStr, var dayStr, var partStr, var answer, ..]
                    => Submit(yearStr, dayStr, partStr, answer),
                ["submit", var yearStr, var dayStr, var partStr, ..]
                    => Submit(yearStr, dayStr, partStr),
                ["status", var yearStr, ..] => Status(yearStr),
                ["status", ..] => Status(),
                _ => Help(false, args)
            };
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return -1;
        }
    }

    private static int Help(bool noArgs = false, IEnumerable<string>? args = null)
    {
        if (noArgs)
            Console.Error.WriteLine("You didn't provide any command line arguments. Displaying help.");
        
        else if (args is not null)
            Console.Error.Write($"Invalid command line arguments '{string.Join(" ", args)}'. Displaying help.");
        
        Console.Error.WriteLine(HelpText);
        return 0;
    }

    private static int Cookie(string cookie)
    {
        using FileStream file = File.Open(
            Path.Combine(AppContext.BaseDirectory, "cookie.txt"), 
            FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        using TextWriter textWriter = new StreamWriter(file);
        textWriter.Write(cookie);
        return 0;
    }

    private static int Fetch(string yearStr, string dayStr, string? partStr = null)
    {
        (ushort year, byte day, byte? part) = Parse(yearStr, dayStr, partStr);

        string? artifactsPath = TryGetArtifactsPath();
        if (artifactsPath is null)
            return 1;

        string directory = Path.Combine(artifactsPath, yearStr, dayStr);
        string filename = Path.Combine(directory, $"input{partStr}.txt");
        Directory.CreateDirectory(directory);
        
        if (File.Exists(filename))
        {
            Console.Write(File.ReadAllText(filename));
            return 0;
        }

        string cookie = ReadCookie();
        using Stream httpStream = Aoc.GetInput(year, day, cookie);
        
        using FileStream fileStream = File.Open(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
        httpStream.CopyTo(fileStream);
        fileStream.Flush();
        fileStream.Position = 0;
        using StreamReader reader = new(fileStream);
        
        Console.Write(reader.ReadToEnd());
        
        return 0;
    }

    private static int Submit(string yearStr, string dayStr, string partStr, string? answer = null, bool offline = false)
    {
        (ushort year, byte day, byte? part) = Parse(yearStr, dayStr, partStr);
        if (part is null)
            throw new ArgumentNullException(nameof(part), 
                "Please provide the part you want to solve as the 3rd argument");
        
        string? artifactsPath = TryGetArtifactsPath();
        if (artifactsPath is null)
            return 1;
        
        answer ??= Console.In.ReadToEnd().Trim();

        if (answer.Length == 0)
            throw new InvalidOperationException("Can't submit an empty answer!");
        
        string directory = Path.Combine(artifactsPath, yearStr, dayStr);
        string filename = Path.Combine(directory, $"answer{partStr}.txt");
        Directory.CreateDirectory(directory);
        
        if (File.Exists(filename))
            return SubmitCached(answer, filename);

        if (offline)
            return SubmitOffline(answer, filename, year, day, part.Value);

        string cookie = ReadCookie();

        (bool alreadySolved, bool correctly) = SubmitCheckAlreadySolved(
            year, day, part, answer, cookie, filename);

        if (alreadySolved && correctly)
        {
            Console.Error.WriteLine($"'{answer}' is the right answer.");
            return 0;
        }

        if (alreadySolved && !correctly)
        {
            Console.Error.WriteLine($"'{answer}' is not the right answer.");
            return 1;
        }
        
        string content = Aoc.Submit(cookie, year, day, part.Value, answer);
        bool correct = content.Contains("That's the right answer!");

        if (!correct)
        {
            Console.Error.WriteLine($"'{answer}' is not the right answer.");
            return 1;
        }
        
        Console.Error.WriteLine($"'{answer}' is the right answer!");
        File.WriteAllText(filename, answer);
        
        return 0;
    }

    private static (bool alreadySolved, bool correctly) SubmitCheckAlreadySolved(ushort year, byte day,
        [DisallowNull] byte? part, string answer, string cookie, string filename)
    {
        const string magicString = "Your puzzle answer was <code>";

        using Stream httpStream = Aoc.GetDescription(year, day, cookie);
        using StreamReader streamReader = new(httpStream);
        
        ReadOnlySpan<char> span = streamReader.ReadToEnd().AsSpan();
        
        int firstAnswerPosition = span.IndexOf(magicString);
        bool firstPuzzleAlreadySolved = firstAnswerPosition > 0;
        if (firstPuzzleAlreadySolved) 
            span = span[(firstAnswerPosition + magicString.Length)..];

        if (firstPuzzleAlreadySolved && part == 1)
        {
            string correctAnswer = span[..span.IndexOf("</code>")].ToString();
            if (answer.Equals(correctAnswer, StringComparison.InvariantCultureIgnoreCase))
            {
                File.WriteAllText(filename, answer);
                return (true, true);
            }
            return (true, false);
        }

        int secondAnswerPosition = span.IndexOf(magicString);
        bool secondPuzzleAlreadySolved = secondAnswerPosition > 0;
        if (secondPuzzleAlreadySolved)
            span = span[(secondAnswerPosition + magicString.Length)..];

        if (secondPuzzleAlreadySolved && part == 2)
        {
            string correctAnswer = span[..span.IndexOf("</code>")].ToString();
            if (answer.Equals(correctAnswer, StringComparison.InvariantCultureIgnoreCase))
            {
                File.WriteAllText(filename, answer);
                return (true, true);
            }
            return (true, false);
        }

        return (false, false);
    }

    private static int SubmitOffline(string answer, string filename, ushort year, byte day, byte part)
    {
        if (File.Exists(filename))
            throw new InvalidOperationException($"File {filename} already exists!");
            
        File.WriteAllText(filename, answer);
        Console.Error.WriteLine($"Assuming that '{answer}' was the right answer for {year} day {day} / part {part}.");
        return 0;
    }

    private static int SubmitCached(string answer, string filename)
    {
        string cachedAnswer = File.ReadAllText(filename);

        if (cachedAnswer.Equals(answer, StringComparison.InvariantCultureIgnoreCase))
        {
            Console.Error.WriteLine($"'{answer}' is the right answer (cached)!");
            return 0;
        }
            
        Console.Error.WriteLine($"'{answer}' is not the right answer (cached: {cachedAnswer}).");
        return 1;
    }

    private static int Status(string? yearStr = null)
    {
        if (yearStr is not null && !ushort.TryParse(yearStr, out _))
            throw new ArgumentException($"Invalid value for year: '{yearStr}'");

        string? artifactsPath = TryGetArtifactsPath();
        if (artifactsPath is null)
            return 1;
        
        (bool singleYear, string directory) = yearStr is not null 
            ? (true, Path.Combine(artifactsPath, yearStr))
            : (false, artifactsPath);
        
        return singleYear 
            ? WriteStatusSingleYear(directory) 
            : WriteStatusAllYears(directory);
    }

    private static int WriteStatusSingleYear(string yearDirectory)
    {
        Console.WriteLine($"AOC status for {yearDirectory.Split('\\').Last()}:");
        
        int totalGoldStars = 0;
        ConsoleColor defaultColor = Console.ForegroundColor;
        
        foreach (string dayDirectory in Directory.EnumerateDirectories(yearDirectory))
        {
            Console.ForegroundColor = defaultColor;
            
            string dayDir = dayDirectory.Split('\\').Last();
            if (dayDir.Length < 2)
                dayDir = $"0{dayDir}";
            
            Console.Write("Day ");
            Console.Write(dayDir);
            Console.Write(": ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            
            if (File.Exists(Path.Combine(dayDirectory, "answer1.txt")))
            {
                Console.Write('*');
                totalGoldStars++;
            }
            else
                Console.Write(' ');

            if (File.Exists(Path.Combine(dayDirectory, "answer2.txt")))
            {
                Console.Write('*');
                totalGoldStars++;
            }

            Console.WriteLine();
        }
        
        Console.ForegroundColor = defaultColor;
        Console.Write("Total:  ");
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{totalGoldStars} Gold Stars");
        
        Console.ForegroundColor = defaultColor;
        return 0;
    }

    private static int WriteStatusAllYears(string artifactsDirectory)
    {
        Console.WriteLine("AOC puzzle status for all years:");
        
        ConsoleColor defaultColor = Console.ForegroundColor;
        int totalGoldStars = 0;
        
        foreach (string yearDirectory in Directory.EnumerateDirectories(artifactsDirectory))
        {
            Console.ForegroundColor = defaultColor;
            int totalGoldStarsYear = 0;
            
            foreach (string dayDirectory in Directory.EnumerateDirectories(yearDirectory))
            {
                if (File.Exists(Path.Combine(dayDirectory, "answer1.txt")))
                {
                    totalGoldStars++;
                    totalGoldStarsYear++;
                }

                if (File.Exists(Path.Combine(dayDirectory, "answer2.txt")))
                {
                    totalGoldStars++;
                    totalGoldStarsYear++;
                }
            }
            
            Console.Write($"{yearDirectory.Split('\\').Last()}:  ");
            if (totalGoldStarsYear < 10)
                Console.Write(" ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{totalGoldStarsYear} *");
        }

        Console.ForegroundColor = defaultColor;
        Console.Write("Total: ");
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{totalGoldStars} Gold Stars");
        
        Console.ForegroundColor = defaultColor;
        return 0;
    }

    private static string ReadCookie() 
        => File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "cookie.txt"));

    private static string? TryGetArtifactsPath()
    {
        string artifactsPathFilename = Path.Combine(AppContext.BaseDirectory, "artifactspath.txt");
        string? artifactsPath;
        
        if (!File.Exists(artifactsPathFilename))
        {
            Console.Error.WriteLine("It seems like you haven't configured the location of your AOC artifacts.");
            Console.Error.WriteLine("Please enter an absolute path to where these should be stored and press enter:");
            artifactsPath = Console.ReadLine();

            if (string.IsNullOrEmpty(artifactsPath))
            {
                Console.Error.WriteLine("Input was empty.");
                return null;
            }
            
            File.WriteAllText(artifactsPathFilename, artifactsPath);
            return artifactsPath;
        }

        artifactsPath = File.ReadAllText(artifactsPathFilename);
        return artifactsPath;
    }

    private static (ushort year, byte day, byte? part) Parse(string yearStr, string dayStr, string? partStr)
    {
        string errors = "";
        if (!ushort.TryParse(yearStr, out ushort year))
            errors += $"Invalid value for year: '{yearStr}'";
        
        if (!byte.TryParse(dayStr, out byte day))
            errors += $"\nInvalid value for day: '{dayStr}'";

        byte part = 0;
        if (partStr is not null && !byte.TryParse(partStr, out part))
            errors += $"\nInvalid value for part: '{partStr}'";

        if (errors.Length > 0)
            throw new ArgumentException(errors);
        
        return (year, day, partStr is null ? null : part);
    }
}