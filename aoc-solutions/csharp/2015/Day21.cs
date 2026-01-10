namespace _2015;

public static class Day21
{
    public static string Part1(IEnumerable<string> input) => Part1(Equipment.BossFromInput(input.ToArray())).ToString();

    public static string Part1Sample() => "There is no sample for this puzzle";
    
    private static uint Part1(Equipment boss)
    {
        Equipment cheapest = null!;
        uint cost = uint.MaxValue;
        foreach (Equipment generatedEquipment in GenerateEquipments(boss, false))
        {
            if (generatedEquipment.TotalCost < cost)
            {
                cost = generatedEquipment.TotalCost;
                cheapest = generatedEquipment;
            }
        }
        Console.Error.WriteLine($"Cheapest equipment to beat the boss costs {cost}: {cheapest}");
        return cost;
    }
    
    public static string Part2(IEnumerable<string> input) => Part2(Equipment.BossFromInput(input.ToArray())).ToString();

    public static string Part2Sample() => "There is no sample for this puzzle";
    
    private static uint Part2(Equipment boss)
    {
        Equipment mostExpansive = null!;
        uint cost = uint.MinValue;
        foreach (Equipment generatedEquipment in GenerateEquipments(boss, true))
        {
            if (generatedEquipment.TotalCost > cost)
            {
                cost = generatedEquipment.TotalCost;
                mostExpansive = generatedEquipment;
            }
        }
        Console.Error.WriteLine($"Most expansive equipment failing to beat the boss costs {cost}: {mostExpansive}");
        return cost;
    }
    
    private static readonly Equipment[] Weapons = [
        new("Dagger",      8, 4),
        new("Shortsword", 10, 5),
        new("Warhammer",  25, 6),
        new("Longsword",  40, 7),
        new("Greataxe",   74, 8),
    ];
    private static readonly Equipment[] Armors = [
        new("Leather",    13, armor: 1),
        new("Chainmail",  31, armor: 2),
        new("Splintmail", 53, armor: 3),
        new("Bandedmail", 75, armor: 4),
        new("Platemail", 102, armor: 5),
    ];
    private static readonly Equipment[] Rings = [
        new("Damage +1",  25, 1, 0),
        new("Damage +2",  50, 2, 0),
        new("Damage +3", 100, 3, 0),
        new("Defense +1", 20, 0, 1),
        new("Defense +2", 40, 0, 2),
        new("Defense +3", 80, 0, 3),
    ];
    
    private static IEnumerable<Equipment> GenerateEquipments(Equipment boss, bool invertCondition)
    {
        Equipment playerEq;
        foreach (Equipment weapon in Weapons)
        {
            // weapon only, no armors, no rings
            playerEq = weapon.Copy().SetHitPoints(100);
            if (Equipment.CanPlayerWin(playerEq, boss, invertCondition))
                yield return playerEq;
            
            foreach (Equipment armor in Armors)
            {
                // weapon + armor, but no rings
                playerEq = weapon.Copy().Apply(armor.Copy()).SetHitPoints(100);
                if (Equipment.CanPlayerWin(playerEq, boss, invertCondition))
                    yield return playerEq;
                
                foreach (Equipment firstRing in Rings)
                {
                    // weapon + armor + 1 ring
                    playerEq = weapon.Copy().Apply(armor.Copy()).Apply(firstRing.Copy()).SetHitPoints(100);
                    if (Equipment.CanPlayerWin(playerEq, boss, invertCondition))
                        yield return playerEq;

                    foreach (Equipment secondRing in Rings.Where(it => it != firstRing))
                    {
                        // weapon + armor + 2 rings
                        playerEq = weapon.Copy().Apply(armor.Copy()).Apply(firstRing.Copy()).Apply(secondRing.Copy()).SetHitPoints(100);
                        if (Equipment.CanPlayerWin(playerEq, boss, invertCondition))
                            yield return playerEq;
                    }
                }
            }

            foreach (Equipment firstRing in Rings)
            {
                // weapon + 1 ring
                playerEq = weapon.Copy().Apply(firstRing.Copy()).SetHitPoints(100);
                if (Equipment.CanPlayerWin(playerEq, boss, invertCondition))
                    yield return playerEq;
                
                foreach (Equipment secondRing in Rings.Where(it => it != firstRing))
                {
                    // weapon + 2 rings
                    playerEq = weapon.Copy().Apply(firstRing.Copy()).Apply(secondRing.Copy()).SetHitPoints(100);
                    if (Equipment.CanPlayerWin(playerEq, boss, invertCondition))
                        yield return playerEq;
                }
            }
        }
    }
    
    private sealed class Equipment
    {
        public string Name { get; }
        public string FullName => inner is null ? Name : $"{Name}, {inner?.FullName}";
        public uint Cost { get; }
        public uint TotalCost => Cost + (inner?.TotalCost ?? 0);
        public int Damage { get; }
        public int TotalDamage => Damage + (inner?.TotalDamage ?? 0);
        public int Armor { get; }
        public int TotalArmor => Armor + (inner?.TotalArmor ?? 0);
        public int HitPoints { get; private set; }

        public Equipment(string name, uint cost, ushort damage = 0, ushort armor = 0)
        {
            originalName = name;
            if (!name.Contains("Damage") && !name.Contains("Defense"))
            {
                if (damage > 0)
                    Name = name + " (+" + damage + "D)";
                else
                    Name = name + " (+" + armor + "A)";
            }
            else
                Name = name;
            Cost = cost;
            Damage = damage;
            Armor = armor;
        }

        public Equipment Apply(Equipment other)
        {
            if (inner is null)
                inner = other;
            else
                inner.Apply(other);
            return this;
        }

        public Equipment SetHitPoints(ushort hitPoints)
        {
            HitPoints = hitPoints;
            return this;
        }

        public Equipment Copy()
        {
            return new Equipment(originalName, Cost, (ushort)Damage, (ushort)Armor);
        }

        public int DamageInflictedUpon(Equipment defender)
        {
            return Math.Max(TotalDamage - defender.TotalArmor, 1);
        }

        public int NumberOfAttacksRequiredToKill(Equipment defender)
        {
            int damageInflictedUpon = DamageInflictedUpon(defender);
            int hits = defender.HitPoints / damageInflictedUpon;
            int remainder = defender.HitPoints % damageInflictedUpon;
            if (remainder > 0)
                remainder = 1;
            return hits + remainder;
        }

        public static bool CanPlayerWin(Equipment player, Equipment boss, bool invertCondition)
        {
            int hitsToKillBoss = player.NumberOfAttacksRequiredToKill(boss);
            int hitsToKillPlayer = boss.NumberOfAttacksRequiredToKill(player);
            
            bool canPlayerWin = hitsToKillBoss <= hitsToKillPlayer;
            if (!invertCondition)
                return canPlayerWin;
            return !canPlayerWin;
        }

        public static Equipment BossFromInput(string[] lines)
        {
            ushort hitPoints = ushort.Parse(lines[0].Split(": ")[1]);
            ushort damage = ushort.Parse(lines[1].Split(": ")[1]);
            ushort armor = ushort.Parse(lines[2].Split(": ")[1]);
            
            return new Equipment("Boss", 0, damage, armor).SetHitPoints(hitPoints);
        }

        public override string ToString()
        {
            return $"{FullName} - HP: {HitPoints}, DMG: {TotalDamage}, ARM: {TotalArmor}, COST: {TotalCost}";
        }

        private Equipment? inner;
        private readonly string originalName;
    }
}