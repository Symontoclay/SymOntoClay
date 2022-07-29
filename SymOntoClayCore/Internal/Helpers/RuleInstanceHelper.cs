using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class RuleInstanceHelper
    {
        private static readonly string _logicalVarPattern = @"(\A|\b|\s|\W)(?<var>\$(\w|_|\d)+)(\b|\Z|\s)";
        private static readonly Regex _logicalVarRegex = new Regex(_logicalVarPattern);

        public static List<string> GetUniqueVarNamesFromFactString(string factStr)
        {
            var result = new List<string>();

            MatchCollection matchesList = _logicalVarRegex.Matches(factStr);

            foreach (Match match in matchesList)
            {
                if (!match.Success)
                {
                    break;
                }

                GroupCollection groups = match.Groups;

                var gItem = groups["var"];

                result.Add(gItem.Value);
            }

            result = result.Distinct().ToList();

            return result;
        }

        public static List<string> GetUniqueVarNamesWithPrefixFromFactString(string prefix, string factStr)
        {
            var result = new List<string>();

            var pattern = $@"(\A|\b|\s|\W)(?<var>\{prefix}(\d)*)(\b|\Z|\s)";

            var regex = new Regex(pattern);

            MatchCollection matchesList = regex.Matches(factStr);

            foreach (Match match in matchesList)
            {
                if (!match.Success)
                {
                    break;
                }

                GroupCollection groups = match.Groups;

                var gItem = groups["var"];

                result.Add(gItem.Value);
            }

            result = result.Distinct().ToList();

            return result;
        }

        public static string GetNewUniqueVarNameWithPrefixFromFactString(string prefix, string factStr)
        {
            var varNamesList = GetUniqueVarNamesWithPrefixFromFactString(prefix, factStr);

            if(!varNamesList.Any())
            {
                return prefix;
            }

            varNamesList = varNamesList.Select(p => p.Substring(2)).Where(p => string.IsNullOrWhiteSpace(p) || p.Any(x => char.IsDigit(x))).ToList();

            if (!varNamesList.Any())
            {
                return prefix;
            }

            var targetIndex = varNamesList.Select(p => string.IsNullOrWhiteSpace(p) ? 0 : int.Parse(p)).Max() + 1;

            var targetVarName = $"{prefix}{targetIndex}";

            return targetVarName;
        }
    }
}
