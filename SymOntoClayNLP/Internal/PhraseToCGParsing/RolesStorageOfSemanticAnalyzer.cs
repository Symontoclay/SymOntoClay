/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.CG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseToCGParsing
{
    public class RolesStorageOfSemanticAnalyzer : IObjectToString
    {
        private Dictionary<string, List<ConceptCGNode>> mRolesDict = new Dictionary<string, List<ConceptCGNode>>();

        public void Add(string role, ConceptCGNode concept)
        {
            if (mRolesDict.ContainsKey(role))
            {
                var itemsList = mRolesDict[role];
                if (itemsList.Contains(concept))
                {
                    return;
                }
                itemsList.Add(concept);
                return;
            }

            var newItemsList = new List<ConceptCGNode>();
            newItemsList.Add(concept);
            mRolesDict[role] = newItemsList;
        }

        public List<string> Roles
        {
            get
            {
                return mRolesDict.Keys.ToList();
            }
        }

        public int Count
        {
            get
            {
                return mRolesDict.Count;
            }
        }

        public void Assing(RolesStorageOfSemanticAnalyzer source)
        {
            if (source.Count == 0)
            {
                return;
            }

            foreach (var rolesDictKVPItem in source.mRolesDict)
            {
                var role = rolesDictKVPItem.Key;

                foreach (var extendedToken in rolesDictKVPItem.Value)
                {
                    Add(role, extendedToken);
                }
            }
        }

        public List<ConceptCGNode> GetByRole(string role)
        {
            if (mRolesDict.ContainsKey(role))
            {
                return mRolesDict[role];
            }

            return null;
        }

        public override string ToString()
        {
            return ToString(0u);
        }

        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        public string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var nextNextN = nextN + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();
            if (mRolesDict == null)
            {
                sb.AppendLine($"{spaces}{nameof(mRolesDict)} = null");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(mRolesDict)}");
                foreach (var role in mRolesDict)
                {
                    sb.AppendLine($"{nextNSpaces}{nameof(role)} = {role.Key}");
                    var itemsOfRole = role.Value;
                    sb.AppendLine($"{spaces}Begin {nameof(itemsOfRole)}");
                    foreach (var itemOfRole in itemsOfRole)
                    {
                        sb.Append(itemOfRole.ToBriefString(nextNextN));
                    }
                    sb.AppendLine($"{spaces}End {nameof(itemsOfRole)}");
                }
                sb.AppendLine($"{spaces}End {nameof(mRolesDict)}");
            }
            return sb.ToString();
        }
    }
}
