using System.Globalization;
using Common;

namespace _2020;

public static class Day04
{
    public static string Part1(IEnumerable<string> input)
    {
        Passport[] passports = ParsePassports(input);
        return passports.Count(passport => passport.IsValid).ToString();
    }

    public static string Part1Sample() => Part1(Sample.Lines());

    public static string Part2(IEnumerable<string> input)
    {
        Passport[] passports = ParsePassports(input);
        return passports.Count(passport => passport.IsValidStrict).ToString();
    }

    public static string Part2Sample()
    {
        string result = string.Empty;

        result += Part2(ValidSamples.Lines());
        result += Environment.NewLine;
        result += Part2(InvalidSamples.Lines());
        
        return result;
    }

    private static Passport[] ParsePassports(IEnumerable<string> input)
    {
        List<Passport> passports = [];
        List<string> currentPassportLines = [];
        foreach (string line in input)
        {
            if (line.Length > 0)
                currentPassportLines.Add(line);
            else
            {
                passports.Add(Passport.Parse(string.Join(" ", currentPassportLines)));
                currentPassportLines.Clear();
            }
        }
        passports.Add(Passport.Parse(string.Join(" ", currentPassportLines)));
        currentPassportLines.Clear();
        return passports.ToArray();
    }

    private const string Sample = """
        ecl:gry pid:860033327 eyr:2020 hcl:#fffffd
        byr:1937 iyr:2017 cid:147 hgt:183cm

        iyr:2013 ecl:amb cid:350 eyr:2023 pid:028048884
        hcl:#cfa07d byr:1929

        hcl:#ae17e1 iyr:2013
        eyr:2024
        ecl:brn pid:760753108 byr:1931
        hgt:179cm

        hcl:#cfa07d eyr:2025 pid:166559648
        iyr:2011 ecl:brn hgt:59in
        """;
    
    private const string InvalidSamples = """
        eyr:1972 cid:100
        hcl:#18171d ecl:amb hgt:170 pid:186cm iyr:2018 byr:1926

        iyr:2019
        hcl:#602927 eyr:1967 hgt:170cm
        ecl:grn pid:012533040 byr:1946

        hcl:dab227 iyr:2012
        ecl:brn hgt:182cm pid:021572410 eyr:2020 byr:1992 cid:277

        hgt:59cm ecl:zzz
        eyr:2038 hcl:74454a iyr:2023
        pid:3556412378 byr:2007
        """;
    
    private const string ValidSamples = """
        pid:087499704 hgt:74in ecl:grn iyr:2012 eyr:2030 byr:1980
        hcl:#623a2f

        eyr:2029 ecl:blu cid:129 byr:1989
        iyr:2014 pid:896056539 hcl:#a97842 hgt:165cm

        hcl:#888785
        hgt:164cm byr:2001 iyr:2015 cid:88
        pid:545766238 ecl:hzl
        eyr:2022

        iyr:2010 hgt:158cm hcl:#b6652a ecl:blu byr:1944 eyr:2021 pid:093154719
        """;

    private readonly struct Passport
    {
        public readonly string? BirthYear;
        public readonly string? IssueYear;
        public readonly string? ExpirationYear;
        public readonly string? Height;
        public readonly string? HairColor;
        public readonly string? EyeColor;
        public readonly string? PassportId;
        public readonly string? CountryId;

        public Passport(string? birthYear, string? issueYear, string? expirationYear, 
            string? height, string? hairColor, string? eyeColor, string? passportId, 
            string? countryId)
        {
            BirthYear = birthYear;
            IssueYear = issueYear;
            ExpirationYear = expirationYear;
            Height = height;
            HairColor = hairColor;
            EyeColor = eyeColor;
            PassportId = passportId;
            CountryId = countryId;
        }

        public bool IsValid => BirthYear is not null &&
                               IssueYear is not null &&
                               ExpirationYear is not null &&
                               Height is not null &&
                               HairColor is not null &&
                               EyeColor is not null &&
                               PassportId is not null;

        public bool IsValidStrict
        {
            get
            {
                if (!int.TryParse(BirthYear, out int birthYear) || birthYear is < 1920 or > 2002)
                    return false;

                if (!int.TryParse(IssueYear, out int issueYear) || issueYear is < 2010 or > 2020)
                    return false;

                if (!int.TryParse(ExpirationYear, out int expirationYear) || expirationYear is < 2020 or > 2030)
                    return false;

                if (Height?.Contains("cm") is true)
                {
                    string heightVal = Height![..Height!.IndexOf("cm", StringComparison.InvariantCultureIgnoreCase)];
                    if (!int.TryParse(heightVal, out int height) || height is < 150 or > 193)
                        return false;
                }
                else if (Height?.Contains("in") is true)
                {
                    string heightVal = Height![..Height!.IndexOf("in", StringComparison.InvariantCultureIgnoreCase)];
                    if (!int.TryParse(heightVal, out int height) || height is < 59 or > 76)
                        return false;
                }
                else
                    return false;

                if (HairColor?.Length != 7 ||
                    HairColor[0] != '#' ||
                    !int.TryParse(HairColor[1..], NumberStyles.HexNumber, null, out _))
                    return false;

                string[] eyeColors = ["amb", "blu", "brn", "gry", "grn", "hzl", "oth"];
                if (!eyeColors.Contains(EyeColor))
                    return false;

                if (PassportId?.Length != 9 || !int.TryParse(PassportId, out _))
                    return false;

                return true;
            }
        }

        public static Passport Parse(string line)
        {
            string? birthYear = null, issueYear = null, expirationYear = null, height = null;
            string? hairColor = null, eyeColor = null, passportId = null, countryId = null;
            
            string[] pairs = line.Split(' ');
            foreach (string pair in pairs)
            {
                string[] keyValue = pair.Split(':');
                string key = keyValue[0];
                string value = keyValue[1];
                
                if (key == "byr")
                    birthYear = value;
                else if (key == "iyr")
                    issueYear = value;
                else if (key == "eyr")
                    expirationYear = value;
                else if (key == "hgt")
                    height = value;
                else if (key == "hcl")
                    hairColor = value;
                else if (key == "ecl")
                    eyeColor = value;
                else if (key == "pid")
                    passportId = value;
                else if (key == "cid")
                    countryId = value;
            }

            return new Passport(
                birthYear, issueYear, expirationYear, height, hairColor, eyeColor, 
                passportId, countryId);
        }
    }
    
}