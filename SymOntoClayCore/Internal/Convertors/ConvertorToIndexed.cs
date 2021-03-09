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

using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace SymOntoClay.Core.Internal.Convertors
{
    [Obsolete("IndexedData must be removed!", true)]
    public static class ConvertorToIndexed
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif
    
//        public static IndexedRuleInstance ConvertRuleInstance(RuleInstance source, IMainStorageContext mainStorageContext)
//        {
//            var convertingContext = new Dictionary<object, object>();

//            var result = ConvertRuleInstance(source, mainStorageContext, convertingContext);

//#if DEBUG
//            //_gbcLogger.Info($"result = {result}");
//#endif

//            return result;
//        }

//        public static IndexedRuleInstance ConvertRuleInstance(RuleInstance source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
//        {
//#if DEBUG
//            //_gbcLogger.Info($"source = {source}");
//#endif

//            if (source == null)
//            {
//                return null;
//            }

//            if (convertingContext.ContainsKey(source))
//            {
//                return (IndexedRuleInstance)convertingContext[source];
//            }

//#if DEBUG
//            //_gbcLogger.Info("NEXT");
//#endif

//            //var dictionary = mainStorageContext.Dictionary;

//            var result = new IndexedRuleInstance();
//            convertingContext[source] = result;
//            result.Origin = source;
//            source.Indexed = result;

//            //FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

//            result.Kind = source.Kind;

//            //result.Name = ConvertStrongIdentifierValue(source.Name, mainStorageContext, convertingContext);

//            //result.Key = dictionary.GetKey(source.Name.NameValue);

//            result.PrimaryPart = ConvertPrimaryRulePart(source.PrimaryPart, result, mainStorageContext, convertingContext);

//            if (!source.SecondaryParts.IsNullOrEmpty())
//            {
//                foreach (var secondaryPart in source.SecondaryParts)
//                {
//                    result.SecondaryParts.Add(ConvertSecondaryRulePart(secondaryPart, result, mainStorageContext, convertingContext));
//                }
//            }

//#if DEBUG
//            //_gbcLogger.Info($"result (snapshot) = {result}");
//#endif

//            result.CalculateUsedKeys();
//            //result.CalculateLongHashCodes();

//            return result;
//        }

//        private static IndexedPrimaryRulePart ConvertPrimaryRulePart(PrimaryRulePart source, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
//        {
//#if DEBUG
//            //_gbcLogger.Info($"source = {source}");
//#endif

//            if (source == null)
//            {
//                return null;
//            }

//            if (convertingContext.ContainsKey(source))
//            {
//                return (IndexedPrimaryRulePart)convertingContext[source];
//            }

//#if DEBUG
//            //_gbcLogger.Info("NEXT");
//#endif

//            var result = new IndexedPrimaryRulePart();
//            convertingContext[source] = result;
//            result.OriginPrimaryRulePart = source;
//            source.Indexed = result;

//            //FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

//            FillBaseRulePart(source, result, ruleInstance, mainStorageContext, convertingContext);

//            if(!source.SecondaryParts.IsNullOrEmpty())
//            {
//                foreach(var secondaryPart in source.SecondaryParts)
//                {
//                    result.SecondaryParts.Add(ConvertSecondaryRulePart(secondaryPart, ruleInstance, mainStorageContext, convertingContext));
//                }              
//            }

//#if DEBUG
//            //_gbcLogger.Info($"result (snapshot) = {result}");
//#endif

//            //result.CalculateLongHashCodes();

//            return result;
//        }

//        private static IndexedSecondaryRulePart ConvertSecondaryRulePart(SecondaryRulePart source, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
//        {
//#if DEBUG
//            //_gbcLogger.Info($"source = {source}");
//#endif

//            if (source == null)
//            {
//                return null;
//            }

//            if (convertingContext.ContainsKey(source))
//            {
//                return (IndexedSecondaryRulePart)convertingContext[source];
//            }

//            var result = new IndexedSecondaryRulePart();
//            convertingContext[source] = result;
//            result.OriginalSecondaryRulePart = source;
//            source.Indexed = result;

//            //FillAnnotationsModalitiesAndSections(source, result, mainStorageContext, convertingContext);

//            FillBaseRulePart(source, result, ruleInstance, mainStorageContext, convertingContext);

//            result.PrimaryPart = ConvertPrimaryRulePart(source.PrimaryPart, ruleInstance, mainStorageContext, convertingContext);

//#if DEBUG
//            //_gbcLogger.Info($"result (snapshot) = {result}");
//#endif

//            //result.CalculateLongHashCodes();

//            return result;
//        }

        //private static void FillBaseRulePart(BaseRulePart source, IndexedBaseRulePart dest, IndexedRuleInstance ruleInstance, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        //{
        //    var contextOfConvertingExpressionNode = new ContextOfConvertingExpressionNode();

        //    dest.Parent = (IndexedRuleInstance)convertingContext[source.Parent];

        //    dest.Expression = ConvertLogicalQueryNode(source.Expression, dest, ruleInstance, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

        //    dest.RelationsDict = contextOfConvertingExpressionNode.RelationsList.GroupBy(p => p.Key).ToDictionary(p => p.Key, p => (IList<RelationIndexedLogicalQueryNode>)p.ToList());

        //    dest.IsActive = source.IsActive;
        //    dest.HasQuestionVars = contextOfConvertingExpressionNode.HasQuestionVars;
        //    dest.HasVars = contextOfConvertingExpressionNode.HasVars;
        //}

        //private static BaseIndexedLogicalQueryNode ConvertLogicalQueryNode(LogicalQueryNode source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext, ContextOfConvertingExpressionNode contextOfConvertingExpressionNode)
        //{
        //    switch(source.Kind)
        //    {
        //        case KindOfLogicalQueryNode.BinaryOperator:
        //            switch(source.KindOfOperator)
        //            {
        //                case KindOfOperatorOfLogicalQueryNode.Is:
        //                    return ConvertIsOperatorLogicalQueryNode(source, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

        //                default:
        //                    throw new ArgumentOutOfRangeException(nameof(source.KindOfOperator), source.KindOfOperator, null);
        //            }

        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(source.Kind), source.Kind, null);
        //    }
        //}
        
