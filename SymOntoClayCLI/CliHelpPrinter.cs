using Spectre.Console;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SymOntoClay.CLI
{
    public static class CliHelpPrinter
    {
        private const string StartMarker = "<!-- CLI-HELP-START -->";
        private const string EndMarker = "<!-- CLI-HELP-END -->";

        public static void PrintHelp(string readmePath = "README.md")
        {
            AnsiConsole.MarkupLine("[bold cyan]Help - SymOntoClay CLI:[/]");
            AnsiConsole.MarkupLine("Running CLI without arguments prints help.\n");

            if (!File.Exists(readmePath))
            {
                AnsiConsole.MarkupLine($"[yellow]README not found at:[/] {Markup.Escape(Path.GetFullPath(readmePath))}");
                return;
            }

            string markdown = File.ReadAllText(readmePath);
            string section = ExtractSection(markdown, StartMarker, EndMarker);

            RenderMarkdownSection(section);
        }

        private static string ExtractSection(string content, string startMarker, string endMarker)
        {
            int startIndex = content.IndexOf(startMarker, StringComparison.Ordinal);
            if (startIndex < 0)
                return content;

            startIndex += startMarker.Length;
            int endIndex = content.IndexOf(endMarker, startIndex, StringComparison.Ordinal);

            return endIndex >= 0
                ? content[startIndex..endIndex].Trim()
                : content[startIndex..].Trim();
        }

        private static void RenderMarkdownSection(string section)
        {
            Table? currentTable = null;

            foreach (string rawLine in section.Split('\n'))
            {
                string line = rawLine.TrimEnd('\r').Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Заголовки секций (## CLI arguments)
                if (line.StartsWith("## "))
                {
                    FlushTable(ref currentTable);
                    string heading = line[3..].Trim();
                    AnsiConsole.MarkupLine($"\n[bold yellow]{Markup.Escape(heading)}:[/]");
                    continue;
                }

                // Элементы списка: * `команда` - описание
                var match = Regex.Match(line, @"^[-*]\s+`([^`]+)`\s*-\s*(.+)$");
                if (match.Success)
                {
                    currentTable ??= CreateHelpTable();

                    string command = FormatCommand(match.Groups[1].Value);
                    string description = FormatDescription(match.Groups[2].Value);

                    currentTable.AddRow(command, description);
                    continue;
                }

                // Любые другие строки внутри секции
                FlushTable(ref currentTable);
                AnsiConsole.MarkupLine(FormatDescription(line));
            }

            FlushTable(ref currentTable);
        }

        private static Table CreateHelpTable()
        {
            return new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Grey)
                .AddColumn(new TableColumn("[bold]Command / Option[/]").NoWrap())
                .AddColumn(new TableColumn("[bold]Description[/]"));
        }

        private static void FlushTable(ref Table? table)
        {
            if (table is null)
                return;

            AnsiConsole.Write(table);
            table = null;
        }

        private static string FormatCommand(string rawCommand)
        {
            // Подсвечиваем <параметры> и безопасно экранируем всё остальное (включая [...])
            string escaped = Markup.Escape(rawCommand);
            return Regex.Replace(
                escaped,
                @"&lt;([^&]+)&gt;",
                "[bold green]&lt;$1&gt;[/]"
            );
        }

        private static string FormatDescription(string rawDescription)
        {
            // Убираем инлайновый markdown-код `...` и безопасно экранируем текст
            string withoutBackticks = rawDescription.Replace("`", string.Empty);
            string escaped = Markup.Escape(withoutBackticks);
            return Regex.Replace(
                escaped,
                @"&lt;([^&]+)&gt;",
                "[italic green]&lt;$1&gt;[/]"
            );
        }
    }
}
