using System.Reflection;
using Common;

namespace Aoc;

internal static class Program
{
    private const string HelpText = """
        Solves the requested AOC puzzle and write the result to Standard Out.
        
        The following positional arguments are available:
        
        help
            Shows this help. Also shown if no arguments are passed. Exist code always 0.
        
        <year> <day> <part> [-from <filename>] [--sample]
            Solves the requested AOC puzzle.
            
            By default, the input is read from Standard In. When the optional from
            parameter is set, the input is read from the given file instead.
            
            If the --sample switch is used, the puzzle is executed for the sample value.
            This will not read any input and ignore the --from parameter if it is given.
            
            Year must be an unsigned short. Day and part must be an unsigned byte.
            
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
                [var yearStr, var dayStr, var partStr, "--sample",..] 
                    => Solve(yearStr, dayStr, partStr, useSample: true),
                [var yearStr, var dayStr, var partStr, "--from", var filename,..] 
                    => Solve(yearStr, dayStr, partStr, filename),
                [var yearStr, var dayStr, var partStr, ..] 
                    => Solve(yearStr, dayStr, partStr),
                _ => Help(false, args)
            };
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return -1;
        }
    }

    private static int Solve(string yearStr, string dayStr, string partStr, string? filename = null, bool useSample = false)
    {
        (ushort year, byte day, byte part) = Parse(yearStr, dayStr, partStr);
        IEnumerable<string> input = (filename, useSample) switch
        {
            (null, false) => Console.In.ReadToEnd().Lines(),
            (_, true) => [],
            (_, false) => FileReader.ReadLines(filename)
        };

        MethodInfo methodToCall = FindCodeToExecute(year, day, part, useSample);

        string res = useSample 
            ? (string)methodToCall.Invoke(null, null)!
            : (string)methodToCall.Invoke(null, [input])!;
        
        Console.Write(res);
        return 0;
    }

    private static MethodInfo FindCodeToExecute(ushort year, byte day, byte part, bool useSample)
    {
        try
        {
            Assembly assemblyToLoad = Assembly.Load(new AssemblyName(year.ToString()));
            
            string className = day < 10
                ? $"Day0{day}"
                : $"Day{day}";
            TypeInfo classToUse = assemblyToLoad.DefinedTypes.First(typeInfo => typeInfo.Name.Equals(className));
            
            MethodInfo? methodToCall = classToUse.GetMethod($"Part{part}{(useSample ? "Sample" : "")}");
            if (methodToCall is null)
                throw new ApplicationException(
                    $"Can't find code to execute for '{year} / day {day} / part {part}'. Check your command line arguments.");
            
            return methodToCall;
        }
        catch (FileNotFoundException ex)
        {
            throw new ApplicationException(
                $"Can't find code to load for year '{year}'. Check your command line arguments.", ex);
        }
        catch (InvalidOperationException ex)
        {
            throw new ApplicationException(
                $"Can't find code to execute for '{year} / day {day}'. Check your command line arguments.", ex);
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
    
    private static (ushort year, byte day, byte part) Parse(string yearStr, string dayStr, string partStr)
    {
        string errors = "";
        if (!ushort.TryParse(yearStr, out ushort year))
            errors += $"Invalid value for year: '{yearStr}'";
        
        if (!byte.TryParse(dayStr, out byte day))
            errors += $"\nInvalid value for day: '{dayStr}'";

        if (!byte.TryParse(partStr, out byte part))
            errors += $"\nInvalid value for part: '{partStr}'";

        if (errors.Length > 0)
            throw new ArgumentException(errors);
        
        return (year, day, part);
    }
}