//        private static BaseIndexedLogicalQueryNode ConvertIsOperatorLogicalQueryNode(LogicalQueryNode source, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext, ContextOfConvertingExpressionNode contextOfConvertingExpressionNode)
//        {
//#if DEBUG
//            //_gbcLogger.Info($"source = {source}");
//#endif

//            if (source == null)
//            {
//                return null;
//            }

//            if (convertingContext.ContainsKey(source))
//            {
//                return (BaseIndexedLogicalQueryNode)convertingContext[source];
//            }

//            var leftNode = source.Left;
//            var rightNode = source.Right;

//#if DEBUG
//            //_gbcLogger.Info($"leftNode = {leftNode}");
//            //_gbcLogger.Info($"rightNode = {rightNode}");
//#endif

//            var result = new RelationIndexedLogicalQueryNode();
//            convertingContext[source] = result;

//            //FillBaseIndexedLogicalQueryNode(result, source, rulePart, ruleInstance, mainStorageContext, convertingContext);

//            var name = rightNode.Name;

//            //result.Key = mainStorageContext.Dictionary.GetKey(name.NameValue);

//            if(name.KindOfName == KindOfName.QuestionVar)
//            {
//                result.IsQuestion = true;
//            }

//            var sourceParamsList = new List<LogicalQueryNode>() { leftNode };

//            FillRelationParams(result, sourceParamsList, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

//#if DEBUG
//            //_gbcLogger.Info($"result = {result}");
//#endif

//            //result.CalculateLongHashCodes();

//            return result;
//        }

//        private static void FillRelationParams(RelationIndexedLogicalQueryNode dest, IList<LogicalQueryNode> sourceParamsList, IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext, ContextOfConvertingExpressionNode contextOfConvertingExpressionNode)
//        {
//            dest.CountParams = sourceParamsList.Count;

//            var parametersList = new List<BaseIndexedLogicalQueryNode>();
//            var varsInfoList = new List<QueryExecutingCardAboutVar>();
//            var knownInfoList = new List<QueryExecutingCardAboutKnownInfo>();
//            var i = 0;

//            //var dictionary = mainStorageContext.Dictionary;

//            foreach (var param in sourceParamsList)
//            {
//#if DEBUG
//                //_gbcLogger.Info($"param = {param}");
//#endif

//                var resultParam = ConvertLogicalQueryNode(param, mainStorageContext, convertingContext, contextOfConvertingExpressionNode);

//#if DEBUG
//                //_gbcLogger.Info($"resultParam = {resultParam}");
//#endif

