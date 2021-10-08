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
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Storage.LogicalStorage;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class RuleInstance: AnnotatedItem, IStorage, ILogicalStorage
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public bool IsSource { get; set; } = true;

        public StrongIdentifierValue Name { get; set; }
        public KindOfRuleInstance Kind { get; set; } = KindOfRuleInstance.Undefined;
        public PrimaryRulePart PrimaryPart { get; set; }
        public IList<SecondaryRulePart> SecondaryParts { get; set; } = new List<SecondaryRulePart>();
        
        public List<StrongIdentifierValue> UsedKeysList { get; set; }

        public RuleInstance Original { get; set; }
        public RuleInstance Normalized { get; set; }

        private readonly CommonPersistIndexedLogicalData _commonPersistIndexedLogicalData = new CommonPersistIndexedLogicalData();

        private void PrepareDirty(CheckDirtyOptions options)
        {
            if(Name == null || Name.IsEmpty)
            {
                Name = NameHelper.CreateRuleOrFactName();
            }

            if(Kind == KindOfRuleInstance.Undefined)
            {
                if(SecondaryParts.IsNullOrEmpty())
                {
                    Kind = KindOfRuleInstance.Fact;
                }
                else
                {
                    Kind = KindOfRuleInstance.Rule;
                }
            }

            if (IsSource)
            {
#if DEBUG
                _gbcLogger.Info($"this = {DebugHelperForRuleInstance.ToString(this)}");
#endif 

                Normalized = ConvertorToNormalized.Convert(this, options);
                Normalized.PrepareDirty(options);

#if DEBUG
                _gbcLogger.Info($"Normalized = {DebugHelperForRuleInstance.ToString(Normalized)}");
#endif               
            }
            else
            {
                CalculateUsedKeys();
                PrimaryPart?.PrepareDirty(this);

                if (!SecondaryParts.IsNullOrEmpty())
                {
                    foreach (var item in SecondaryParts)
                    {
                        item.PrepareDirty(this);
                    }
                }

                Normalized = this;
            }

            _commonPersistIndexedLogicalData.NSetIndexedRuleInstanceToIndexData(Normalized);
        }

        public void CalculateUsedKeys()
        {
            var usedKeysList = new List<StrongIdentifierValue>();

            PrimaryPart.CalculateUsedKeys(usedKeysList);

            if (!SecondaryParts.IsNullOrEmpty())
            {
                foreach (var secondaryPart in SecondaryParts)
                {
                    secondaryPart.CalculateUsedKeys(usedKeysList);
                }
            }

            UsedKeysList = usedKeysList.Distinct().ToList();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            PrepareDirty(options);

            var result = base.CalculateLongHashCode(options) ^ PrimaryPart.GetLongHashCode(options);

            if (!SecondaryParts.IsNullOrEmpty())
            {
                foreach (var secondaryPart in SecondaryParts)
                {
                    result ^= secondaryPart.GetLongHashCode(options);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public RuleInstance Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public RuleInstance Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (RuleInstance)context[this];
            }

            var result = new RuleInstance();
            context[this] = result;

            result.IsSource = IsSource;
            result.Name = Name.Clone(context);
            result.Kind = Kind;
            result.PrimaryPart = PrimaryPart.Clone(context);
            result.SecondaryParts = SecondaryParts?.Select(p => p.Clone(context)).ToList();
            UsedKeysList = UsedKeysList.ToList();

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            PrimaryPart?.DiscoverAllAnnotations(result);

            if (!SecondaryParts.IsNullOrEmpty())
            {
                foreach(var item in SecondaryParts)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }
        }

        public IList<LogicalQueryNode> GetInheritanceRelations()
        {
            if(Kind != KindOfRuleInstance.Fact)
            {
                return new List<LogicalQueryNode>();
            }

            return PrimaryPart.GetInheritanceRelations();            
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSource)} = {IsSource}");

            sb.PrintObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintObjProp(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintObjListProp(n, nameof(SecondaryParts), SecondaryParts);

            sb.PrintObjListProp(n, nameof(UsedKeysList), UsedKeysList);

            sb.PrintExisting(n, nameof(Original), Original);
            sb.PrintExisting(n, nameof(Normalized), Normalized);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSource)} = {IsSource}");

            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintShortObjProp(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintShortObjListProp(n, nameof(SecondaryParts), SecondaryParts);

            sb.PrintShortObjListProp(n, nameof(UsedKeysList), UsedKeysList);

            sb.PrintExisting(n, nameof(Original), Original);
            sb.PrintExisting(n, nameof(Normalized), Normalized);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSource)} = {IsSource}");

            sb.PrintBriefObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintExisting(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintExistingList(n, nameof(SecondaryParts), SecondaryParts);

            sb.PrintBriefObjListProp(n, nameof(UsedKeysList), UsedKeysList);

            sb.PrintExisting(n, nameof(Original), Original);
            sb.PrintExisting(n, nameof(Normalized), Normalized);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);

            return $"{spaces}{DebugHelperForRuleInstance.ToString(this)}";
        }

        #region IStorage
        /// <inheritdoc/>
        KindOfStorage IStorage.Kind => KindOfStorage.Sentence;

        /// <inheritdoc/>
        ILogicalStorage IStorage.LogicalStorage => this;

        /// <inheritdoc/>
        IMethodsStorage IStorage.MethodsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IActionsStorage IStorage.ActionsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        ITriggersStorage IStorage.TriggersStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IInheritanceStorage IStorage.InheritanceStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        ISynonymsStorage IStorage.SynonymsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IOperatorsStorage IStorage.OperatorsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IChannelsStorage IStorage.ChannelsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IMetadataStorage IStorage.MetadataStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IVarStorage IStorage.VarStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IFuzzyLogicStorage IStorage.FuzzyLogicStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        void IStorage.AddParentStorage(IStorage storage) => throw new NotImplementedException();

        /// <inheritdoc/>
        void IStorage.RemoveParentStorage(IStorage storage) => throw new NotImplementedException();

        /// <inheritdoc/>
        void IStorage.CollectChainOfStorages(IList<StorageUsingOptions> result, IList<IStorage> usedStorages, int level, CollectChainOfStoragesOptions options)
        {
            level++;

            var item = new StorageUsingOptions()
            {
                Priority = level,
                Storage = this,
                UseFacts = true,
                UseProductions = true,
                UseAdditionalInstances = true
            };

            if(options != null)
            {
                if(options.UseFacts.HasValue)
                {
                    item.UseFacts = options.UseFacts.Value;
                }
            }

            result.Add(item);
        }

        /// <inheritdoc/>
        void IStorage.CollectChainOfStorages(IList<IStorage> result) => throw new NotImplementedException();

        /// <inheritdoc/>
        IList<IStorage> IStorage.GetStorages() => throw new NotImplementedException();

        /// <inheritdoc/>
        DefaultSettingsOfCodeEntity IStorage.DefaultSettingsOfCodeEntity { get; set; }
