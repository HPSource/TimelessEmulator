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

        String[] skills = GetPassivesToTransform();

        String[] desired = GetDesiredPassives();

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

    private static string[] GetPassivesToTransform() {
        TextPrompt<String> timelessJewelLocationPrompt = new TextPrompt<String>($"[green]Enter a comma separated (case sensitive) list of Notables to check (e.g. Overcharged,Endurance):[/]")
            .Validate((String input) =>
            {
                string[] skills = input.Split(",");

                for (int index = 0; index < skills.Length; index++) {
                    PassiveSkill passiveSkill = DataManager.GetPassiveSkillByFuzzyValue(skills[index]);

                    if (passiveSkill == null)
                        return ValidationResult.Error($"[red]Error[/]: Unable to find [yellow]passive skill[/] `{skills[index]}`.");

                    if (!DataManager.IsPassiveSkillValidForAlteration(passiveSkill))
                        return ValidationResult.Error($"[red]Error[/]: The [yellow]passive skill[/] `{skills[index]}` is not valid for alteration.");
                }

                return ValidationResult.Success();
            });

        return AnsiConsole.Prompt(timelessJewelLocationPrompt).Split(",");
    }

    private static string[] GetDesiredPassives() {
        TextPrompt<string> timelessPassivePrompt = new TextPrompt<string>($"[green]Enter a comma separated (case sensitive) list of Notables desired (e.g. Slum Lord,Axiom Warden)[/]");

        return AnsiConsole.Prompt(timelessPassivePrompt).Split(",");
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
