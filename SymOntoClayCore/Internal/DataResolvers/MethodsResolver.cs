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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class MethodsResolver : BaseResolver
    {
        #region public constructors
        public MethodsResolver(IMainStorageContext context)
            : base(context)
        {
            var dataResolversFactory = context.DataResolversFactory;

            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();
            _synonymsResolver = dataResolversFactory.GetSynonymsResolver();
        }
        #endregion
        
        #region public methods
        public IExecutable Resolve(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, localCodeExecutionContext, _defaultOptions);
        }

        public IExecutable Resolve(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var method = ResolveMethod(name, localCodeExecutionContext, options);

            if(method == null)
            {
                return ResolveAction(name, localCodeExecutionContext, options);
            }

            return method;
        }

        public IExecutable Resolve(StrongIdentifierValue name, Dictionary<StrongIdentifierValue, Value> namedParameters, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, namedParameters, localCodeExecutionContext, _defaultOptions);
        }

        public IExecutable Resolve(StrongIdentifierValue name, Dictionary<StrongIdentifierValue, Value> namedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var method = ResolveMethod(name, namedParameters, localCodeExecutionContext, options);

            if (method == null)
            {
                return ResolveAction(name, namedParameters, localCodeExecutionContext, options);
            }

            return method;
        }

        public IExecutable Resolve(StrongIdentifierValue name, List<Value> positionedParameters, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, positionedParameters, localCodeExecutionContext, _defaultOptions);
        }

        public IExecutable Resolve(StrongIdentifierValue name, List<Value> positionedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var method = ResolveMethod(name, positionedParameters, localCodeExecutionContext, options);

            if (method == null)
            {
                return ResolveAction(name, positionedParameters, localCodeExecutionContext, options);
            }

            return method;
        }

        public bool IsFit(Function function, KindOfFunctionParameters kindOfParameters, IDictionary<StrongIdentifierValue, Value> namedParameters, IList<Value> positionedParameters, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return IsFit(function, kindOfParameters, namedParameters, positionedParameters, localCodeExecutionContext, _defaultOptions);
        }

        public bool IsFit(Function function, KindOfFunctionParameters kindOfParameters, IDictionary<StrongIdentifierValue, Value> namedParameters, IList<Value> positionedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if(namedParameters != null)
            {
                namedParameters = NormalizeNamedParameters(namedParameters);
            }

#if DEBUG
            //Log($"kindOfParameters = {kindOfParameters}");
#endif

            switch (kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    {
                        var arguments = function.Arguments;

                        if (arguments.IsNullOrEmpty() || arguments.All(p => p.HasDefaultValue))
                        {
                            return true;
                        }

                        return false;
                    }

                case KindOfFunctionParameters.NamedParameters:
                    {
                        var rankMatrix = IsFit(function, namedParameters, localCodeExecutionContext, options);

#if DEBUG
                        //Log($"rankMatrix = {rankMatrix.WritePODListToString()}");
#endif

                        if(rankMatrix == null)
                        {
                            return false;
                        }

                        return true;
                    }

                case KindOfFunctionParameters.PositionedParameters:
                    {
                        var rankMatrix = IsFit(function, positionedParameters, localCodeExecutionContext, options);

#if DEBUG
                        //Log($"rankMatrix = {rankMatrix.WritePODListToString()}");
#endif

                        if (rankMatrix == null)
                        {
                            return false;
                        }

                        return true;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

            throw new NotImplementedException();
        }
        #endregion

        #region private fields
        private readonly InheritanceResolver _inheritanceResolver;
        private readonly SynonymsResolver _synonymsResolver;

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();

        private static readonly StrongIdentifierValue _fuzzyTypeIdentifier = NameHelper.CreateName(StandardNamesConstants.FuzzyTypeName);
        private static readonly StrongIdentifierValue _numberTypeIdentifier = NameHelper.CreateName(StandardNamesConstants.NumberTypeName);
        #endregion

        #region private methods
        private IExecutable ResolveMethod(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

#if DEBUG
            //Log($"name = {name}");
            //Log($"localCodeExecutionContext = {localCodeExecutionContext}");
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawMethodsList(name, 0, storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = FilterCodeItems(rawList, localCodeExecutionContext);

#if DEBUG
            //Log($"filteredList = {filteredList.WriteListToString()}");
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

        private IExecutable ResolveMethod(StrongIdentifierValue name, Dictionary<StrongIdentifierValue, Value> namedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

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

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawMethodsList(name, namedParameters.Count, storagesList, weightedInheritanceItems);

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

            namedParameters = NormalizeNamedParameters(namedParameters);

            filteredList = FilterByTypeOfParameters(filteredList, namedParameters, localCodeExecutionContext, options);

#if DEBUG
            //Log($"filteredList (2) = {filteredList.WriteListToString()}");
#endif

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return GetTargetValueFromList(filteredList, namedParameters.Count, localCodeExecutionContext, options);
        }

        private IExecutable ResolveMethod(StrongIdentifierValue name, List<Value> positionedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

#if DEBUG
            //Log($"name = {name}");
            //Log($"localCodeExecutionContext = {localCodeExecutionContext}");
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawMethodsList(name, positionedParameters.Count, storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"rawList.Count = {rawList.Count}");
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

            filteredList = FilterByTypeOfParameters(filteredList, positionedParameters, localCodeExecutionContext, options);

#if DEBUG
            //Log($"filteredList (2) = {filteredList.WriteListToString()}");
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

        private IExecutable ResolveAction(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

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

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawActionsList(name, 0, storagesList, weightedInheritanceItems);

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

            if (filteredList.Count == 1)
            {
                return ConvertToActionInstance(filteredList.Single().ResultItem, localCodeExecutionContext, options);
            }

            return ConvertToActionInstance(GetTargetValueFromList(filteredList, 0, localCodeExecutionContext, options), localCodeExecutionContext, options);
        }

        private IExecutable ResolveAction(StrongIdentifierValue name, Dictionary<StrongIdentifierValue, Value> namedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

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

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawActionsList(name, namedParameters.Count, storagesList, weightedInheritanceItems);

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

            namedParameters = NormalizeNamedParameters(namedParameters);

            filteredList = FilterByTypeOfParameters(filteredList, namedParameters, localCodeExecutionContext, options);

#if DEBUG
            //Log($"filteredList (2) = {filteredList.WriteListToString()}");
#endif

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return ConvertToActionInstance(filteredList.Single().ResultItem, localCodeExecutionContext, options);
            }

            return ConvertToActionInstance(GetTargetValueFromList(filteredList, namedParameters.Count, localCodeExecutionContext, options), localCodeExecutionContext, options);
        }

        private IExecutable ResolveAction(StrongIdentifierValue name, List<Value> positionedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

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

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawActionsList(name, positionedParameters.Count, storagesList, weightedInheritanceItems);

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

            filteredList = FilterByTypeOfParameters(filteredList, positionedParameters, localCodeExecutionContext, options);

#if DEBUG
            //Log($"filteredList (2) = {filteredList.WriteListToString()}");
#endif

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return ConvertToActionInstance(filteredList.Single().ResultItem, localCodeExecutionContext, options);
            }

            return ConvertToActionInstance(GetTargetValueFromList(filteredList, positionedParameters.Count, localCodeExecutionContext, options), localCodeExecutionContext, options);
        }

        private ActionInstanceValue ConvertToActionInstance(ActionPtr actionPtr, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"actionPtr = {actionPtr}");
#endif

            var result = new ActionInstanceValue(actionPtr, localCodeExecutionContext.Storage);

            result.CheckDirty();
            
            return result;
        }

        private Dictionary<StrongIdentifierValue, Value> NormalizeNamedParameters(IDictionary<StrongIdentifierValue, Value> source)
        {
            var result = new Dictionary<StrongIdentifierValue, Value>();

            foreach (var namedParameter in source)
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

                result[parameterName] = namedParameter.Value;
            }

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>> GetRawMethodsList(StrongIdentifierValue name, int paramsCount, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"paramsCount = {paramsCount}");
#endif

            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>>();
            }

            var synonymsList = _synonymsResolver.GetSynonyms(name, storagesList);

#if DEBUG
            //Log($"synonymsList = {synonymsList.WriteListToString()}");
#endif
            
            var result = new List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>>();

            var itemsList = NGetRawMethodsList(name, paramsCount, storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"itemsList?.Count = {itemsList?.Count}");
#endif

            if(!itemsList.IsNullOrEmpty())
            {
                result.AddRange(itemsList);
            }

            foreach(var synonym in synonymsList)
            {
                itemsList = NGetRawMethodsList(synonym, paramsCount, storagesList, weightedInheritanceItems);

#if DEBUG
                //Log($"itemsList?.Count = {itemsList?.Count}");
#endif

                if (!itemsList.IsNullOrEmpty())
                {
                    result.AddRange(itemsList);
                }
            }

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>> NGetRawMethodsList(StrongIdentifierValue name, int paramsCount, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"paramsCount = {paramsCount}");
#endif

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.MethodsStorage.GetNamedFunctionsDirectly(name, paramsCount, weightedInheritanceItems);

#if DEBUG
                //Log($"itemsList = {itemsList?.WriteListToString()}");
#endif

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

        private List<WeightedInheritanceResultItemWithStorageInfo<ActionPtr>> GetRawActionsList(StrongIdentifierValue name, int paramsCount, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"paramsCount = {paramsCount}");
#endif

            var synonymsList = _synonymsResolver.GetSynonyms(name, storagesList);

#if DEBUG
            //Log($"synonymsList = {synonymsList.WriteListToString()}");
#endif

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<ActionPtr>>();

            var itemsList = NGetRawActionsList(name, paramsCount, storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"itemsList?.Count = {itemsList?.Count}");
#endif

            if (!itemsList.IsNullOrEmpty())
            {
                result.AddRange(itemsList);
            }

            foreach (var synonym in synonymsList)
            {
                itemsList = NGetRawActionsList(synonym, paramsCount, storagesList, weightedInheritanceItems);

#if DEBUG
                //Log($"itemsList?.Count = {itemsList?.Count}");
#endif

                if (!itemsList.IsNullOrEmpty())
                {
                    result.AddRange(itemsList);
                }
            }

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<ActionPtr>> NGetRawActionsList(StrongIdentifierValue name, int paramsCount, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"paramsCount = {paramsCount}");
#endif

            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<ActionPtr>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<ActionPtr>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.ActionsStorage.GetActionsDirectly(name, paramsCount, weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<ActionPtr>(item, distance, storage));
                }
            }

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<T>> FilterByTypeOfParameters<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source, Dictionary<StrongIdentifierValue, Value> namedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
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

        private List<uint> IsFit(IExecutable function, IDictionary<StrongIdentifierValue, Value> namedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
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

                if(parameterValue == null)
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

        private Value GetParameterValue(StrongIdentifierValue argumentName, IDictionary<StrongIdentifierValue, Value> namedParameters, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"argumentName = {argumentName}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
#endif

            if(namedParameters.ContainsKey(argumentName))
            {
                return namedParameters[argumentName];
            }

            var synonymsList = _synonymsResolver.GetSynonyms(argumentName, localCodeExecutionContext);

#if DEBUG
            //Log($"synonymsList = {synonymsList.WriteListToString()}");
#endif

            foreach(var synonym in synonymsList)
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

        private List<WeightedInheritanceResultItemWithStorageInfo<T>> FilterByTypeOfParameters<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source, List<Value> positionedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
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

        private List<uint> IsFit(IExecutable function, IList<Value> positionedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
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

        private T GetTargetValueFromList<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source, int paramsCount, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
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

        private void CorrectParametersRankMatrixForSpecialCases<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source)
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

        private Dictionary<int, IFunctionArgument> ConvertArgumentsListToDictByPosition(IList<IFunctionArgument> argumentsList)
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

        private Dictionary<int, StrongIdentifierValue> CheckSpecialCasesInParameters<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source)
            where T: AnnotatedItem, IExecutable
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
        #endregion
    }
}
