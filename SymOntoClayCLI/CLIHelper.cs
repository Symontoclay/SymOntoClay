using Spectre.Console;
using System;
using System.Reflection;
using System.Threading;

namespace SymOntoClay.CLI
{
    public class CLIHelper
    {
        public static void ShowCopyrightSplash(int durationMs = 2000)
        {
            AnsiConsole.Clear();

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var title = new FigletText("SymOntoClay CLI")
                .Centered()
                .Color(Color.Cyan1);

            var body = new Markup(
                $"[bold white]SymOntoClay CLI[/] [grey]v{version}[/]\n" +
                $"[grey]Copyright © 2020 - {DateTime.Today.Year:####} Sergiy Tolkachov aka metatypeman.[/]\n" +
                "[grey]Press any key to continue...[/]")
                .Centered();

            var content = new Rows(
                title,
                new Rule().RuleStyle("grey"),
                body);

            var panel = new Panel(Align.Center(content, VerticalAlignment.Middle))
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Cyan1)
                .Padding(2, 1)
                .Expand();

            AnsiConsole.Write(panel);

            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < durationMs)
            {
                if (!Console.IsInputRedirected && Console.KeyAvailable)
                {
                    Console.ReadKey(intercept: true);
                    break;
                }

                Thread.Sleep(50);
            }

            AnsiConsole.Clear();
        }

        public static void PrintHelp(string readmePath = "README.md")
        {
            CliHelpPrinter.PrintHelp(readmePath);
        }
    }
}
