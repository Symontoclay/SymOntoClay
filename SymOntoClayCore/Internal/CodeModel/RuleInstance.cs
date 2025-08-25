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
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.CodeModel.MonitorSerializableObjects;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage.LogicalStoraging;
using SymOntoClay.Core.Internal.Storage.SynonymsStoraging;
using SymOntoClay.Core.Internal.Visitors;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class RuleInstance: CodeItem, IStorage, ILogicalStorage//, IEquatable<RuleInstance>
    {
#if DEBUG
        //private static readonly Logger _staticLogger = LogManager.GetCurrentClassLogger();
#endif

        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.RuleOrFact;

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.RuleInstance;

        /// <inheritdoc/>
        public override bool IsRuleInstance => true;

        /// <inheritdoc/>
        public override RuleInstance AsRuleInstance => this;

        private List<StrongIdentifierValue> _builtInSuperTypes;

        /// <inheritdoc/>
        public override IReadOnlyList<StrongIdentifierValue> BuiltInSuperTypes => _builtInSuperTypes;

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

        private ulong? _timeStamp;

        public ulong? TimeStamp 
        { 
            get
            {
                return _timeStamp;
            }

            set
            {
                if(_timeStamp == value)
                {
                    return;
                }

                _timeStamp = value;

                if(Normalized != null)
                {
                    Normalized.TimeStamp = _timeStamp;
                }
                
                if(Original != null)
                {
                    Original.TimeStamp = _timeStamp;
                }
            }
        }

        public List<StrongIdentifierValue> UsedKeysList { get; set; }

        public List<LogicalQueryNode> LeavesList { get; set; }
        
        public RuleInstance Original { get; set; }
        public RuleInstance Normalized { get; set; }

        private readonly CommonPersistIndexedLogicalData _commonPersistIndexedLogicalData = new CommonPersistIndexedLogicalData();

        public void AddLogicalStorage(ILogicalStorage logicalStorage)
        {
            if(LogicalStorages.Contains(logicalStorage))
            {
                return;
            }

            LogicalStorages.Add(logicalStorage);
        }

        public void RemoveFromLogicalStorage(ILogicalStorage logicalStorage)
        {
            if(LogicalStorages.Contains(logicalStorage))
            {
                LogicalStorages.Remove(logicalStorage);
            }
        }

        public List<ILogicalStorage> LogicalStorages { get; private set; } = new List<ILogicalStorage>();

        public void Accept(ILogicalVisitor logicalVisitor)
        {
            logicalVisitor.VisitRuleInstance(this);
        }

        /// <inheritdoc/>
        public override bool NullValueEquals()
        {
            return false;
        }

        private void PrepareDirty(CheckDirtyOptions options)
        {
#if DEBUG
            //_staticLogger.Info($"this = {this.ToHumanizedString()}");
#endif

            if (Name == null || Name.IsEmpty)
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
                NPrepareDirty();

                Normalized = ConverterToNormalized.Convert(this, options);
                Normalized.PrepareDirty(options);

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

            _commonPersistIndexedLogicalData.NSetIndexedRuleInstanceToIndexData(options?.EngineContext?.Logger, Normalized, true);
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

            var logicalQueryNodeLeavesVisitor = new LogicalQueryNodeLeavesVisitor();
            LeavesList = logicalQueryNodeLeavesVisitor.Run(this);

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

            if (SecondaryParts.IsNullOrEmpty())
            {
                _builtInSuperTypes = new List<StrongIdentifierValue>() { NameHelper.CreateName(StandardNamesConstants.FactTypeName) };
            }
            else
            {
                _builtInSuperTypes = new List<StrongIdentifierValue>() { NameHelper.CreateName(StandardNamesConstants.RuleTypeName) };

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
        public override CodeItem CloneCodeItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public RuleInstance Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
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
            result.TimeStamp = TimeStamp;
            result.LogicalStorages = LogicalStorages?.ToList();
            result.UsedKeysList = UsedKeysList?.ToList();
            result.LeavesList = LeavesList?.ToList();

            result.AppendCodeItem(this, context);

            result.Name = NameHelper.CreateRuleOrFactName();

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

        public IList<LogicalQueryNode> GetInheritanceRelations(IMonitorLogger logger)
        {
            if(KindOfRuleInstance != KindOfRuleInstance.Fact)
            {
                return new List<LogicalQueryNode>();
            }

            return PrimaryPart.GetInheritanceRelations(logger);
        }

        public IList<StrongIdentifierValue> GetStandaloneConcepts(IMonitorLogger logger)
        {
            if (KindOfRuleInstance != KindOfRuleInstance.Fact)
            {
                return new List<StrongIdentifierValue>();
            }

            return PrimaryPart.GetStandaloneConcepts(logger);
        }

        /// <inheritdoc/>
        public override object ToMonitorSerializableObject(IMonitorLogger logger)
        {
            var result = new RuleInstanceMonitorSerializableObject();

            result.LogicalQuery = ToHumanizedString();

            return result;
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

            sb.AppendLine($"{spaces}{nameof(TimeStamp)} = {TimeStamp}");

            sb.PrintBriefObjListProp(n, nameof(LogicalStorages), LogicalStorages);

            sb.PrintObjListProp(n, nameof(UsedKeysList), UsedKeysList);
            sb.PrintObjListProp(n, nameof(LeavesList), LeavesList);

            sb.PrintExisting(n, nameof(Original), Original);
            sb.PrintExisting(n, nameof(Normalized), Normalized);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
#if DEBUG
            //_staticLogger.Info($"-----------------------");
#endif

            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSource)} = {IsSource}");
            sb.AppendLine($"{spaces}{nameof(IsParameterized)} = {IsParameterized}");

            sb.AppendLine($"{spaces}{nameof(KindOfRuleInstance)} = {KindOfRuleInstance}");

            sb.PrintShortObjProp(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintShortObjListProp(n, nameof(SecondaryParts), SecondaryParts);

            sb.PrintShortObjProp(n, nameof(ObligationModality), ObligationModality);
            sb.PrintShortObjProp(n, nameof(SelfObligationModality), SelfObligationModality);

            sb.AppendLine($"{spaces}{nameof(TimeStamp)} = {TimeStamp}");

            sb.PrintBriefObjListProp(n, nameof(LogicalStorages), LogicalStorages);

            sb.PrintShortObjListProp(n, nameof(UsedKeysList), UsedKeysList);
            sb.PrintShortObjListProp(n, nameof(LeavesList), LeavesList);

            sb.PrintExisting(n, nameof(Original), Original);
            sb.PrintExisting(n, nameof(Normalized), Normalized);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
#if DEBUG
            //_staticLogger.Info($"%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
#endif

            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSource)} = {IsSource}");
            sb.AppendLine($"{spaces}{nameof(IsParameterized)} = {IsParameterized}");

            sb.AppendLine($"{spaces}{nameof(KindOfRuleInstance)} = {KindOfRuleInstance}");

            sb.PrintExisting(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintExisting(n, nameof(SecondaryParts), SecondaryParts);

            sb.PrintExisting(n, nameof(ObligationModality), ObligationModality);
            sb.PrintExisting(n, nameof(SelfObligationModality), SelfObligationModality);

            sb.AppendLine($"{spaces}{nameof(TimeStamp)} = {TimeStamp}");

            sb.PrintExisting(n, nameof(LogicalStorages), LogicalStorages);

            sb.PrintExisting(n, nameof(UsedKeysList), UsedKeysList);
            sb.PrintExisting(n, nameof(LeavesList), LeavesList);

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

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return ToHumanizedString(options);
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = ToHumanizedString()
            };
        }

        #region IStorage
        /// <inheritdoc/>
        KindOfStorage IStorage.Kind => KindOfStorage.Sentence;

        /// <inheritdoc/>
        StrongIdentifierValue IStorage.TargetClassName => null;

        /// <inheritdoc/>
        StrongIdentifierValue IStorage.InstanceName => null;

        /// <inheritdoc/>
        IInstance IStorage.Instance => null;

        /// <inheritdoc/>
        bool IStorage.IsIsolated => false;

        /// <inheritdoc/>
        ILogicalStorage IStorage.LogicalStorage => this;

        /// <inheritdoc/>
        IRelationsStorage IStorage.RelationsStorage => throw new NotImplementedException("DAF6015D-84A6-42F3-B03D-01D56205EC64");

        /// <inheritdoc/>
        IMethodsStorage IStorage.MethodsStorage => throw new NotImplementedException("CC8E2AAF-5878-468E-8952-D946CE01B5B1");

        /// <inheritdoc/>
        IConstructorsStorage IStorage.ConstructorsStorage => throw new NotImplementedException("C877D570-322F-4E63-B7CC-8104DBE64AF6");

        /// <inheritdoc/>
        IActionsStorage IStorage.ActionsStorage => throw new NotImplementedException("AB4BD863-5202-420B-923F-C9F4700CFD8E");

        /// <inheritdoc/>
        IStatesStorage IStorage.StatesStorage => throw new NotImplementedException("8C5C9066-1B1D-4E3B-99C5-D1CBB4415C65");
        
        /// <inheritdoc/>
        ITriggersStorage IStorage.TriggersStorage => throw new NotImplementedException("038703A1-633F-4C0C-92D4-BB9D66A9BFEA");

        /// <inheritdoc/>
        IInheritanceStorage IStorage.InheritanceStorage => throw new NotImplementedException("E54F6DF2-DA44-445D-A22E-B69A6E84AB6E");

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
        IOperatorsStorage IStorage.OperatorsStorage => throw new NotImplementedException("AC133155-99E8-477E-99D8-C2B7CDEF41E3");

        /// <inheritdoc/>
        IChannelsStorage IStorage.ChannelsStorage => throw new NotImplementedException("833361AC-4C18-484F-8ED7-53EFF11CF81F");

        /// <inheritdoc/>
        IMetadataStorage IStorage.MetadataStorage => throw new NotImplementedException("5E237BFF-7DA3-4961-9A78-E86BA0C5A75D");

        /// <inheritdoc/>
        IVarStorage IStorage.VarStorage => throw new NotImplementedException("43F08F2B-94BE-49C5-BBB1-1F9998106040");

        /// <inheritdoc/>
        IFuzzyLogicStorage IStorage.FuzzyLogicStorage => throw new NotImplementedException("4EAEA764-9850-46D1-A717-08627C482CD5");

        /// <inheritdoc/>
        IIdleActionItemsStorage IStorage.IdleActionItemsStorage => throw new NotImplementedException("F52AE3AB-FFA5-4267-822E-CE5B4D81E8D5");

        /// <inheritdoc/>
        ITasksStorage IStorage.TasksStorage => throw new NotImplementedException("B0054D41-6AAB-4FB5-9646-99E0200EBA81");

        /// <inheritdoc/>
        IPropertyStorage IStorage.PropertyStorage => throw new NotImplementedException("438657D2-2B3A-4004-80C8-CB255BAE8A35");

        /// <inheritdoc/>
        void IStorage.AddParentStorage(IMonitorLogger logger, IStorage storage) => throw new NotImplementedException("07C2DCC9-C3BD-49D9-A430-809B60BC5229");

        /// <inheritdoc/>
        void IStorage.RemoveParentStorage(IMonitorLogger logger, IStorage storage) => throw new NotImplementedException("F1816BBA-AE88-4E00-9FE2-12A2C3C61212");

        /// <inheritdoc/>
        void IStorage.CollectChainOfStorages(IMonitorLogger logger, IList<StorageUsingOptions> result, IList<IStorage> usedStorages, int level, CollectChainOfStoragesOptions options)
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
        void IStorage.CollectChainOfStorages(IMonitorLogger logger, IList<IStorage> result) => throw new NotImplementedException("63D026BF-9E19-4686-8284-5B294873B8A2");

        /// <inheritdoc/>
        IList<IStorage> IStorage.GetStorages(IMonitorLogger logger) => throw new NotImplementedException("A5D70BA4-8768-4F00-AA1C-651554189120");

        /// <inheritdoc/>
        DefaultSettingsOfCodeEntity IStorage.DefaultSettingsOfCodeEntity { get; set; }

        /// <inheritdoc/>
        public List<StorageUsingOptions> CodeItemsStoragesList { get; set; }

        void IStorage.AddOnParentStorageChangedHandler(IOnParentStorageChangedStorageHandler handler)
        {
        }

        void IStorage.RemoveOnParentStorageChangedHandler(IOnParentStorageChangedStorageHandler handler)
        {
        }

