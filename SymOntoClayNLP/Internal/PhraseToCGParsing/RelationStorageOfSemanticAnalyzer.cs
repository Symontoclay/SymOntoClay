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
            var nextN = n + 4;
            var sb = new StringBuilder();
            return sb.ToString();
        }
    }
}
