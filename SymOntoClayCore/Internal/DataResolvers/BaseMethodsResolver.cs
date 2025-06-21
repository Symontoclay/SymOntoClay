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

using Newtonsoft.Json.Linq;
using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public abstract class BaseMethodsResolver : BaseResolver
    {
        protected BaseMethodsResolver(IMainStorageContext context)
            : base(context)
        {
            
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            var dataResolversFactory = _context.DataResolversFactory;

            _typeConverter = _context.TypeConverter;

            _synonymsResolver = dataResolversFactory.GetSynonymsResolver();

            var commonNamesStorage = _context.CommonNamesStorage;
            _fuzzyTypeName = commonNamesStorage.FuzzyTypeName;
            _numberTypeName = commonNamesStorage.NumberTypeName;
        }

        protected SynonymsResolver _synonymsResolver;
        private ITypeConverter _typeConverter;

        private StrongIdentifierValue _fuzzyTypeName;
        private StrongIdentifierValue _numberTypeName;

        protected List<WeightedInheritanceResultItemWithStorageInfo<T>> FilterByTypeOfParameters<T>(IMonitorLogger logger, List<WeightedInheritanceResultItemWithStorageInfo<T>> source, Dictionary<StrongIdentifierValue, Value> namedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
            where T : AnnotatedItem, IExecutable
        {
            var result = new List<WeightedInheritanceResultItemWithStorageInfo<T>>();

            foreach (var function in source)
            {
                var rankMatrix = IsFit(logger, function.ResultItem, namedParameters, localCodeExecutionContext, options);

                if (rankMatrix == null)
                {
                    continue;
                }

                function.ParametersRankMatrix = rankMatrix;

                result.Add(function);
            }

            return result;
        }

        protected List<WeightedInheritanceResultItemWithStorageInfo<T>> FilterByTypeOfParameters<T>(IMonitorLogger logger, List<WeightedInheritanceResultItemWithStorageInfo<T>> source, List<Value> positionedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
            where T : AnnotatedItem, IExecutable
        {
            var result = new List<WeightedInheritanceResultItemWithStorageInfo<T>>();

            foreach (var function in source)
            {
                var rankMatrix = IsFit(logger, function.ResultItem, positionedParameters, localCodeExecutionContext, options);

#if DEBUG
                //Info("D943BB34-839F-4FAE-901B-7ECABF137126", $"rankMatrix = {rankMatrix.WritePODListToString()}");
#endif

                if (rankMatrix == null)
                {
                    continue;
                }

                function.ParametersRankMatrix = rankMatrix;

                result.Add(function);
            }

            return result;
        }

        protected List<ParameterRank> IsFit(IMonitorLogger logger, IExecutable function, IDictionary<StrongIdentifierValue, Value> namedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var result = new List<ParameterRank>();

            var countOfUsedParameters = 0;

            foreach (var argument in function.Arguments)
            {
                var argumentName = argument.Name;

                var parameterValue = GetParameterValue(logger, argumentName, namedParameters, localCodeExecutionContext);

                if (parameterValue == null)
                {
                    if (argument.HasDefaultValue)
                    {
                        result.Add(new ParameterRank { Distance = 0u, NeedTypeConversion = false });

                        continue;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    countOfUsedParameters++;

                    var distance = _inheritanceResolver.GetDistance(logger, argument.TypesList, parameterValue, localCodeExecutionContext, options);

                    if (distance.HasValue)
                    {
                        result.Add(new ParameterRank { Distance = distance.Value, NeedTypeConversion = false });
                        continue;
                    }

                    var checkResult = _typeConverter.CheckFitValue(logger, parameterValue, argument.TypesList, localCodeExecutionContext, options);

                    if (checkResult.KindOfResult == KindOfTypeFitCheckingResult.IsNotFit)
                    {
                        return null;
                    }

                    result.Add(new ParameterRank { Distance = 0u, NeedTypeConversion = true });
                }
            }

            if (countOfUsedParameters < namedParameters.Count)
            {
                return null;
            }

            return result;
        }

        protected List<ParameterRank> IsFit(IMonitorLogger logger, IExecutable function, IList<Value> positionedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var positionedParametersEnumerator = positionedParameters.GetEnumerator();

            var result = new List<ParameterRank>();

            foreach (var argument in function.Arguments)
            {
                if (!positionedParametersEnumerator.MoveNext())
                {
                    if (argument.HasDefaultValue)
                    {
                        result.Add(new ParameterRank { Distance = 0u, NeedTypeConversion = false }); 

                        continue;
                    }

                    throw new NotImplementedException("3A6D5F5E-E72A-4D8D-9733-12203259B3CE");
                }

                var parameterItem = positionedParametersEnumerator.Current;

#if DEBUG
                //Info("BFF51E62-6521-49A9-867D-EDA9FFC3E4B8", $"argument.TypesList = {argument.TypesList.WritePODListToString()}");
                //Info("7277A1B2-6C59-47CD-9292-40FA8B5DB7FD", $"parameterItem = {parameterItem.ToHumanizedLabel()}");
#endif

                var distance = _inheritanceResolver.GetDistance(logger, argument.TypesList, parameterItem, localCodeExecutionContext, options);

#if DEBUG
                //Info("BDEAEF0A-D02A-4B73-82DF-D5863078759C", $"distance = {distance}");
#endif

                if(distance.HasValue)
                {
                    result.Add(new ParameterRank { Distance = distance.Value, NeedTypeConversion = false });
                    continue;
                }

                var checkResult = _typeConverter.CheckFitValue(logger, parameterItem, argument.TypesList, localCodeExecutionContext, options);

#if DEBUG
                //Info("2C479143-0443-474F-8575-01EA17BB0637", $"checkResult = {checkResult}");
#endif

                if (checkResult.KindOfResult == KindOfTypeFitCheckingResult.IsNotFit)
                {
                    return null;
                }

                result.Add(new ParameterRank { Distance = 0u, NeedTypeConversion = true });
            }

            return result;
        }

        protected Value GetParameterValue(IMonitorLogger logger, StrongIdentifierValue argumentName, IDictionary<StrongIdentifierValue, Value> namedParameters, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            if (namedParameters.ContainsKey(argumentName))
            {
                return namedParameters[argumentName];
            }

            var synonymsList = _synonymsResolver.GetSynonyms(logger, argumentName, localCodeExecutionContext);

            foreach (var synonym in synonymsList)
            {
                if (namedParameters.ContainsKey(synonym))
                {
                    return namedParameters[synonym];
                }

                var alternativeSynonym = NameHelper.CreateAlternativeArgumentName(synonym, logger);

                if (namedParameters.ContainsKey(alternativeSynonym))
                {
                    return namedParameters[alternativeSynonym];
                }
            }

            var alternativeArgumentName = NameHelper.CreateAlternativeArgumentName(argumentName, logger);

            synonymsList = _synonymsResolver.GetSynonyms(logger, alternativeArgumentName, localCodeExecutionContext);

            foreach (var synonym in synonymsList)
            {
                if (namedParameters.ContainsKey(synonym))
                {
                    return namedParameters[synonym];
                }

                var alternativeSynonym = NameHelper.CreateAlternativeArgumentName(synonym, logger);

                if (namedParameters.ContainsKey(alternativeSynonym))
                {
                    return namedParameters[alternativeSynonym];
                }
            }

            return null;
        }

        protected WeightedInheritanceResultItemWithStorageInfo<T> GetTargetValueFromList<T>(IMonitorLogger logger, List<WeightedInheritanceResultItemWithStorageInfo<T>> source, int paramsCount, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
            where T : AnnotatedItem, IExecutable
        {
            CorrectParametersRankMatrixForSpecialCases(logger, source);

            IOrderedEnumerable<WeightedInheritanceResultItemWithStorageInfo<T>> orderedList = null;

            if (paramsCount > 0)
            {
                for (var i = 0; i < paramsCount; i++)
                {
                    switch (i)
                    {
                        case 0:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[0].Distance).ThenBy(p => p.ParametersRankMatrix[0].NeedTypeConversion);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[0].Distance).ThenBy(p => p.ParametersRankMatrix[0].NeedTypeConversion);
                            }
                            break;

                        case 1:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[1].Distance).ThenBy(p => p.ParametersRankMatrix[1].NeedTypeConversion);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[1].Distance).ThenBy(p => p.ParametersRankMatrix[1].NeedTypeConversion);
                            }
                            break;

                        case 2:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[2].Distance).ThenBy(p => p.ParametersRankMatrix[2].NeedTypeConversion);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[2].Distance).ThenBy(p => p.ParametersRankMatrix[2].NeedTypeConversion);
                            }
                            break;

                        case 3:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[3].Distance).ThenBy(p => p.ParametersRankMatrix[3].NeedTypeConversion);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[3].Distance).ThenBy(p => p.ParametersRankMatrix[3].NeedTypeConversion);
                            }
                            break;

                        case 4:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[4].Distance).ThenBy(p => p.ParametersRankMatrix[4].NeedTypeConversion);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[4].Distance).ThenBy(p => p.ParametersRankMatrix[4].NeedTypeConversion);
                            }
                            break;

                        case 5:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[5].Distance).ThenBy(p => p.ParametersRankMatrix[5].NeedTypeConversion);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[5].Distance).ThenBy(p => p.ParametersRankMatrix[5].NeedTypeConversion);
                            }
                            break;

                        case 6:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[6].Distance).ThenBy(p => p.ParametersRankMatrix[6].NeedTypeConversion);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[6].Distance).ThenBy(p => p.ParametersRankMatrix[6].NeedTypeConversion);
                            }
                            break;

                        case 7:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[7].Distance).ThenBy(p => p.ParametersRankMatrix[7].NeedTypeConversion);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[7].Distance).ThenBy(p => p.ParametersRankMatrix[7].NeedTypeConversion);
                            }
                            break;

                        case 8:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[8].Distance).ThenBy(p => p.ParametersRankMatrix[8].NeedTypeConversion);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[8].Distance).ThenBy(p => p.ParametersRankMatrix[8].NeedTypeConversion);
                            }
                            break;

                        case 9:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[9].Distance).ThenBy(p => p.ParametersRankMatrix[9].NeedTypeConversion);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[9].Distance).ThenBy(p => p.ParametersRankMatrix[9].NeedTypeConversion);
                            }
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(i), i, null);
                    }
                }
            }

            if (orderedList == null)
            {
                orderedList = source.OrderByDescending(p => p.IsSelf);
            }
            else
            {
                orderedList = orderedList.ThenByDescending(p => p.IsSelf);
            }

            orderedList = orderedList.ThenBy(p => p.Distance).ThenByDescending(p => p.ResultItem.CodeItem.TypeOfAccess == TypeOfAccess.Local).ThenBy(p => p.StorageDistance);

            return orderedList.FirstOrDefault();
        }

        protected void CorrectParametersRankMatrixForSpecialCases<T>(IMonitorLogger logger, List<WeightedInheritanceResultItemWithStorageInfo<T>> source)
            where T : AnnotatedItem, IExecutable
        {
            var advicesDict = CheckSpecialCasesInParameters(logger, source);

            if (advicesDict.Count == 0)
            {
                return;
            }

            foreach (var function in source)
            {
                var parametersRankMatrix = function.ParametersRankMatrix;
                var argumentsList = function.ResultItem.Arguments;

                var argumentsDict = ConvertArgumentsListToDictByPosition(logger, argumentsList);

                foreach (var advice in advicesDict)
                {
                    var position = advice.Key;
                    var checkedTypeName = advice.Value;

                    var argument = argumentsDict[position];

                    var typesList = argument.TypesList;

                    if (typesList.IsNullOrEmpty())
                    {
                        continue;
                    }

                    if (typesList.Contains(checkedTypeName))
                    {
                        continue;
                    }

                    if (checkedTypeName == _fuzzyTypeName)
                    {
                        if (typesList.Contains(_numberTypeName))
                        {
                            parametersRankMatrix[position].Distance++;

                            continue;
                        }
                    }
                    else
                    {
                        throw new NotImplementedException("5B93D0DE-4DD7-4D54-B3CC-E03CF6725EE6");
                    }
                }
            }
        }

        protected Dictionary<int, IFunctionArgument> ConvertArgumentsListToDictByPosition(IMonitorLogger logger, IList<IFunctionArgument> argumentsList)
        {
            var result = new Dictionary<int, IFunctionArgument>();

            var i = 0;

            foreach (var argument in argumentsList)
            {
                result[i] = argument;

                i++;
            }

            return result;
        }

        protected Dictionary<int, StrongIdentifierValue> CheckSpecialCasesInParameters<T>(IMonitorLogger logger, List<WeightedInheritanceResultItemWithStorageInfo<T>> source)
            where T : AnnotatedItem, IExecutable
        {
            var result = new Dictionary<int, StrongIdentifierValue>();

            foreach (var function in source)
            {
                var argumentsList = function.ResultItem.Arguments;

                if (argumentsList.IsNullOrEmpty())
                {
                    continue;
                }

                var i = -1;

                foreach (var argument in argumentsList)
                {
                    i++;
                    var typesList = argument.TypesList;

                    if (typesList.IsNullOrEmpty())
                    {
                        continue;
                    }

                    if (typesList.Contains(_fuzzyTypeName))
                    {
                        result[i] = _fuzzyTypeName;
                    }
                }
            }

            return result;
        }

    }
}
