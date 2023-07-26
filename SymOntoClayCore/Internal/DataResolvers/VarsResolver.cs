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

using NLog.Fluent;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class VarsResolver : BaseResolver
    {
        public VarsResolver(IMainStorageContext context)
            : base(context)
        {
            _inheritanceResolver = context.DataResolversFactory.GetInheritanceResolver();
        }

        private readonly InheritanceResolver _inheritanceResolver;

        public void SetVarValue(StrongIdentifierValue varName, Value value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            SetVarValue(varName, value, localCodeExecutionContext, _defaultOptions);
        }

        public void SetVarValue(StrongIdentifierValue varName, Value value, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if (varName.KindOfName != KindOfName.Var)
            {
                throw new Exception($"It is impossible to set value '{value.ToHumanizedString()}' into '{varName.ToHumanizedString()}'. Value can be set only into variable.");
            }

            var varPtr = Resolve(varName, localCodeExecutionContext, options);

            if(varPtr == null)
            {
                varPtr = CreateAndSaveLocalVariable(varName, localCodeExecutionContext);
            }

            CheckFitVariableAndValue(varPtr, value, localCodeExecutionContext, options);

            varPtr.Value = value;
        }

        public void CheckFitVariableAndValue(Var varItem, Value value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            CheckFitVariableAndValue(varItem, value, localCodeExecutionContext, _defaultOptions);
        }

        public void CheckFitVariableAndValue(Var varItem, Value value, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if(varItem.TypesList.IsNullOrEmpty())
            {
                return;
            }

            if(value.IsNullValue)
            {
                return;
            }

            var isFit = _inheritanceResolver.IsFit(varItem.TypesList, value, localCodeExecutionContext, options);

            if (isFit)
            {
                return;
            }

            throw new Exception($"The value '{value.ToHumanizedString()}' does not fit to variable {varItem.ToHumanizedString()}");
        }

        private Var CreateAndSaveLocalVariable(StrongIdentifierValue varName, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var result = new Var();
            result.Name = varName;
            result.TypeOfAccess = TypeOfAccess.Local;

            localCodeExecutionContext.Storage.VarStorage.Append(result);

            return result;
        }

        public Value GetVarValue(StrongIdentifierValue varName, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetVarValue(varName, localCodeExecutionContext, _defaultOptions);
        }

        public Value GetVarValue(StrongIdentifierValue varName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if(varName.KindOfName == KindOfName.SystemVar)
            {
                return GetSystemVarValue(varName, localCodeExecutionContext, options);
            }

            return GetUsualVarValue(varName, localCodeExecutionContext, options);
        }

        private Value GetSystemVarValue(StrongIdentifierValue varName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

            foreach (var storageItem in storagesList)
            {
                var targetValue = storageItem.Storage.VarStorage.GetSystemValueDirectly(varName);

                if(targetValue != null)
                {
                    return targetValue;
                }
            }

            throw new NotImplementedException();
        }

        private Value GetUsualVarValue(StrongIdentifierValue varName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var varPtr = Resolve(varName, localCodeExecutionContext, _defaultOptions);

            if (varPtr == null)
            {
                varPtr = CreateAndSaveLocalVariable(varName, localCodeExecutionContext);
            }

            return varPtr.Value;
        }

        public Var Resolve(StrongIdentifierValue varName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            Var varPtr = null;

#if DEBUG
            Log($"varName = {varName}");
            Log($"localCodeExecutionContext.OwnerStorage != null = {localCodeExecutionContext.OwnerStorage != null}");
#endif

            if (localCodeExecutionContext.OwnerStorage != null)
            {
                varPtr = NResolve(varName, localCodeExecutionContext.Owner, localCodeExecutionContext.OwnerStorage, true, localCodeExecutionContext, options);
            }

            if(varPtr != null)
            {
                return varPtr;
            }

            var result = NResolve(varName, localCodeExecutionContext.Holder, localCodeExecutionContext.Storage, false, localCodeExecutionContext, options);

#if DEBUG
            Log($"result?.GetHashCode() = {result?.GetHashCode()}");
#endif

            return result;
        }

        private Var NResolve(StrongIdentifierValue varName, StrongIdentifierValue holder, IStorage storage, bool privateOnly, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Log($"varName = {varName}");
#endif

#if DEBUG
            //var oldStoragesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);
            //foreach (var tmpItem in oldStoragesList)
            //{
            //    Log("############################################################");
            //    Log($"tmpItem.Storage.Kind = {tmpItem.Storage.Kind}");
            //    Log($"tmpItem.Storage.GetHashCode() = {tmpItem.Storage.GetHashCode()}");
            //    Log($"tmpItem.Storage.TargetClassName = {tmpItem.Storage.TargetClassName?.ToHumanizedString()}");
            //    Log($"tmpItem.Storage.IsIsolated = {tmpItem.Storage.IsIsolated}");
            //    tmpItem.Storage.VarStorage.DbgPrintVariables();
            //}
#endif

            var storagesList = GetStoragesList(storage, KindOfStoragesList.Var);

#if DEBUG
            foreach(var tmpItem in storagesList)
            {
                Log("-------------------------------------");
                Log($"tmpItem.Storage.Kind = {tmpItem.Storage.Kind}");
                Log($"tmpItem.Storage.GetHashCode() = {tmpItem.Storage.GetHashCode()}");
                Log($"tmpItem.Storage.TargetClassName = {tmpItem.Storage.TargetClassName?.ToHumanizedString()}");
                Log($"tmpItem.Storage.IsIsolated = {tmpItem.Storage.IsIsolated}");
                tmpItem.Storage.VarStorage.DbgPrintVariables();
            }
#endif

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawVarsList(varName, storagesList, weightedInheritanceItems);

#if DEBUG
            foreach (var tmpItem in rawList)
            {
                Log("==========================================");
                Log($"tmpItem.ResultItem.GetHashCode() = {tmpItem.ResultItem.GetHashCode()}");
            }
#endif

            if (!rawList.Any())
            {
                return null;
            }

            if(privateOnly)
            {
                rawList = rawList.Where(p => p.ResultItem.TypeOfAccess == TypeOfAccess.Private).ToList();

            }

            var filteredList = FilterCodeItems(rawList, holder, localCodeExecutionContext);

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            var minStogageDistance = filteredList.Min(p => p.StorageDistance);

            filteredList = filteredList.Where(p => p.StorageDistance == minStogageDistance).ToList();

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return OrderAndDistinctByInheritance(filteredList, options).FirstOrDefault()?.ResultItem;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<Var>> GetRawVarsList(StrongIdentifierValue name, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<Var>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<Var>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.VarStorage.GetVarDirectly(name, weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<Var>(item, distance, storage));
                }
            }

            return result;
        }

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();

        public ResolverOptions DefaultOptions => _defaultOptions;
    }
}