//                parametersList.Add(resultParam);
//                var kindOfParam = param.Kind;
//                switch (kindOfParam)
//                {
//                    case KindOfLogicalQueryNode.Concept:
//                        {
//                            var knownInfo = new QueryExecutingCardAboutKnownInfo();
//                            knownInfo.Kind = kindOfParam;
//                            knownInfo.Expression = param;
//                            knownInfo.Position = i;
//                            knownInfo.Name = param.Name;
//                            knownInfoList.Add(knownInfo);
//                        }
//                        break;

//                    case KindOfLogicalQueryNode.Entity:
//                        {
//                            var knownInfo = new QueryExecutingCardAboutKnownInfo();
//                            knownInfo.Kind = kindOfParam;
//                            knownInfo.Expression = param;
//                            knownInfo.Position = i;
//                            knownInfo.Name = param.Name;
//                            knownInfoList.Add(knownInfo);
//                        }
//                        break;

//                    //case KindOfLogicalQueryNode.EntityRef:
//                    //    {
//                    //        var originParam = param.AsEntityRef;
//                    //        var knownInfo = new QueryExecutingCardAboutKnownInfo();
//                    //        knownInfo.Kind = kindOfParam;
//                    //        knownInfo.Expression = param;
//                    //        knownInfo.Position = i;
//                    //        knownInfo.Key = originParam.Key;
//                    //        knownInfoList.Add(knownInfo);
//                    //    }
//                    //    break;

//                    //case KindOfLogicalQueryNode.EntityCondition:
//                    //    {
//                    //        var originParam = param.AsEntityCondition;
//                    //        var knownInfo = new QueryExecutingCardAboutKnownInfo();
//                    //        knownInfo.Kind = kindOfParam;
//                    //        knownInfo.Expression = param;
//                    //        knownInfo.Position = i;
//                    //        knownInfo.Key = originParam.Key;
//                    //        knownInfoList.Add(knownInfo);
//                    //    }
//                    //    break;

//                    case KindOfLogicalQueryNode.LogicalVar:
//                        {
//                            var originParam = param;
//                            var varInfo = new QueryExecutingCardAboutVar();
//                            varInfo.NameOfVar = originParam.Name;
//                            varInfo.Position = i;
//                            varsInfoList.Add(varInfo);
//                        }
//                        break;

//                    case KindOfLogicalQueryNode.QuestionVar:
//                        {
//                            var originParam = param;
//                            var varInfo = new QueryExecutingCardAboutVar();
//                            varInfo.NameOfVar = originParam.Name;
//                            varInfo.Position = i;
//                            varsInfoList.Add(varInfo);
//                        }
//                        break;

//                    case KindOfLogicalQueryNode.Value:
//                        {
//                            var originParam = param;
//                            var knownInfo = new QueryExecutingCardAboutKnownInfo();
//                            knownInfo.Kind = kindOfParam;
//                            knownInfo.Expression = param;
//                            knownInfo.Position = i;
//                            knownInfo.Value = originParam.Value;
//                            knownInfoList.Add(knownInfo);

//#if DEBUG
//                            _gbcLogger.Info($"knownInfo = {knownInfo}");
//#endif
//                        }
//                        break;

//                    //case KindOfLogicalQueryNode.FuzzyLogicValue:
//                    //    {
//                    //        var originParam = param.AsFuzzyLogicValue;
//                    //        var knownInfo = new QueryExecutingCardAboutKnownInfo();
//                    //        knownInfo.Kind = kindOfParam;
//                    //        knownInfo.Expression = param;
//                    //        knownInfo.Position = i;
//                    //        knownInfo.Value = originParam.Value;
//                    //        knownInfoList.Add(knownInfo);
//                    //    }
//                    //    break;

//                    //case KindOfLogicalQueryNode.Fact:
//                    //    {
//                    //        var originParam = param.AsFact;
//                    //        var knownInfo = new QueryExecutingCardAboutKnownInfo();
//                    //        knownInfo.Kind = kindOfParam;
//                    //        knownInfo.Expression = param;
//                    //        knownInfo.Position = i;
//                    //        knownInfo.Key = originParam.Key;
//                    //        knownInfoList.Add(knownInfo);
//                    //    }
//                    //    break;

//                    default:
//                        throw new ArgumentOutOfRangeException(nameof(kindOfParam), kindOfParam, null);
//                }
//                i++;
//            }
//            dest.Params = parametersList;
//            dest.VarsInfoList = varsInfoList;
//            dest.KnownInfoList = knownInfoList;

//            contextOfConvertingExpressionNode.RelationsList.Add(dest);
//        }
    }
}
