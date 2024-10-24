/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class RuleInstanceHelper
    {
        private static readonly string _logicalVarPattern = @"(\A|\b|\s|\W)(?<var>\$(\w|_|\d)+)(\b|\Z|\s)";
        private static readonly Regex _logicalVarRegex = new Regex(_logicalVarPattern);

        public static List<string> GetUniqueVarNames(string factStr)
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

        public static List<string> GetUniqueVarNames(RuleInstance fact)
        {
            var result = new List<string>();

            GetUniqueVarNames(fact, result);

            result = result.Distinct().ToList();

            return result;
        }

        public static List<string> GetUniqueVarNamesWithPrefix(string prefix, string factStr)
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

        public static List<string> GetUniqueVarNamesWithPrefix(string prefix, RuleInstance fact)
        {
            var result = GetUniqueVarNames(fact);

            if (!result.Any())
            {
                return result;
            }

            result = result.Where(p => p == prefix || p.StartsWith(prefix)).ToList();

            return result;
        }

        public static string GetNewUniqueVarNameWithPrefix(string prefix, string factStr)
        {
            return GetNewUniqueVarNameWithPrefix(prefix, GetUniqueVarNamesWithPrefix(prefix, factStr));
        }

        public static string GetNewUniqueVarNameWithPrefix(string prefix, RuleInstance fact)
        {
            return GetNewUniqueVarNameWithPrefix(prefix, GetUniqueVarNamesWithPrefix(prefix, fact));
        }

        private static string GetNewUniqueVarNameWithPrefix(string prefix, List<string> varNamesList)
        {
            if (!varNamesList.Any())
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

        private static void GetUniqueVarNames(RuleInstance source, List<string> result)
        {
            if(source.PrimaryPart != null)
            {
                GetUniqueVarNames(source.PrimaryPart, result);
            }

            if (!source.SecondaryParts.IsNullOrEmpty())
            {
                foreach (var secondaryPart in source.SecondaryParts)
                {
                    GetUniqueVarNames(secondaryPart, result);
                }
            }
        }

        private static void GetUniqueVarNames(BaseRulePart source, List<string> result)
        {
            GetUniqueVarNames(source.Expression, result);
        }

        private static void GetUniqueVarNames(LogicalQueryNode source, List<string> result)
        {
            var linkedVars = source.LinkedVars;

            if(!linkedVars.IsNullOrEmpty())
            {
                foreach(var item in linkedVars)
                {
                    result.Add(item.Name.NameValue);
                }
            }

            var kind = source.Kind;

            switch(kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    GetUniqueVarNames(source.Left, result);
                    GetUniqueVarNames(source.Right, result);
                    break;

                case KindOfLogicalQueryNode.UnaryOperator:
                case KindOfLogicalQueryNode.Group:
                    GetUniqueVarNames(source.Left, result);
                    break;

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.Value:
                case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                    break;

                case KindOfLogicalQueryNode.LogicalVar:
                    result.Add(source.Name.NameValue);
                    break;

                case KindOfLogicalQueryNode.Fact:
                    GetUniqueVarNames(source.Fact, result);
                    break;

                case KindOfLogicalQueryNode.Relation:
                    foreach(var item in source.ParamsList)
                    {
                        GetUniqueVarNames(item, result);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}
