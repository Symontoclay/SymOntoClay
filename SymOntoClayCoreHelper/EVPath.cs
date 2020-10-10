using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SymOntoClay.CoreHelper
{
    public static class EVPath
    {
        private static Regex _normalizeMatch = new Regex("(%(\\w|\\(|\\))+%)");
        private static Regex _normalizeMatch2 = new Regex("(\\w|\\(|\\))+");

        public static string Normalize(string sourcePath)
        {
            var match = _normalizeMatch.Match(sourcePath);

            if (match.Success)
            {
                var targetValue = match.Value;

                var match2 = _normalizeMatch2.Match(targetValue);

                if (match2.Success)
                {
                    var variableName = match2.Value;

                    var variableValue = string.Empty;

                    if(_additionalVariablesDict.ContainsKey(variableName))
                    {
                        variableValue = _additionalVariablesDict[variableName];
                    }
                    else
                    {
                        variableValue = Environment.GetEnvironmentVariable(variableName);
                    }

                    if (!string.IsNullOrWhiteSpace(variableValue))
                    {
                        sourcePath = sourcePath.Replace(targetValue, variableValue);
                    }
                }
            }

            return Path.GetFullPath(sourcePath);
        }

        public static void RegVar(string varName, string varValue)
        {
            _additionalVariablesDict[varName] = varValue;
        }

        private static readonly Dictionary<string, string> _additionalVariablesDict = new Dictionary<string, string>();
    }
}
