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

        public void SetVarValue(StrongIdentifierValue varName, Value value, LocalCodeExecutionContext localCodeExecutionContext)
        {
            SetVarValue(varName, value, localCodeExecutionContext, _defaultOptions);
        }

        public void SetVarValue(StrongIdentifierValue varName, Value value, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"varName = {varName}");
            //Log($"value = {value}");
#endif

            if (varName.KindOfName != KindOfName.Var)
            {
                throw new Exception($"It is impossible to set value '{value.ToHumanizedString()}' into '{varName.ToHumanizedString()}'. Value can be set only into variable.");
            }

            var varPtr = Resolve(varName, localCodeExecutionContext, options);

#if DEBUG
            //Log($"varPtr = {varPtr}");
#endif

            if(varPtr == null)
            {
#if DEBUG
                //Log($"localCodeExecutionContext.Storage.VarStorage.GetHashCode() = {localCodeExecutionContext.Storage.VarStorage.GetHashCode()};localCodeExecutionContext.Storage.VarStorage.Kind = {localCodeExecutionContext.Storage.VarStorage.Kind}");
#endif

                varPtr = CreateAndSaveLocalVariable(varName, localCodeExecutionContext);
            }

#if DEBUG
            //Log($"varPtr (after) = {varPtr}");
#endif

            CheckFitVariableAndValue(varPtr, value, localCodeExecutionContext, options);

            varPtr.Value = value;
        }

        public void CheckFitVariableAndValue(Var varItem, Value value, LocalCodeExecutionContext localCodeExecutionContext)
        {
            CheckFitVariableAndValue(varItem, value, localCodeExecutionContext, _defaultOptions);
        }

        public void CheckFitVariableAndValue(Var varItem, Value value, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
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

#if DEBUG
            //Log($"isFit = {isFit}");
#endif

            if (isFit)
            {
                return;
            }

            throw new Exception($"The value '{value.ToHumanizedString()}' does not fit to variable {varItem.ToHumanizedString()}");
        }

        private Var CreateAndSaveLocalVariable(StrongIdentifierValue varName, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"varName = {varName}");
#endif

            var result = new Var();
            result.Name = varName;
            result.TypeOfAccess = TypeOfAccess.Local;

#if DEBUG
            //Log($"localCodeExecutionContext.Storage = {localCodeExecutionContext.Storage}");
#endif

            localCodeExecutionContext.Storage.VarStorage.Append(result);

            return result;
        }

        public Value GetVarValue(StrongIdentifierValue varName, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetVarValue(varName, localCodeExecutionContext, _defaultOptions);
        }

        public Value GetVarValue(StrongIdentifierValue varName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"varName = {varName}");
#endif

            if(varName.KindOfName == KindOfName.SystemVar)
            {
                return GetSystemVarValue(varName, localCodeExecutionContext, options);
            }

            return GetUsualVarValue(varName, localCodeExecutionContext, options);
        }

        private Value GetSystemVarValue(StrongIdentifierValue varName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"varName = {varName}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
#endif

            foreach (var storageItem in storagesList)
            {
#if DEBUG
                //Log($"storageItem.Storage.VarStorage.GetHashCode() = {storageItem.Storage.VarStorage.GetHashCode()}");
#endif

                var targetValue = storageItem.Storage.VarStorage.GetSystemValueDirectly(varName);

#if DEBUG
                //Log($"targetValue = {targetValue}");
#endif

                if(targetValue != null)
                {
                    return targetValue;
                }
            }

            throw new NotImplementedException();
        }

        private Value GetUsualVarValue(StrongIdentifierValue varName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Log($"varName = {varName}");
#endif

            var varPtr = Resolve(varName, localCodeExecutionContext, _defaultOptions);

#if DEBUG
            Log($"varPtr = {varPtr}");
#endif

            if (varPtr == null)
            {
                varPtr = CreateAndSaveLocalVariable(varName, localCodeExecutionContext);
            }

#if DEBUG
            Log($"varPtr (after) = {varPtr}");
#endif

            return varPtr.Value;
        }

        public Var Resolve(StrongIdentifierValue varName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"varName = {varName}");
            //Log($"localCodeExecutionContext = {localCodeExecutionContext}");
            //Log($"localCodeExecutionContext.OwnerStorage == null = {localCodeExecutionContext.OwnerStorage == null}");
#endif

            Var varPtr = null;

            if (localCodeExecutionContext.OwnerStorage != null)
            {
                varPtr = NResolve(varName, localCodeExecutionContext.Owner, localCodeExecutionContext.OwnerStorage, true, localCodeExecutionContext, options);
            }

#if DEBUG
            //Log($"varPtr = {varPtr}");
#endif

            if(varPtr != null)
            {
                return varPtr;
            }

            return NResolve(varName, localCodeExecutionContext.Holder, localCodeExecutionContext.Storage, false, localCodeExecutionContext, options);
        }

        private Var NResolve(StrongIdentifierValue varName, StrongIdentifierValue holder, IStorage storage, bool privateOnly, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"varName = {varName}");
            //Log($"localCodeExecutionContext = {localCodeExecutionContext}");
            //Log($"holder = {holder}");
            //Log($"privateOnly = {privateOnly}");
#endif

            //var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

#if DEBUG
            //foreach (var tmpStorage in storagesList)
            //{
                //Log($"tmpStorage = {tmpStorage}");
                //Log($"tmpStorage.Storage.Kind = {tmpStorage.Storage.Kind}");
            //}
#endif

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawVarsList(varName, storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            if (!rawList.Any())
            {
                return null;
            }

            if(privateOnly)
            {
                rawList = rawList.Where(p => p.ResultItem.TypeOfAccess == TypeOfAccess.Private).ToList();

#if DEBUG
                //Log($"rawList (after) = {rawList.WriteListToString()}");
#endif
            }

            var filteredList = FilterCodeItems(rawList, holder, localCodeExecutionContext);

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

            var maxStogageDistance = filteredList.Max(p => p.StorageDistance);

#if DEBUG
            //Log($"maxStogageDistance = {maxStogageDistance}");
#endif

            filteredList = filteredList.Where(p => p.StorageDistance == maxStogageDistance).ToList();

#if DEBUG
            //Log($"filteredList (after) = {filteredList.WriteListToString()}");
#endif

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return OrderAndDistinctByInheritance(filteredList, options).FirstOrDefault()?.ResultItem;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<Var>> GetRawVarsList(StrongIdentifierValue name, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            Log($"name = {name}");
#endif

            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<Var>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<Var>>();

            foreach (var storageItem in storagesList)
            {
#if DEBUG
                Log($"storageItem = {storageItem}");
                Log($"storageItem.Storage.Kind = {storageItem.Storage.Kind}");
                Log($"storageItem.Storage.TargetClassName = {storageItem.Storage.TargetClassName}");
                Log($"storageItem.Storage.InstanceName = {storageItem.Storage.InstanceName}");
                //Log($"storageItem.Storage.VarStorage.GetHashCode() = {storageItem.Storage.VarStorage.GetHashCode()}; storageItem.Storage.VarStorage.Kind = {storageItem.Storage.VarStorage.Kind}");
#endif

                var itemsList = storageItem.Storage.VarStorage.GetVarDirectly(name, weightedInheritanceItems);

#if DEBUG
                Log($"itemsList = {itemsList.WriteListToString()}");
#endif

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
