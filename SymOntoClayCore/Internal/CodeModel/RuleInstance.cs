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

using NLog;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Storage.LogicalStoraging;
using SymOntoClay.Core.Internal.Storage.SynonymsStoraging;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class RuleInstance: CodeItem, IStorage, ILogicalStorage
    {
        //public RuleInstance()
        //{
        //    TypeOfAccess = DefaultTypeOfAccess;
        //}

#if DEBUG
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.RuleOrFact;

        /// <inheritdoc/>
        public override bool IsRuleInstance => true;

        /// <inheritdoc/>
        public override RuleInstance AsRuleInstance => this;

        public bool IsSource { get; set; } = true;

        public bool IsParameterized { get; set; }

        public KindOfRuleInstance KindOfRuleInstance { get; set; } = KindOfRuleInstance.Undefined;
        public PrimaryRulePart PrimaryPart { get; set; }
        public IList<SecondaryRulePart> SecondaryParts { get; set; } = new List<SecondaryRulePart>();

        [Modality]
        [ResolveToType(typeof(LogicalValue))]        
        public Value ObligationModality { get; set; }        

        [Modality]
        [ResolveToType(typeof(LogicalValue))]
        public Value SelfObligationModality { get; set; }

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

            if(KindOfRuleInstance == KindOfRuleInstance.Undefined)
            {
                if(SecondaryParts.IsNullOrEmpty())
                {
                    KindOfRuleInstance = KindOfRuleInstance.Fact;
                }
                else
                {
                    KindOfRuleInstance = KindOfRuleInstance.Rule;
                }
            }

            if (IsSource)
            {
#if DEBUG
                //_gbcLogger.Info($"(options != null) = {options != null}");
                //_gbcLogger.Info($"this = {DebugHelperForRuleInstance.ToString(this)}");
#endif

                NPrepareDirty();

                Normalized = ConverterToNormalized.Convert(this, options);
                Normalized.PrepareDirty(options);

#if DEBUG
                //_gbcLogger.Info($"Normalized = {DebugHelperForRuleInstance.ToString(Normalized)}");
#endif               
            }
            else
            {
                CalculateUsedKeys();
                NPrepareDirty();

                Normalized = this;
            }

            if (PrimaryPart != null)
            {
                if (PrimaryPart.IsParameterized)
                {
                    IsParameterized = true;
                }
            }

            if (!SecondaryParts.IsNullOrEmpty())
            {
                foreach (var item in SecondaryParts)
                {
                    if (item.IsParameterized)
                    {
                        IsParameterized = true;
                    }
                }
            }

            _commonPersistIndexedLogicalData.NSetIndexedRuleInstanceToIndexData(Normalized);
        }

        private void NPrepareDirty()
        {
            PrimaryPart?.PrepareDirty(this);

            if (!SecondaryParts.IsNullOrEmpty())
            {
                foreach (var item in SecondaryParts)
                {
                    item.PrepareDirty(this);
                }
            }

            ObligationModality?.CheckDirty();
            SelfObligationModality?.CheckDirty();
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

        public void ResolveVariables(IPackedVarsResolver varsResolver)
        {
            if (PrimaryPart != null)
            {
                if (PrimaryPart.IsParameterized)
                {
                    PrimaryPart.ResolveVariables(varsResolver);
                }
            }

            if (!SecondaryParts.IsNullOrEmpty())
            {
                foreach (var item in SecondaryParts)
                {
                    if (item.IsParameterized)
                    {
                        item.ResolveVariables(varsResolver);
                    }
                }
            }

            IsParameterized = false;
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
        protected override void OnHolderChanged()
        {
            base.OnHolderChanged();

            if (PrimaryPart != null)
            {
                PrimaryPart.SetHolder(Holder);
            }

            if (!SecondaryParts.IsNullOrEmpty())
            {
                foreach (var item in SecondaryParts)
                {
                    item.SetHolder(Holder);
                }
            }

#if DEBUG
            //_gbcLogger.Info($"Normalized != null = {Normalized != null}; Normalized != this = {Normalized != this}");
#endif

            if(Normalized != null && Normalized != this)
            {
                Normalized.Holder = Holder;
            }
        }

        /// <inheritdoc/>
        protected override void OnTypeOfAccessChanged()
        {
            base.OnTypeOfAccessChanged();

            if (PrimaryPart != null)
            {
                PrimaryPart.SetTypeOfAccess(TypeOfAccess);
            }

            if (!SecondaryParts.IsNullOrEmpty())
            {
                foreach (var item in SecondaryParts)
                {
                    item.SetTypeOfAccess(TypeOfAccess);
                }
            }

            if (Normalized != null && Normalized != this)
            {
                Normalized.TypeOfAccess = TypeOfAccess;
            }
        }

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> cloneContext)
        {
            return Clone(cloneContext);
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
            result.KindOfRuleInstance = KindOfRuleInstance;
            result.PrimaryPart = PrimaryPart.Clone(context);
            result.SecondaryParts = SecondaryParts?.Select(p => p.Clone(context)).ToList();
            result.IsParameterized = IsParameterized;
            result.ObligationModality = ObligationModality;
            result.SelfObligationModality = SelfObligationModality;
            result.UsedKeysList = UsedKeysList?.ToList();

            result.AppendCodeItem(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
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
            if(KindOfRuleInstance != KindOfRuleInstance.Fact)
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
            sb.AppendLine($"{spaces}{nameof(IsParameterized)} = {IsParameterized}");

            sb.AppendLine($"{spaces}{nameof(KindOfRuleInstance)} = {KindOfRuleInstance}");

            sb.PrintObjProp(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintObjListProp(n, nameof(SecondaryParts), SecondaryParts);

            sb.PrintObjProp(n, nameof(ObligationModality), ObligationModality);
            sb.PrintObjProp(n, nameof(SelfObligationModality), SelfObligationModality);

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
            sb.AppendLine($"{spaces}{nameof(IsParameterized)} = {IsParameterized}");

            sb.AppendLine($"{spaces}{nameof(KindOfRuleInstance)} = {KindOfRuleInstance}");

            sb.PrintShortObjProp(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintShortObjListProp(n, nameof(SecondaryParts), SecondaryParts);

            sb.PrintShortObjProp(n, nameof(ObligationModality), ObligationModality);
            sb.PrintShortObjProp(n, nameof(SelfObligationModality), SelfObligationModality);

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
            sb.AppendLine($"{spaces}{nameof(IsParameterized)} = {IsParameterized}");

            sb.AppendLine($"{spaces}{nameof(KindOfRuleInstance)} = {KindOfRuleInstance}");

            sb.PrintExisting(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintExistingList(n, nameof(SecondaryParts), SecondaryParts);

            sb.PrintExisting(n, nameof(ObligationModality), ObligationModality);
            sb.PrintExisting(n, nameof(SelfObligationModality), SelfObligationModality);

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

            return $"{spaces}{ToHumanizedString()}";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return DebugHelperForRuleInstance.ToString(this, options);
        }

        #region IStorage
        /// <inheritdoc/>
        KindOfStorage IStorage.Kind => KindOfStorage.Sentence;

        /// <inheritdoc/>
        ILogicalStorage IStorage.LogicalStorage => this;

        /// <inheritdoc/>
        IRelationsStorage IStorage.RelationsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IMethodsStorage IStorage.MethodsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IActionsStorage IStorage.ActionsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IStatesStorage IStorage.StatesStorage => throw new NotImplementedException();
        
        /// <inheritdoc/>
        ITriggersStorage IStorage.TriggersStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IInheritanceStorage IStorage.InheritanceStorage => throw new NotImplementedException();

        private ISynonymsStorage _synonymsStorage;
        private readonly object _synonymsStorageLockObj = new object();

        /// <inheritdoc/>
        ISynonymsStorage IStorage.SynonymsStorage
        {
            get
            {
                lock(_synonymsStorageLockObj)
                {
                    if(_synonymsStorage == null)
                    {
                        _synonymsStorage = new EmptySynonymsStorage(this);
                    }

                    return _synonymsStorage;
                }
            }
        }

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
                UseInheritanceFacts = true
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
        void ILogicalStorage.Append(IList<RuleInstance> ruleInstancesList) => throw new NotImplementedException();

        /// <inheritdoc/>
        void ILogicalStorage.Remove(RuleInstance ruleInstance) => throw new NotImplementedException();

        /// <inheritdoc/>
        void ILogicalStorage.Remove(IList<RuleInstance> ruleInstancesList) => throw new NotImplementedException();

        /// <inheritdoc/>
        void ILogicalStorage.RemoveById(string id) => throw new NotImplementedException();

        /// <inheritdoc/>
        public event Action OnChanged;

        /// <inheritdoc/>
        public event Action<IList<StrongIdentifierValue>> OnChangedWithKeys;

        /// <inheritdoc/>
        public event Func<RuleInstance, IAddFactOrRuleResult> OnAddingFact;

        /// <inheritdoc/>
        IList<LogicalQueryNode> ILogicalStorage.GetAllRelations(ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
        {
            LogicalSearchExplainNode currentExplainNode = null;

            if (parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.LogicalStorage,
                    LogicalStorage = this
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
            }

            return _commonPersistIndexedLogicalData.GetAllRelations();
        }

        /// <inheritdoc/>
        IList<RuleInstance> ILogicalStorage.GetAllOriginFacts()
        {
            if(KindOfRuleInstance == KindOfRuleInstance.Fact)
            {
                return new List<RuleInstance>() { this };
            }

            return new List<RuleInstance>();
        }

        /// <inheritdoc/>
        IList<BaseRulePart> ILogicalStorage.GetIndexedRulePartOfFactsByKeyOfRelation(StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
        {
#if DEBUG
            //LogInstance.Log($"key = {key}");
#endif

            LogicalSearchExplainNode currentExplainNode = null;

            if(parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.LogicalStorage,
                    Key = name,
                    LogicalStorage = this
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
            }

            return _commonPersistIndexedLogicalData.GetIndexedRulePartOfFactsByKeyOfRelation(name);
        }

        /// <inheritdoc/>
        IList<BaseRulePart> ILogicalStorage.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
        {
            LogicalSearchExplainNode currentExplainNode = null;

            if (parentExplainNode != null)
            {
                currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                {
                    Kind = KindOfLogicalSearchExplainNode.LogicalStorage,
                    Key = name,
                    LogicalStorage = this
                };

                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
            }

            return _commonPersistIndexedLogicalData.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(name);
        }

        /// <inheritdoc/>
        void ILogicalStorage.DbgPrintFactsAndRules() => throw new NotImplementedException();

        #endregion
    }
}
