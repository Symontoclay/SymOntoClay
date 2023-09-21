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
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class MethodsResolver : BaseMethodsResolver
    {
        #region public constructors
        public MethodsResolver(IMainStorageContext context)
            : base(context)
        {
        }
        #endregion
        
        #region public methods
        public IExecutable Resolve(IMonitorLogger logger, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(logger, name, localCodeExecutionContext, _defaultOptions);
        }

        public IExecutable Resolve(IMonitorLogger logger, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var result = EnumerableLocalCodeExecutionContext<IExecutable>(logger, localCodeExecutionContext, (ctx) =>
            {
                var method = ResolveMethod(logger, name, ctx, options);

                if (method == null)
                {
                    return ResolveAction(logger, name, ctx, options);
                }

                return method;
            });

            return result;
        }

        public IExecutable Resolve(IMonitorLogger logger, StrongIdentifierValue name, Dictionary<StrongIdentifierValue, Value> namedParameters, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(logger, name, namedParameters, localCodeExecutionContext, _defaultOptions);
        }

        public IExecutable Resolve(IMonitorLogger logger, StrongIdentifierValue name, Dictionary<StrongIdentifierValue, Value> namedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var result = EnumerableLocalCodeExecutionContext<IExecutable>(logger, localCodeExecutionContext, (ctx) => {
                var method = ResolveMethod(logger, name, namedParameters, localCodeExecutionContext, options);

                if (method == null)
                {
                    return ResolveAction(logger, name, namedParameters, localCodeExecutionContext, options);
                }

                return method;
            });

            return result;
        }

        public IExecutable Resolve(IMonitorLogger logger, StrongIdentifierValue name, List<Value> positionedParameters, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(logger, name, positionedParameters, localCodeExecutionContext, _defaultOptions);
        }

        public IExecutable Resolve(IMonitorLogger logger, StrongIdentifierValue name, List<Value> positionedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var result = EnumerableLocalCodeExecutionContext<IExecutable>(logger, localCodeExecutionContext, (ctx) => {
                var method = ResolveMethod(logger, name, positionedParameters, localCodeExecutionContext, options);

                if (method == null)
                {
                    return ResolveAction(logger, name, positionedParameters, localCodeExecutionContext, options);
                }

                return method;
            });

            return result;
        }

        public bool IsFit(IMonitorLogger logger, Function function, KindOfFunctionParameters kindOfParameters, IDictionary<StrongIdentifierValue, Value> namedParameters, IList<Value> positionedParameters, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return IsFit(logger, function, kindOfParameters, namedParameters, positionedParameters, localCodeExecutionContext, _defaultOptions);
        }

        public bool IsFit(IMonitorLogger logger, Function function, KindOfFunctionParameters kindOfParameters, IDictionary<StrongIdentifierValue, Value> namedParameters, IList<Value> positionedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if(namedParameters != null)
            {
                namedParameters = NormalizeNamedParameters(logger, namedParameters);
            }

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
                        var rankMatrix = IsFit(logger, function, namedParameters, localCodeExecutionContext, options);

                        if(rankMatrix == null)
                        {
                            return false;
                        }

                        return true;
                    }

                case KindOfFunctionParameters.PositionedParameters:
                    {
                        var rankMatrix = IsFit(logger, function, positionedParameters, localCodeExecutionContext, options);

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
        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();
        #endregion

        #region private methods
        private IExecutable ResolveMethod(IMonitorLogger logger, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawMethodsList(logger, name, 0, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = FilterCodeItems(logger, rawList, localCodeExecutionContext);

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return GetTargetValueFromList(logger, filteredList, 0, localCodeExecutionContext, options);
        }

        private IExecutable ResolveMethod(IMonitorLogger logger, StrongIdentifierValue name, Dictionary<StrongIdentifierValue, Value> namedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawMethodsList(logger, name, namedParameters.Count, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(logger, rawList);

            if (!filteredList.Any())
            {
                return null;
            }

            namedParameters = NormalizeNamedParameters(logger, namedParameters);

            filteredList = FilterByTypeOfParameters(logger, filteredList, namedParameters, localCodeExecutionContext, options);

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return GetTargetValueFromList(logger, filteredList, namedParameters.Count, localCodeExecutionContext, options);
        }

        private IExecutable ResolveMethod(IMonitorLogger logger, StrongIdentifierValue name, List<Value> positionedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawMethodsList(logger, name, positionedParameters.Count, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(logger, rawList);

            if (!filteredList.Any())
            {
                return null;
            }

            filteredList = FilterByTypeOfParameters(logger, filteredList, positionedParameters, localCodeExecutionContext, options);

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return GetTargetValueFromList(logger, filteredList, positionedParameters.Count, localCodeExecutionContext, options);
        }

        private IExecutable ResolveAction(IMonitorLogger logger, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawActionsList(logger, name, 0, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(logger, rawList);

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return GetTargetValueFromList(logger, filteredList, 0, localCodeExecutionContext, options);
        }

        private IExecutable ResolveAction(IMonitorLogger logger, StrongIdentifierValue name, Dictionary<StrongIdentifierValue, Value> namedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawActionsList(logger, name, namedParameters.Count, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(logger, rawList);

            if (!filteredList.Any())
            {
                return null;
            }

            namedParameters = NormalizeNamedParameters(logger, namedParameters);

            filteredList = FilterByTypeOfParameters(logger, filteredList, namedParameters, localCodeExecutionContext, options);

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return GetTargetValueFromList(logger, filteredList, namedParameters.Count, localCodeExecutionContext, options);
        }

        private IExecutable ResolveAction(IMonitorLogger logger, StrongIdentifierValue name, List<Value> positionedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawActionsList(logger, name, positionedParameters.Count, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(logger, rawList);

            if (!filteredList.Any())
            {
                return null;
            }

            filteredList = FilterByTypeOfParameters(logger, filteredList, positionedParameters, localCodeExecutionContext, options);

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return GetTargetValueFromList(logger, filteredList, positionedParameters.Count, localCodeExecutionContext, options);
        }

        private Dictionary<StrongIdentifierValue, Value> NormalizeNamedParameters(IMonitorLogger logger, IDictionary<StrongIdentifierValue, Value> source)
        {
            var result = new Dictionary<StrongIdentifierValue, Value>();

            foreach (var namedParameter in source)
            {
                var parameterName = namedParameter.Key;

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

                result[parameterName] = namedParameter.Value;
            }

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>> GetRawMethodsList(IMonitorLogger logger, StrongIdentifierValue name, int paramsCount, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var synonymsList = _synonymsResolver.GetSynonyms(logger, name, storagesList);

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>>();

            var itemsList = NGetRawMethodsList(logger, name, paramsCount, storagesList, weightedInheritanceItems);

            if(!itemsList.IsNullOrEmpty())
            {
                result.AddRange(itemsList);
            }

            foreach(var synonym in synonymsList)
            {
                itemsList = NGetRawMethodsList(logger, synonym, paramsCount, storagesList, weightedInheritanceItems);

                if (!itemsList.IsNullOrEmpty())
                {
                    result.AddRange(itemsList);
                }
            }

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>> NGetRawMethodsList(IMonitorLogger logger, StrongIdentifierValue name, int paramsCount, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var result = new List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.MethodsStorage.GetNamedFunctionsDirectly(logger, name, paramsCount, weightedInheritanceItems);

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

        private List<WeightedInheritanceResultItemWithStorageInfo<ActionPtr>> GetRawActionsList(IMonitorLogger logger, StrongIdentifierValue name, int paramsCount, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var synonymsList = _synonymsResolver.GetSynonyms(logger, name, storagesList);

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<ActionPtr>>();

            var itemsList = NGetRawActionsList(logger, name, paramsCount, storagesList, weightedInheritanceItems);

            if (!itemsList.IsNullOrEmpty())
            {
                result.AddRange(itemsList);
            }

            foreach (var synonym in synonymsList)
            {
                itemsList = NGetRawActionsList(logger, synonym, paramsCount, storagesList, weightedInheritanceItems);

                if (!itemsList.IsNullOrEmpty())
                {
                    result.AddRange(itemsList);
                }
            }

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<ActionPtr>> NGetRawActionsList(IMonitorLogger logger, StrongIdentifierValue name, int paramsCount, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<ActionPtr>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<ActionPtr>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.ActionsStorage.GetActionsDirectly(logger, name, paramsCount, weightedInheritanceItems);

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
        #endregion
    }
}
