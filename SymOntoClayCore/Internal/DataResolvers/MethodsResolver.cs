using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class MethodsResolver : BaseResolver
    {
        public MethodsResolver(IMainStorageContext context)
            : base(context)
        {
        }

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();

        public NamedFunction Resolve(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, localCodeExecutionContext, _defaultOptions);
        }

        public NamedFunction Resolve(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"name = {name}");
            //Log($"value = {value}");
            //Log($"reason = {reason}");
            //Log($"localCodeExecutionContext = {localCodeExecutionContext}");
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            var inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawList(name, 0, storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(rawList);

#if DEBUG
            Log($"filteredList = {filteredList.WriteListToString()}");
#endif

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return GetTargetValueFromList(filteredList, 0, localCodeExecutionContext, options);
        }

        public NamedFunction Resolve(StrongIdentifierValue name, Dictionary<StrongIdentifierValue, Value> namedParameters, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, namedParameters, localCodeExecutionContext, _defaultOptions);
        }

        public NamedFunction Resolve(StrongIdentifierValue name, Dictionary<StrongIdentifierValue, Value> namedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"name = {name}");
            //Log($"value = {value}");
            //Log($"reason = {reason}");
            //Log($"localCodeExecutionContext = {localCodeExecutionContext}");
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            var inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawList(name, namedParameters.Count, storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(rawList);

#if DEBUG
            //Log($"filteredList = {filteredList.WriteListToString()}");
#endif

            if (!filteredList.Any())
            {
                return null;
            }

            filteredList = FilterByTypeOfParameters(filteredList, namedParameters, localCodeExecutionContext, options);

#if DEBUG
            //Log($"filteredList (2) = {filteredList.WriteListToString()}");
#endif

            if (!filteredList.Any())
            {
                return null;
            }

            if(filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return GetTargetValueFromList(filteredList, namedParameters.Count, localCodeExecutionContext, options);
        }

        public NamedFunction Resolve(StrongIdentifierValue name, List<Value> positionedParameters, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, positionedParameters, localCodeExecutionContext, _defaultOptions);
        }

        public NamedFunction Resolve(StrongIdentifierValue name, List<Value> positionedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"name = {name}");
            //Log($"value = {value}");
            //Log($"reason = {reason}");
            //Log($"localCodeExecutionContext = {localCodeExecutionContext}");
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            var inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawList(name, positionedParameters.Count, storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(rawList);

#if DEBUG
            //Log($"filteredList = {filteredList.WriteListToString()}");
#endif

            if(!filteredList.Any())
            {
                return null;
            }

            filteredList = FilterByTypeOfParameters(filteredList, positionedParameters, localCodeExecutionContext, options);

#if DEBUG
            Log($"filteredList (2) = {filteredList.WriteListToString()}");
#endif

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return GetTargetValueFromList(filteredList, positionedParameters.Count, localCodeExecutionContext, options);
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>> GetRawList(StrongIdentifierValue name, int paramsCount, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"paramsCount = {paramsCount}");
#endif

            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.MethodsStorage.GetNamedFunctionsDirectly(name, paramsCount, weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<NamedFunction>(item, distance, storage));
                }
            }

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>> FilterByTypeOfParameters(List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>> source, Dictionary<StrongIdentifierValue, Value> namedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var result = new List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>>();

            foreach(var function in source)
            {
                var rankMatrix = IsFit(function.ResultItem, namedParameters, localCodeExecutionContext, options);

#if DEBUG
                //Log($"rankMatrix = {rankMatrix.WritePODListToString()}");
#endif

                if(rankMatrix == null)
                {
                    continue;
                }

                function.ParametersRankMatrix = rankMatrix;

                result.Add(function);
            }

            return result;
        }

        private List<uint> IsFit(NamedFunction function, Dictionary<StrongIdentifierValue, Value> namedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

            var result = new List<uint>();

            var usedParameters = new List<StrongIdentifierValue>();

            foreach (var namedParameter in namedParameters)
            {
                var parameterName = namedParameter.Key;

#if DEBUG
                //Log($"parameterName = {parameterName}");
#endif

                var kindOfParameterName = parameterName.KindOfName;

                switch (kindOfParameterName)
                {
                    case KindOfName.Var:
                        break;

                    case KindOfName.Concept:
                        parameterName = NameHelper.CreateName($"@{parameterName.NameValue}");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfParameterName), kindOfParameterName, null);
                }

#if DEBUG
                //Log($"parameterName (after) = {parameterName}");
#endif

                var parameterValue = namedParameter.Value;

#if DEBUG
                //Log($"parameterValue = {parameterValue}");
#endif

                var argument = function.GetArgument(parameterName);

#if DEBUG
                //Log($"argument = {argument}");
#endif

                if(argument == null)
                {
                    return null;
                }

                usedParameters.Add(parameterName);

                var distance = inheritanceResolver.GetDistance(argument.TypesList, parameterValue, localCodeExecutionContext, options);

#if DEBUG
                //Log($"distance = {distance}");
#endif

                if(!distance.HasValue)
                {
                    return null;
                }

                result.Add(distance.Value);
            }

            var argumentsList = function.Arguments;

            if (usedParameters.Count < argumentsList.Count)
            {
                foreach (var argument in argumentsList)
                {
                    if (usedParameters.Contains(argument.Name))
                    {
                        continue;
                    }

#if DEBUG
                    //Log($"argument = {argument}");
#endif

                    if (argument.HasDefaultValue)
                    {
                        result.Add(0u);

                        continue;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }                

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>> FilterByTypeOfParameters(List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>> source, List<Value> positionedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var result = new List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>>();

            foreach (var function in source)
            {
                var rank = IsFit(function.ResultItem, positionedParameters, localCodeExecutionContext, options);

#if DEBUG
                Log($"rank = {rank}");
#endif

                if (rank == 0)
                {
                    continue;
                }

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        private int IsFit(NamedFunction function, List<Value> positionedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

            throw new NotImplementedException();
        }

        private NamedFunction GetTargetValueFromList(List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>> source, int paramsCount, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"paramsCount = {paramsCount}");
#endif

            IOrderedEnumerable<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>> orderedList = null;

            if (paramsCount > 0)
            {
                for(var i = 0; i < paramsCount; i++)
                {
                    switch(i)
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

            if(orderedList == null)
            {
                orderedList = source.OrderByDescending(p => p.IsSelf);
            }
            else
            {
                orderedList = orderedList.ThenByDescending(p => p.IsSelf);
            }

            orderedList = orderedList.ThenBy(p => p.Distance).ThenBy(p => p.StorageDistance);

            return orderedList.FirstOrDefault()?.ResultItem;
        }
    }
}