#if DEBUG
        /// <inheritdoc/>
        void IStorage.DbgPrintFactsAndRules() => throw new NotImplementedException();
#endif
        #endregion

        #region ILogicalStorage
        /// <inheritdoc/>
        KindOfStorage ISpecificStorage.Kind => KindOfStorage.Sentence;

        /// <inheritdoc/>
        IStorage ISpecificStorage.Storage => this;

        /// <inheritdoc/>
        void ILogicalStorage.Append(RuleInstance ruleInstance) => throw new NotImplementedException();

        /// <inheritdoc/>
        void ILogicalStorage.Append(RuleInstance ruleInstance, bool isPrimary) => throw new NotImplementedException();

        /// <inheritdoc/>
        void ILogicalStorage.Remove(RuleInstance ruleInstance) => throw new NotImplementedException();

        /// <inheritdoc/>
        void ILogicalStorage.RemoveById(string id) => throw new NotImplementedException();

        /// <inheritdoc/>
        public event Action OnChanged;

        /// <inheritdoc/>
        public event Action<IList<StrongIdentifierValue>> OnChangedWithKeys;

        /// <inheritdoc/>
        IList<LogicalQueryNode> ILogicalStorage.GetAllRelations()
        {
            return _commonPersistIndexedLogicalData.GetAllRelations();
        }

        /// <inheritdoc/>
        IList<BaseRulePart> ILogicalStorage.GetIndexedRulePartOfFactsByKeyOfRelation(StrongIdentifierValue name)
        {
#if DEBUG
            //LogInstance.Log($"key = {key}");
#endif

            return _commonPersistIndexedLogicalData.GetIndexedRulePartOfFactsByKeyOfRelation(name);
        }

        /// <inheritdoc/>
        IList<BaseRulePart> ILogicalStorage.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(StrongIdentifierValue name)
        {
            return _commonPersistIndexedLogicalData.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(name);
        }

        /// <inheritdoc/>
        void ILogicalStorage.DbgPrintFactsAndRules() => throw new NotImplementedException();

        #endregion
    }
}