#if DEBUG
        /// <inheritdoc/>
        void IStorage.DbgPrintFactsAndRules(IMonitorLogger logger) => throw new NotImplementedException("3E406EDD-78EE-430D-BFC3-98F53DF9AF05");
#endif
#endregion

        #region ILogicalStorage
        /// <inheritdoc/>
        KindOfStorage ISpecificStorage.Kind => KindOfStorage.Sentence;

        /// <inheritdoc/>
        IStorage ISpecificStorage.Storage => this;

        /// <inheritdoc/>
        void ILogicalStorage.Append(IMonitorLogger logger, RuleInstance ruleInstance) => throw new NotImplementedException("058DA169-BAC8-4CF9-A8BF-6321B8479218");

        /// <inheritdoc/>
        void ILogicalStorage.Append(IMonitorLogger logger, RuleInstance ruleInstance, bool isPrimary) => throw new NotImplementedException("4778DF8E-35BC-40AF-825D-EE4D77925772");

        /// <inheritdoc/>
        void ILogicalStorage.Append(IMonitorLogger logger, IList<RuleInstance> ruleInstancesList) => throw new NotImplementedException("0E90B71B-A95C-468A-B735-4A20B73B89CC");

        /// <inheritdoc/>
        void ILogicalStorage.Remove(IMonitorLogger logger, RuleInstance ruleInstance) => throw new NotImplementedException("4490F7BA-CE10-4CF6-B564-058261F74D53");

        /// <inheritdoc/>
        void ILogicalStorage.Remove(IMonitorLogger logger, IList<RuleInstance> ruleInstancesList) => throw new NotImplementedException("40842BD0-B005-4B01-9B27-F3F4F6687450");

        /// <inheritdoc/>
        void ILogicalStorage.RemoveById(IMonitorLogger logger, string id) => throw new NotImplementedException("918EDBB9-8D25-4A79-8205-BB88C484DC6B");

        void ILogicalStorage.AddOnChangedHandler(IOnChangedLogicalStorageHandler handler)
        {
        }

        void ILogicalStorage.RemoveOnChangedHandler(IOnChangedLogicalStorageHandler handler)
        {
        }

        void ILogicalStorage.AddOnChangedWithKeysHandler(IOnChangedWithKeysLogicalStorageHandler handler)
        {
        }

        void ILogicalStorage.RemoveOnChangedWithKeysHandler(IOnChangedWithKeysLogicalStorageHandler handler)
        {
        }

        void ILogicalStorage.AddOnAddingFactHandler(IOnAddingFactHandler handler)
        {
        }

        void ILogicalStorage.RemoveOnAddingFactHandler(IOnAddingFactHandler handler)
        {
        }

        /// <inheritdoc/>
        IList<LogicalQueryNode> ILogicalStorage.GetAllRelations(IMonitorLogger logger, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
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

            return _commonPersistIndexedLogicalData.GetAllRelations(logger);
        }

        /// <inheritdoc/>
        IList<RuleInstance> ILogicalStorage.GetAllOriginFacts(IMonitorLogger logger)
        {
            if(KindOfRuleInstance == KindOfRuleInstance.Fact)
            {
                return new List<RuleInstance>() { this };
            }

            return new List<RuleInstance>();
        }

        /// <inheritdoc/>
        IList<BaseRulePart> ILogicalStorage.GetIndexedRulePartOfFactsByKeyOfRelation(IMonitorLogger logger, StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
        {
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

            return _commonPersistIndexedLogicalData.GetIndexedRulePartOfFactsByKeyOfRelation(logger, name);
        }

        /// <inheritdoc/>
        IList<BaseRulePart> ILogicalStorage.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(IMonitorLogger logger, StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
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

            return _commonPersistIndexedLogicalData.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(logger, name);
        }

        /// <inheritdoc/>
        public IReadOnlyList<LogicalQueryNode> GetLogicalQueryNodes(IMonitorLogger logger, IList<LogicalQueryNode> exceptList, ReplacingNotResultsStrategy replacingNotResultsStrategy, IList<KindOfLogicalQueryNode> targetKindsOfItems)
        {
            return _commonPersistIndexedLogicalData.GetLogicalQueryNodes(logger, exceptList, replacingNotResultsStrategy, targetKindsOfItems);
        }

        /// <inheritdoc/>
        void ILogicalStorage.DbgPrintFactsAndRules(IMonitorLogger logger) => throw new NotImplementedException("A7DE412A-843F-4B88-A488-01DF60860F46");

        #endregion
    }
}
