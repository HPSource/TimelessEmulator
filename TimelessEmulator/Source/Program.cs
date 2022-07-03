using System;
using System.Collections;
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

        String[] skills = new String[] 
        //{"Melding", "Nimbleness", "Tolerance", "Undertaker", "Vampirism"}
        //{"Essence Extraction", "Quick Recovery", "Anointed Flesh", "Asylum"}
        //{"Essence Extraction", "Quick Recovery", "Anointed Flesh", "Asylum", "Arcanist's Dominion"}
        //{"Overcharge", "Light of Divinity", "Holy Dominion", "Divine Fervour", "Faith and Steel", "Devotion", "Endurance"}
        //{"Overcharge", "Light of Divinity", "Holy Dominion", "Divine Fervour"}
        //{"Overcharge", "Light of Divinity", "Holy Dominion", "Divine Fervour", "Divine Judgement", "Divine Wrath"}
        {"Overcharge", "Light of Divinity", "Holy Dominion", "Divine Fervour", "Divine Judgement", "Divine Wrath", "Devotion", "Endurance", "Faith and Steel"}
        ;

        ArrayList desired = new ArrayList()
        {"Slum Lord", "Eternal Fervour"}
        ;

        for (uint seed = 2000; seed <= 160000; seed = seed +20) {
            ArrayList matches = new ArrayList();
            for (int skillindex = 0; skillindex < skills.Length; skillindex++) {
                TimelessJewel timelessJewel = GetTimelessJewelFromInput(seed);

                AlternateTreeManager alternateTreeManager = new AlternateTreeManager(DataManager.GetPassiveSkillByFuzzyValue(skills[skillindex]), timelessJewel);

                AlternatePassiveSkillInformation alternatePassiveSkillInformation = alternateTreeManager.ReplacePassiveSkill();

                if (desired.Contains(alternatePassiveSkillInformation.AlternatePassiveSkill.Name)) {
                    matches.Add(skills[skillindex] + ": [cyan]" + alternatePassiveSkillInformation.AlternatePassiveSkill.Name + "[/]");
                }
            }

            if (matches.Count > 2) {
                String joined = String.Join("\n", matches.Cast<string>().ToArray());
                AnsiConsole.MarkupLine($"([red]{matches.Count}[/]) [yellow]{seed}[/]:\n{joined}");
            }

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
