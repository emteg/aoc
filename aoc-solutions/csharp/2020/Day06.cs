using Common;

namespace _2020;

public static class Day06
{
    public static string Part1(IEnumerable<string> input)
    {
        List<HashSet<char>> allAnswersByGroup = [];
        HashSet<char> currentGroupsAnswers = [];
        foreach (string currentPersonsAnswers in input)
        {
            if (currentPersonsAnswers.Length == 0)
            {
                allAnswersByGroup.Add(currentGroupsAnswers);
                currentGroupsAnswers = [];
                continue;
            }
            
            foreach (char c in currentPersonsAnswers) 
                currentGroupsAnswers.Add(c);
        }
        allAnswersByGroup.Add(currentGroupsAnswers);

        int result = allAnswersByGroup.Sum(hashSet => hashSet.Count);
        return result.ToString();
    }
    
    public static string Part1Sample() => Part1(Sample.Lines());
    
    
    public static string Part2(IEnumerable<string> input)
    {
        List<Group> groups = [];
        List<Person> currentGroup = [];
        foreach (string personsAnswers in input)
        {
            if (personsAnswers.Length == 0)
            {
                groups.Add(new Group(currentGroup));
                currentGroup = [];
                continue;
            }
            currentGroup.Add(new Person(personsAnswers));
        }
        groups.Add(new Group(currentGroup));
        
        return groups.Sum(group => group.QuestionsAnsweredWithYesByEverybody.Count).ToString();
    }
    
    public static string Part2Sample() => Part2(Sample.Lines());
    
    private const string Sample = """
        abc
        
        a
        b
        c
        
        ab
        ac
        
        a
        a
        a
        a
        
        b
        """;

    private readonly struct Group
    {
        public readonly IReadOnlySet<char> QuestionsAnsweredWithYesByEverybody;

        public Group(IEnumerable<Person> persons)
        {
            HashSet<char>? intersection = null;
            foreach (Person person in persons)
            {
                if (intersection is null)
                {
                    intersection = [..person.QuestionsAnsweredWithYes];
                    continue;
                }

                intersection.IntersectWith(person.QuestionsAnsweredWithYes);
            }

            QuestionsAnsweredWithYesByEverybody = intersection!;
        }
    }

    private readonly struct Person
    {
        public readonly IReadOnlySet<char> QuestionsAnsweredWithYes;

        public Person(string questionsAnsweredWithYes)
        {
            this.QuestionsAnsweredWithYes = new HashSet<char>(questionsAnsweredWithYes);
        }

        public override string ToString()
        {
            return new string(QuestionsAnsweredWithYes.ToArray());
        }
    }
}