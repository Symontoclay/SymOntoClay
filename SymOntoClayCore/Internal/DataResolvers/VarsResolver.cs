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
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class VarsResolver : BaseResolver
    {
        public VarsResolver(IMainStorageContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();

            _anyTypeName = _context.CommonNamesStorage.AnyTypeName;
            _typeConverter = _context.TypeConverter;
        }

        private StrongIdentifierValue _anyTypeName;
        private ITypeConverter _typeConverter;

        public CallResult SetVarValue(IMonitorLogger logger, StrongIdentifierValue varName, Value value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return SetVarValue(logger, varName, value, localCodeExecutionContext, _defaultOptions);
        }

        public CallResult SetVarValue(IMonitorLogger logger, StrongIdentifierValue varName, Value value, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if (varName.KindOfName != KindOfName.Var)
            {
                throw new Exception($"It is impossible to set value '{value.ToHumanizedString()}' into '{varName.ToHumanizedString()}'. Value can be set only into variable.");
            }

            var varPtr = Resolve(logger, varName, localCodeExecutionContext, options);

            if(varPtr == null)
            {
                varPtr = CreateAndSaveLocalVariable(logger, varName, localCodeExecutionContext);
            }

            var callResult = _typeConverter.CheckAndTryConvert(logger, value, varPtr.TypesList, localCodeExecutionContext);

            if (callResult.IsError)
            {
                return callResult;
            }

            varPtr.SetValueDirectly(logger, callResult.Value);

            return new CallResult(value);
        }

        private VarInstance CreateAndSaveLocalVariable(IMonitorLogger logger, StrongIdentifierValue varName, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var result = new VarInstance(varName, TypeOfAccess.Local, _context);

            localCodeExecutionContext.Storage.VarStorage.Append(logger, result);

            return result;
        }

        public Value GetVarValue(IMonitorLogger logger, StrongIdentifierValue varName, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetVarValue(logger, varName, localCodeExecutionContext, _defaultOptions);
        }

        public Value GetVarValue(IMonitorLogger logger, StrongIdentifierValue varName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if(varName.KindOfName == KindOfName.SystemVar)
            {
                return GetSystemVarValue(logger, varName, localCodeExecutionContext, options);
            }

            return GetUsualVarValue(logger, varName, localCodeExecutionContext, options);
        }

        private Value GetSystemVarValue(IMonitorLogger logger, StrongIdentifierValue varName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            foreach (var storageItem in storagesList)
            {
                var targetValue = storageItem.Storage.VarStorage.GetSystemValueDirectly(logger, varName);

                if(targetValue != null)
                {
                    return targetValue;
                }
            }

            throw new NotImplementedException("A597A52F-30D6-4901-8718-F76713A51211");
        }

        private Value GetUsualVarValue(IMonitorLogger logger, StrongIdentifierValue varName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var varPtr = Resolve(logger, varName, localCodeExecutionContext, _defaultOptions);

            if (varPtr == null)
            {
                varPtr = CreateAndSaveLocalVariable(logger, varName, localCodeExecutionContext);
            }

            return varPtr.Value;
        }

        public VarInstance Resolve(IMonitorLogger logger, StrongIdentifierValue varName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            VarInstance varPtr = null;

            if (localCodeExecutionContext.OwnerStorage != null)
            {
                varPtr = NResolve(logger, varName, localCodeExecutionContext.Owner, localCodeExecutionContext.OwnerStorage, true, localCodeExecutionContext, options);
            }

            if(varPtr != null)
            {
                return varPtr;
            }

            return NResolve(logger, varName, localCodeExecutionContext.Holder, localCodeExecutionContext.Storage, false, localCodeExecutionContext, options);
        }

        private VarInstance NResolve(IMonitorLogger logger, StrongIdentifierValue varName, StrongIdentifierValue holder, IStorage storage, bool privateOnly, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.Var);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawVarsList(logger, varName, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                return null;
            }

            if(privateOnly)
            {
                rawList = rawList.Where(p => p.ResultItem.TypeOfAccess == TypeOfAccess.Private).ToList();

            }

            var filteredList = FilterCodeItems(logger, rawList, holder, localCodeExecutionContext);

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            var minStorageDistance = filteredList.Min(p => p.StorageDistance);

            filteredList = filteredList.Where(p => p.StorageDistance == minStorageDistance).ToList();

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return OrderAndDistinctByInheritance(logger, filteredList, options).FirstOrDefault()?.ResultItem;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<VarInstance>> GetRawVarsList(IMonitorLogger logger, StrongIdentifierValue name, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<VarInstance>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<VarInstance>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.VarStorage.GetVarDirectly(logger, name, weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<VarInstance>(item, distance, storage));
                }
            }

            return result;
        }

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();

        public ResolverOptions DefaultOptions => _defaultOptions;
    }
}
