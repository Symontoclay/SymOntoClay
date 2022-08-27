/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseToCGParsing
{
    public class RelationStorageOfSemanticAnalyzer : IObjectToString
    {
        private Dictionary<string, Dictionary<string, List<string>>> _infoDict = new Dictionary<string, Dictionary<string, List<string>>>();
        private Dictionary<string, List<string>> _infoForSingleRelationDict = new Dictionary<string, List<string>>();

        /// <summary>
        /// inputConcept -> relationName -> outputConcept
        /// </summary>
        /// <param name="inputConcept"></param>
        /// <param name="outputConcept"></param>
        /// <param name="relationName"></param>
        public void AddRelation(string inputConcept, string outputConcept, string relationName)
        {
            if (_infoDict.ContainsKey(inputConcept))
            {
                var targetDict = _infoDict[inputConcept];

                if (targetDict.ContainsKey(outputConcept))
                {
                    var targetList = targetDict[outputConcept];

                    if (!targetList.Contains(relationName))
                    {
                        targetList.Add(relationName);
                    }

                    return;
                }

                {
                    var targetList = new List<string>() { relationName };
                    targetDict[outputConcept] = targetList;
                }
                return;
            }

            {
                var targetList = new List<string>() { relationName };
                var targetDict = new Dictionary<string, List<string>>();
                targetDict[outputConcept] = targetList;
                _infoDict[inputConcept] = targetDict;
            }
        }

        /// <summary>
        /// relationName -> outputConcept
        /// </summary>
        /// <param name="outputConcept"></param>
        /// <param name="relationName"></param>
        public void AddRelation(string outputConcept, string relationName)
        {
            if (_infoForSingleRelationDict.ContainsKey(outputConcept))
            {
                var targetList = _infoForSingleRelationDict[outputConcept];

                if (!targetList.Contains(relationName))
                {
                    targetList.Add(relationName);
                }

                return;
            }

            {
                var targetList = new List<string>() { relationName };
                _infoForSingleRelationDict[outputConcept] = targetList;
            }
        }

        /// <summary>
        /// inputConcept -> relationName -> outputConcept
        /// </summary>
        /// <param name="inputConcept"></param>
        /// <param name="outputConcept"></param>
        /// <param name="relationName"></param>
        /// <returns></returns>
        public bool ContainsRelation(string inputConcept, string outputConcept, string relationName)
        {
            if (_infoDict.ContainsKey(inputConcept))
            {
                var targetDict = _infoDict[inputConcept];

                if (targetDict.ContainsKey(outputConcept))
                {
                    var targetList = targetDict[outputConcept];
                    return targetList.Contains(relationName);
                }
            }

            return false;
        }

        /// <summary>
        /// relationName -> outputConcept
        /// </summary>
        /// <param name="outputConcept"></param>
        /// <param name="relationName"></param>
        /// <returns></returns>
        public bool ContainsRelation(string outputConcept, string relationName)
        {
            if (_infoForSingleRelationDict.ContainsKey(outputConcept))
            {
                var targetList = _infoForSingleRelationDict[outputConcept];
                return targetList.Contains(relationName);
            }

            return false;
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
            var sb = new StringBuilder();
            return sb.ToString();
        }
    }
}
