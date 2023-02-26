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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public abstract class BaseMethodsResolver : BaseResolver
    {
        protected BaseMethodsResolver(IMainStorageContext context)
            : base(context)
        {
            var dataResolversFactory = context.DataResolversFactory;

            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();
            _synonymsResolver = dataResolversFactory.GetSynonymsResolver();
        }

        protected readonly InheritanceResolver _inheritanceResolver;
        protected readonly SynonymsResolver _synonymsResolver;

        private static readonly StrongIdentifierValue _fuzzyTypeIdentifier = NameHelper.CreateName(StandardNamesConstants.FuzzyTypeName);
        private static readonly StrongIdentifierValue _numberTypeIdentifier = NameHelper.CreateName(StandardNamesConstants.NumberTypeName);

        protected List<WeightedInheritanceResultItemWithStorageInfo<T>> FilterByTypeOfParameters<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source, Dictionary<StrongIdentifierValue, Value> namedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
            where T : AnnotatedItem, IExecutable
        {
            var result = new List<WeightedInheritanceResultItemWithStorageInfo<T>>();

            foreach (var function in source)
            {
                var rankMatrix = IsFit(function.ResultItem, namedParameters, localCodeExecutionContext, options);

#if DEBUG
                //Log($"rankMatrix = {rankMatrix.WritePODListToString()}");
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

        protected List<WeightedInheritanceResultItemWithStorageInfo<T>> FilterByTypeOfParameters<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source, List<Value> positionedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
            where T : AnnotatedItem, IExecutable
        {
            var result = new List<WeightedInheritanceResultItemWithStorageInfo<T>>();

            foreach (var function in source)
            {
                var rankMatrix = IsFit(function.ResultItem, positionedParameters, localCodeExecutionContext, options);

#if DEBUG
                //Log($"rankMatrix = {rankMatrix.WritePODListToString()}");
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

        protected List<uint> IsFit(IExecutable function, IDictionary<StrongIdentifierValue, Value> namedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
#endif

            var result = new List<uint>();

            var countOfUsedParameters = 0;

            foreach (var argument in function.Arguments)
            {
#if DEBUG
                //Log($"argument = {argument}");
#endif

                var argumentName = argument.Name;

#if DEBUG
                //Log($"argumentName = {argumentName}");
#endif

                var parameterValue = GetParameterValue(argumentName, namedParameters, localCodeExecutionContext);

#if DEBUG

                //Log($"parameterValue = {parameterValue}");
#endif

                if (parameterValue == null)
                {
                    if (argument.HasDefaultValue)
                    {
                        result.Add(0u);

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

                    var distance = _inheritanceResolver.GetDistance(argument.TypesList, parameterValue, localCodeExecutionContext, options);

#if DEBUG
                    //Log($"distance = {distance}");
#endif

                    if (!distance.HasValue)
                    {
                        return null;
                    }

                    result.Add(distance.Value);
                }
            }

#if DEBUG
            //Log($"countOfUsedParameters = {countOfUsedParameters}");
            //Log($"namedParameters.Count = {namedParameters.Count}");
#endif

            if (countOfUsedParameters < namedParameters.Count)
            {
                return null;
            }

            return result;
        }

        protected List<uint> IsFit(IExecutable function, IList<Value> positionedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var positionedParametersEnumerator = positionedParameters.GetEnumerator();

            var result = new List<uint>();

            foreach (var argument in function.Arguments)
            {
#if DEBUG
                //Log($"argument = {argument}");
#endif

                if (!positionedParametersEnumerator.MoveNext())
                {
                    if (argument.HasDefaultValue)
                    {
                        result.Add(0u);

                        continue;
                    }

                    throw new NotImplementedException();
                }

                var parameterItem = positionedParametersEnumerator.Current;

#if DEBUG
                //Log($"parameterItem = {parameterItem}");
#endif

                var distance = _inheritanceResolver.GetDistance(argument.TypesList, parameterItem, localCodeExecutionContext, options);

#if DEBUG
                //Log($"distance = {distance}");
#endif

                if (!distance.HasValue)
                {
                    return null;
                }

                result.Add(distance.Value);
            }

            return result;
        }

        protected Value GetParameterValue(StrongIdentifierValue argumentName, IDictionary<StrongIdentifierValue, Value> namedParameters, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"argumentName = {argumentName}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
#endif

            if (namedParameters.ContainsKey(argumentName))
            {
                return namedParameters[argumentName];
            }

            var synonymsList = _synonymsResolver.GetSynonyms(argumentName, localCodeExecutionContext);

#if DEBUG
            //Log($"synonymsList = {synonymsList.WriteListToString()}");
#endif

            foreach (var synonym in synonymsList)
            {
#if DEBUG
                //Log($"synonym = {synonym}");
#endif

                if (namedParameters.ContainsKey(synonym))
                {
                    return namedParameters[synonym];
                }

                var alternativeSynonym = NameHelper.CreateAlternativeArgumentName(synonym);

#if DEBUG
                //Log($"alternativeSynonym = {alternativeSynonym}");
#endif

                if (namedParameters.ContainsKey(alternativeSynonym))
                {
                    return namedParameters[alternativeSynonym];
                }
            }

            var alternativeArgumentName = NameHelper.CreateAlternativeArgumentName(argumentName);

#if DEBUG
            //Log($"alternativeArgumentName = {alternativeArgumentName}");
#endif

            synonymsList = _synonymsResolver.GetSynonyms(alternativeArgumentName, localCodeExecutionContext);

#if DEBUG
            //Log($"synonymsList = {synonymsList.WriteListToString()}");
#endif

            foreach (var synonym in synonymsList)
            {
#if DEBUG
                //Log($"synonym = {synonym}");
#endif

                if (namedParameters.ContainsKey(synonym))
                {
                    return namedParameters[synonym];
                }

                var alternativeSynonym = NameHelper.CreateAlternativeArgumentName(synonym);

#if DEBUG
                //Log($"alternativeSynonym = {alternativeSynonym}");
#endif

                if (namedParameters.ContainsKey(alternativeSynonym))
                {
                    return namedParameters[alternativeSynonym];
                }
            }

            return null;
        }

        protected T GetTargetValueFromList<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source, int paramsCount, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
            where T : AnnotatedItem, IExecutable
        {
#if DEBUG
            //Log($"paramsCount = {paramsCount}");
#endif

            CorrectParametersRankMatrixForSpecialCases(source);

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
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[0]);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[0]);
                            }
                            break;

                        case 1:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[1]);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[1]);
                            }
                            break;

                        case 2:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[2]);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[2]);
                            }
                            break;

                        case 3:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[3]);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[3]);
                            }
                            break;

                        case 4:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[4]);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[4]);
                            }
                            break;

                        case 5:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[5]);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[5]);
                            }
                            break;

                        case 6:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[6]);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[6]);
                            }
                            break;

                        case 7:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[7]);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[7]);
                            }
                            break;

                        case 8:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[8]);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[8]);
                            }
                            break;

                        case 9:
                            if (orderedList == null)
                            {
                                orderedList = source.OrderBy(p => p.ParametersRankMatrix[9]);
                            }
                            else
                            {
                                orderedList = orderedList.ThenBy(p => p.ParametersRankMatrix[9]);
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

            return orderedList.FirstOrDefault()?.ResultItem;
        }

        protected void CorrectParametersRankMatrixForSpecialCases<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source)
            where T : AnnotatedItem, IExecutable
        {
            var advicesDict = CheckSpecialCasesInParameters(source);

            if (advicesDict.Count == 0)
            {
                return;
            }

            foreach (var function in source)
            {
                var parametersRankMatrix = function.ParametersRankMatrix;
                var argumentsList = function.ResultItem.Arguments;

#if DEBUG
                //Log($"parametersRankMatrix = {parametersRankMatrix.WritePODListToString()}");
                //Log($"argumentsList = {argumentsList.WriteListToString()}");
#endif

                var argumentsDict = ConvertArgumentsListToDictByPosition(argumentsList);

                foreach (var advice in advicesDict)
                {
                    var position = advice.Key;
                    var checkedTypeName = advice.Value;

                    var argument = argumentsDict[position];

#if DEBUG
                    //Log($"position = {position}");
                    //Log($"checkedTypeName = {checkedTypeName}");
                    //Log($"argument = {argument}");
#endif

                    var typesList = argument.TypesList;

                    if (typesList.IsNullOrEmpty())
                    {
                        continue;
                    }

                    if (typesList.Contains(checkedTypeName))
                    {
                        continue;
                    }

                    if (checkedTypeName == _fuzzyTypeIdentifier)
                    {
                        if (typesList.Contains(_numberTypeIdentifier))
                        {
#if DEBUG
                            //Log($"parametersRankMatrix (before) = {parametersRankMatrix.WritePODListToString()}");
#endif

                            parametersRankMatrix[position]++;

#if DEBUG
                            //Log($"parametersRankMatrix (after) = {parametersRankMatrix.WritePODListToString()}");
#endif

                            continue;
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }

        protected Dictionary<int, IFunctionArgument> ConvertArgumentsListToDictByPosition(IList<IFunctionArgument> argumentsList)
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

        protected Dictionary<int, StrongIdentifierValue> CheckSpecialCasesInParameters<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source)
            where T : AnnotatedItem, IExecutable
        {
            var result = new Dictionary<int, StrongIdentifierValue>();

            foreach (var function in source)
            {
                var argumentsList = function.ResultItem.Arguments;

#if DEBUG
                //Log($"argumentsList = {argumentsList.WriteListToString()}");
#endif

                if (argumentsList.IsNullOrEmpty())
                {
                    continue;
                }

                var i = -1;

                foreach (var argument in argumentsList)
                {
                    i++;
#if DEBUG
                    //Log($"i = {i}");
                    //Log($"argument = {argument}");
#endif

                    var typesList = argument.TypesList;

                    if (typesList.IsNullOrEmpty())
                    {
                        continue;
                    }

                    if (typesList.Contains(_fuzzyTypeIdentifier))
                    {
                        result[i] = _fuzzyTypeIdentifier;
                    }
                }
            }

            return result;
        }

    }
}
