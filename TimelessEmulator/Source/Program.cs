using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using TimelessEmulator.Data;
using TimelessEmulator.Data.Models;
using TimelessEmulator.Game;

namespace TimelessEmulator;

public static class Program
{

    static Program()
    {

    }

    public static void Main(string[] arguments)
    {
        Console.Title = $"{Settings.ApplicationName} Ver. {Settings.ApplicationVersion}";

        AnsiConsole.MarkupLine("Hello, [green]Henry[/]!");
        AnsiConsole.MarkupLine("Loading [green]data files[/]...");

        if (!DataManager.Initialize())
            ExitWithError("Failed to initialize the [yellow]data manager[/].");

        uint timelessJewelSeed = GetSeedFromConsole();

        String location = GetJewelLocationFromConsole();

        if (location.Equals("All")) {
            foreach (KeyValuePair<String, String[]> pair in keystones) {
                AnsiConsole.MarkupLine($"[red]{pair.Key}[/]");
                ComputeSkillTreeTransformations(pair.Value, timelessJewelSeed);        
            }
        } else {
            AnsiConsole.MarkupLine($"[red]{location}[/]");
            ComputeSkillTreeTransformations(keystones.First(q => (q.Key.Equals(location))).Value, timelessJewelSeed);
        }
        
        WaitForExit();
    }

    private static TimelessJewel GetTimelessJewelFromInput(uint timelessJewelSeed)
    {
        uint alternateTreeVersionIndex = 5;

        AlternateTreeVersion alternateTreeVersion = DataManager.AlternateTreeVersions
            .First(q => (q.Index == alternateTreeVersionIndex));

        TimelessJewelConqueror timelessJewelConqueror = new TimelessJewelConqueror(1, 0);

        return new TimelessJewel(alternateTreeVersion, timelessJewelConqueror, timelessJewelSeed);
    }

    private static uint GetSeedFromConsole() {
        TextPrompt<uint> timelessJewelSeedTextPrompt = new TextPrompt<uint>($"[green]Timeless Jewel Seed (2000 - 160000)[/]:")
            .Validate((uint input) =>
            {
                if ((input >= 2000) &&
                    (input <= 160000))
                {
                    return ValidationResult.Success();
                }

                return ValidationResult.Error($"[red]Error[/]: The [yellow]timeless jewel seed[/] must be between {2000} and {160000}.");
            });

        return AnsiConsole.Prompt(timelessJewelSeedTextPrompt);
    }

    private static String GetJewelLocationFromConsole() {
        String locations = String.Join(", ", keystones.Keys);
        TextPrompt<String> timelessJewelLocationPrompt = new TextPrompt<String>($"[green]Timeless Jewel Location (or All)[/]\n({locations})")
            .Validate((String input) =>
            {
                if (keystones.Keys.Contains(input)) {
                    return ValidationResult.Success();
                }

                if (input.Equals("All")) {
                    return ValidationResult.Success();
                }

                return ValidationResult.Error($"[red]Error[/]: Enter a keystone (or null)");
            });

        return AnsiConsole.Prompt(timelessJewelLocationPrompt);
    }

    private static void ComputeSkillTreeTransformations(String[] skills, uint timelessJewelSeed) {
        for (int skillindex = 0; skillindex < skills.Length; skillindex++) {
            TimelessJewel timelessJewel = GetTimelessJewelFromInput(timelessJewelSeed);

            AlternateTreeManager alternateTreeManager = new AlternateTreeManager(DataManager.GetPassiveSkillByFuzzyValue(skills[skillindex]), timelessJewel);

            AlternatePassiveSkillInformation alternatePassiveSkillInformation = alternateTreeManager.ReplacePassiveSkill();

            AnsiConsole.MarkupLine(skills[skillindex] + ": [cyan]" + alternatePassiveSkillInformation.AlternatePassiveSkill.Name + "[/]");  
        }      
    }

