/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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
