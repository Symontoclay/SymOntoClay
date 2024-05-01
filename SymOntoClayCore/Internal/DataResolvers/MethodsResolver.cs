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
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public IExecutable Resolve(IMonitorLogger logger, string callMethodId, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(logger, callMethodId, name, localCodeExecutionContext, _defaultOptions);
        }

        public IExecutable Resolve(IMonitorLogger logger, string callMethodId, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var result = EnumerableLocalCodeExecutionContext<IExecutable>(logger, localCodeExecutionContext, (ctx) =>
            {
                var method = ResolveMethod(logger, callMethodId, name, ctx, options);

                if (method == null)
                {
                    return ResolveAction(logger, callMethodId, name, ctx, options);
                }

                return method;
            });

            return result;
        }

        public IExecutable Resolve(IMonitorLogger logger, string callMethodId, StrongIdentifierValue name, Dictionary<StrongIdentifierValue, Value> namedParameters, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(logger, callMethodId, name, namedParameters, localCodeExecutionContext, _defaultOptions);
        }

        public IExecutable Resolve(IMonitorLogger logger, string callMethodId, StrongIdentifierValue name, Dictionary<StrongIdentifierValue, Value> namedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var result = EnumerableLocalCodeExecutionContext<IExecutable>(logger, localCodeExecutionContext, (ctx) => {
                var method = ResolveMethod(logger, callMethodId, name, namedParameters, localCodeExecutionContext, options);

                if (method == null)
                {
                    return ResolveAction(logger, callMethodId, name, namedParameters, localCodeExecutionContext, options);
                }

                return method;
            });

            return result;
        }

        public IExecutable Resolve(IMonitorLogger logger, string callMethodId, StrongIdentifierValue name, List<Value> positionedParameters, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(logger, callMethodId, name, positionedParameters, localCodeExecutionContext, _defaultOptions);
        }

        public IExecutable Resolve(IMonitorLogger logger, string callMethodId, StrongIdentifierValue name, List<Value> positionedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var result = EnumerableLocalCodeExecutionContext<IExecutable>(logger, localCodeExecutionContext, (ctx) => {
                var method = ResolveMethod(logger, callMethodId, name, positionedParameters, localCodeExecutionContext, options);

                if (method == null)
                {
                    return ResolveAction(logger, callMethodId, name, positionedParameters, localCodeExecutionContext, options);
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
        private IExecutable ResolveMethod(IMonitorLogger logger, string callMethodId, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            logger.MethodResolving("976907DE-71BF-4083-8AC4-7CEB631CCF2B", callMethodId);

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawMethodsList(logger, name, 0, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                logger.EndMethodResolving("82F40AB7-4635-4F1F-9F98-1BCB038D439C", callMethodId);

                return null;
            }

            var filteredList = FilterCodeItems(logger, rawList, localCodeExecutionContext);

            if (!filteredList.Any())
            {
                logger.EndMethodResolving("4D692F7C-4B2B-4AB2-8AB6-0E7D0EE7FFAC", callMethodId);

                return null;
            }

            if (filteredList.Count == 1)
            {
                var result = filteredList.Single().ResultItem;

                logger.EndMethodResolving("C0E1449A-BD6E-4D7E-818D-2920F3DF8E54", callMethodId);

                return result;
            }

            {
                var result = GetTargetValueFromList(logger, filteredList, 0, localCodeExecutionContext, options);

                logger.EndMethodResolving("F09F2B55-078C-41C6-8C7E-D173A5EFFC05", callMethodId);

                return result;
            }
        }

        private IExecutable ResolveMethod(IMonitorLogger logger, string callMethodId, StrongIdentifierValue name, Dictionary<StrongIdentifierValue, Value> namedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            logger.MethodResolving("484228EB-B4DA-4663-AEC3-F7DA1CFCD6A0", callMethodId);

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawMethodsList(logger, name, namedParameters.Count, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                logger.EndMethodResolving("BECFBC51-514F-4A07-A6A2-13FF45016CA1", callMethodId);

                return null;
            }

            var filteredList = Filter(logger, rawList);

            if (!filteredList.Any())
            {
                logger.EndMethodResolving("80CEDD3B-6A2A-4646-A113-09C23FCF5905", callMethodId);

                return null;
            }

            namedParameters = NormalizeNamedParameters(logger, namedParameters);

            filteredList = FilterByTypeOfParameters(logger, filteredList, namedParameters, localCodeExecutionContext, options);

            if (!filteredList.Any())
            {
                logger.EndMethodResolving("896A47B7-3088-4906-AD9D-5E062BCDE360", callMethodId);

                return null;
            }

            if (filteredList.Count == 1)
            {
                var result = filteredList.Single().ResultItem;

                logger.EndMethodResolving("2959C0BA-89F9-4F3F-9150-CB0D2CE2B049", callMethodId);

                return result;
            }

            {
                var result = GetTargetValueFromList(logger, filteredList, namedParameters.Count, localCodeExecutionContext, options);

                logger.EndMethodResolving("9827CC29-F394-47E1-84BC-28A24E8217A6", callMethodId);

                return result;
            }
        }

        private IExecutable ResolveMethod(IMonitorLogger logger, string callMethodId, StrongIdentifierValue name, List<Value> positionedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            logger.MethodResolving("0D121D2D-4396-4D2B-A10E-D8869F837EE0", callMethodId);

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawMethodsList(logger, name, positionedParameters.Count, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                logger.EndMethodResolving("39FC45C0-48BB-402F-98B1-CA78B61F3E18", callMethodId);

                return null;
            }

            var filteredList = Filter(logger, rawList);

            if (!filteredList.Any())
            {
                logger.EndMethodResolving("5F8E02C0-CB45-414B-9C4B-626B45CEBFDC", callMethodId);

                return null;
            }

            filteredList = FilterByTypeOfParameters(logger, filteredList, positionedParameters, localCodeExecutionContext, options);

            if (!filteredList.Any())
            {
                logger.EndMethodResolving("E45B48C6-8AF8-4097-AC65-2DB0DBF2AF4B", callMethodId);

                return null;
            }

            if (filteredList.Count == 1)
            {
                var result = filteredList.Single().ResultItem;

                logger.EndMethodResolving("FE6647BE-2E28-4E2E-8D44-0A22EB6B27E7", callMethodId);

                return result;
            }

            {
                var result = GetTargetValueFromList(logger, filteredList, positionedParameters.Count, localCodeExecutionContext, options);

                logger.EndMethodResolving("159D1775-2BB9-4536-8303-88665B085540", callMethodId);

                return result;
            }
        }

        private IExecutable ResolveAction(IMonitorLogger logger, string callMethodId, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            logger.ActionResolving("ECC4EA61-A29D-4C1C-9BA5-C92A929BAFC2", callMethodId);

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawActionsList(logger, name, 0, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                logger.EndActionResolving("F8D2EF38-CC0F-44C9-8909-37EAE7B8A3BB", callMethodId);

                return null;
            }

            var filteredList = Filter(logger, rawList);

            if (!filteredList.Any())
            {
                logger.EndActionResolving("97839DFB-130F-4655-83E5-02A998B3E422", callMethodId);

                return null;
            }

            if (filteredList.Count == 1)
            {
                var result = filteredList.Single().ResultItem;

                logger.EndActionResolving("62FDF518-9FB9-44E5-84FF-C3DE20FB6E92", callMethodId);

                return result;
            }

            {
                var result = GetTargetValueFromList(logger, filteredList, 0, localCodeExecutionContext, options);

                logger.EndActionResolving("41E4F23F-99A2-4D7A-BDBB-A585CE78787B", callMethodId);

                return result;
            }            
        }

        private IExecutable ResolveAction(IMonitorLogger logger, string callMethodId, StrongIdentifierValue name, Dictionary<StrongIdentifierValue, Value> namedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            logger.ActionResolving("E91CA7A9-F2EE-4766-A597-63636B76C6F0", callMethodId);

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawActionsList(logger, name, namedParameters.Count, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                logger.EndActionResolving("CDACA6DB-ACBA-4448-BB97-1D1D4D8D4D69", callMethodId);

                return null;
            }

            var filteredList = Filter(logger, rawList);

            if (!filteredList.Any())
            {
                logger.EndActionResolving("B6EF8E7A-6780-4897-A29D-9AC488A22109", callMethodId);

                return null;
            }

            namedParameters = NormalizeNamedParameters(logger, namedParameters);

            filteredList = FilterByTypeOfParameters(logger, filteredList, namedParameters, localCodeExecutionContext, options);

            if (!filteredList.Any())
            {
                logger.EndActionResolving("1B75500A-5FD6-4056-A7F4-66B16A199466", callMethodId);

                return null;
            }

            if (filteredList.Count == 1)
            {
                var result = filteredList.Single().ResultItem;

                logger.EndActionResolving("2CE7572C-28B9-4238-B7D7-324D647AAD7B", callMethodId);

                return result;
            }

            {
                var result = GetTargetValueFromList(logger, filteredList, namedParameters.Count, localCodeExecutionContext, options);

                logger.EndActionResolving("CB480087-B5F7-40AB-BFAE-8D149B4871F0", callMethodId);

                return result;
            }            
        }

        private IExecutable ResolveAction(IMonitorLogger logger, string callMethodId, StrongIdentifierValue name, List<Value> positionedParameters, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            logger.ActionResolving("8A6FE64C-3B5E-4C1F-AA2F-B8DC23D7399C", callMethodId);

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawActionsList(logger, name, positionedParameters.Count, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                logger.EndActionResolving("E3D6128D-9A88-4024-B2B9-0C8AF9EC31B5", callMethodId);

                return null;
            }

            var filteredList = Filter(logger, rawList);

            if (!filteredList.Any())
            {
                logger.EndActionResolving("779C98FF-5A9C-4CE9-89C8-0E625B3A743C", callMethodId);

                return null;
            }

            filteredList = FilterByTypeOfParameters(logger, filteredList, positionedParameters, localCodeExecutionContext, options);

            if (!filteredList.Any())
            {
                logger.EndActionResolving("04FBF727-95AD-45A6-8559-CBDA33FACAA6", callMethodId);

                return null;
            }

            if (filteredList.Count == 1)
            {
                var result = filteredList.Single().ResultItem;

                logger.EndActionResolving("A34370C9-70B0-4B2E-A3A4-0313D7D25D38", callMethodId);

                return result;
            }

            {
                var result = GetTargetValueFromList(logger, filteredList, positionedParameters.Count, localCodeExecutionContext, options);

                logger.EndActionResolving("4C6AADA0-3ABC-4B71-87B7-C04AC654833C", callMethodId);

                return result;
            }
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