    private static Dictionary<String, String[]> keystones = new Dictionary<String, String[]>() {
            {"MoM", new String[]{"Anointed Flesh", "Asylum", "Essence Infusion", "Arcanist's Dominion", "Annihilation", "Enduring Bond", "Essence Extraction", "Quick Recovery"}},
            {"ZO", new String[]{"Agility", "Might", "Arcane Guarding", "Serpent Stance", "Fearsome Force", "Blunt Trauma", "Hex Master", "Death Attunement", "Enigmatic Reach", "Prism Weave", "Acrimony", "Unnatural Calm", "Corruption"}},
            {"Witch", new String[]{"Breath of Flames", "Heart of Thunder", "Breath of Lightning", "Breath of Rime", "Heart of Ice", "Golem Commander", "Skittering Runes", "Presage", "Discord Artisan", "Infused Flesh", "Cruel Preparation", "Deep Thoughts", "Instability", "Essence Surge", "Wandslinger", "Prodigal Perfection", "Mystic Bulwark", "Enigmatic Defence", "Frost Walker", "Lord of the Dead", "Arcane Will", "Mental Rapidity"}},
            {"EB", new String[]{"Utmost Intellect", "Elder Power", "Disintegration", "Fusillade", "Arcing Blows", "Whispers of Doom", "Influence", "Mysticism", "Efficient Explosives", "Light Eater", "searing Heat", "Alacrity", "Successive Detonations"}},
            {"Pain Attunement", new String[]{"Grave Intentions", "Deep Wisdom", "Nimbleness", "Vampirism", "Undertaker", "Tolerance", "Melding"}},
            {"Shadow", new String[]{"Soul Thief", "Coldhearted Calculation", "Elemental Focus", "Saboteur", "Resourcefulness", "Fangs of the Viper", "Will of Blades", "Sleight of Hand", "Blood Drinker", "Master of Blades", "Clever Thief", "Depth Perception", "Claws of the Magpie", "Claws of the Hawk", "Backstabbing", "Flaying", "One With Evil", "Mind Drinker", "Infused", "Frenetic", "From the Shadows"}},
            {"Supreme Ego", new String[]{"Wasting", "Adder's Touch", "Void Barrier", "Revenge of the Hunted", "Replenishing Remedies", "Ballistics", "Charisma", "Overcharged", "True Strike", "Dire Torment", "Taste for Blood", "Master Sapper"}},
            {"Ranger", new String[]{"Silent Steps", "Careful Conservationist", "Inveterate", "Trick Shot", "King of the Hill", "Master Fletcher", "Heartseeker", "Survivalist", "Aspect of the Lynx", "Weapon Artistry", "Quickstep", "Intuition", "Fervour", "Acuity", "Herbalism", "Winter Spirit", "Flash Freeze", "Swift Venoms"}},
            {"Point Blank", new String[]{"Thick Skin", "Longshot", "Twin Terrors", "Utmost Swiftness", "Fangs of Frost", "Dazzling Strikes", "Feller of Foes", "Blade Barrier", "Marked for Death", "Bladedancer"}},
            {"EE", new String[]{"Profane Chemistry", "Crystal Skin", "Avatar of the Hunt", "Weathered Hunter", "Burning Brutality", "Gladiator's Perseverance", "Deadly Draw", "Art of the Gladiator"}},
            {"Duelist", new String[]{"Master of the Arena", "Mana Flows", "Bravery", "Art of the Gladiator", "Defiance", "Dervish", "Destroyer", "Fury Bolts", "Measured Fury", "Vigour", "Savagery", "Titanic Impacts", "Ribcage Crusher", "Revelry", "Cloth and Chain", "Golem's Blood", "Assured Strike", "Deflection", "Dirty Techniques", "Adamant", "Surveillance", "Testudo"}},
            {"CtA", new String[]{"Steadfast", "Tribal Fury", "Lava Lash", "Blade of Cunning", "Bastion Breaker", "Executioner"}},
            {"Unwavering", new String[]{"Martial Experience", "Command of Steel", "Prismatic Skin", "Admonisher", "Eagle Eye", "Bloodletting"}},
            {"Marauder", new String[]{"Disemboweling", "Hearty", "Cleaving", "Slaughter", "Savage Wounds", "Barbarism", "Warrior Training", "Strong Arm", "Stamina", "Juggernaut", "Cannibalistic Rite", "Aggressive Bastion", "Lust for Carnage", "Robust", "Diamond Skin", "Spiked Bulwark"}},
            {"Eternal Youth", new String[]{"Steelwood Stance", "Sanctity", "Powerful Bond", "Sanctuary", "Expertise", "Ancestral Knowledge", "Deep Breaths", "Blacksmith's Clout", "Dynamo", "Gravepact", "Combat Stamina", ""}},
            {"Templar", new String[]{"Arcane Capacitor", "Runesmith", "Faith and Steel", "Divine Wrath", "Divine Judgement", "Divine Fury", "Smashing Strikes", "Devotion", "Sanctum of Thought", "Endurance", "Overcharge", "Light of Divinity", "Holy Dominion", "Divine Fervour"}},
            {"ScionTopLeft", new String[]{"Forethought", "Decay Ward", "Dreamer", "Path of the Savant", "Potency of Will", "Ash, Frost and Storm", "Shaper", "Inspiring Bond", "Veteran Soldier", "Relentless", "Malicious Intent", "Constitution", "Path of the Warrior", "Totemic Zeal", "Foresight"}},
            {"ScionTopRight", new String[]{"Foresight", "Thrill Killer", "Destructive Apparatus", "Path of the Savant", "Inspiring Bond", "Leadership", "Harrier", "True Strike", "Hired Killer", "Exceptional Performance", "Window of Opportunity", "Path of the Hunter", "Reflexes", "Potency of Will"}},
            {"ScionDuelist", new String[]{"Malicious Intent", "Path of the Warrior", "Constitution", "Hired Killer", "Reflexes", "Path of the Hunter", "Totemic Zeal", "Exceptional Performance", "Window of Opportunity", "Sentinel", "Battle Rouse", "Arcane Chemistry"}}
        };

    private static void WaitForExit()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("Press [yellow]any key[/] to exit.");

        try
        {
            Console.ReadKey();
        }
        catch { }

        Environment.Exit(0);
    }

    private static void PrintError(string error)
    {
        AnsiConsole.MarkupLine($"[red]Error[/]: {error}");
    }

    private static void ExitWithError(string error)
    {
        PrintError(error);
        WaitForExit();
    }

}
