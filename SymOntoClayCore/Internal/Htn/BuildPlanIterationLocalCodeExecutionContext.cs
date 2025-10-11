using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace SymOntoClay.Core.Internal.Htn
{
    public class BuildPlanIterationLocalCodeExecutionContext: ILocalCodeExecutionContext
    {
        private BuildPlanIterationLocalCodeExecutionContext()
        {
        }

        public BuildPlanIterationLocalCodeExecutionContext(IEngineContext context, ILocalCodeExecutionContext parent)
        {
            _context = context;
            Parent = parent;
            _storage = new BuildPlanIterationStorage(context.Logger, context, parent.Storage);
        }

        private IEngineContext _context;

        /// <inheritdoc/>
        public ILocalCodeExecutionContext Parent { get; private set; }

        /// <inheritdoc/>
        public bool UseParentInResolving => true;

        /// <inheritdoc/>
        public bool IsIsolated => Parent.IsIsolated;

        /// <inheritdoc/>
        public bool IsReal => false;

        /// <inheritdoc/>
        public StrongIdentifierValue Holder => Parent.Holder;

        private BuildPlanIterationStorage _storage;

        /// <inheritdoc/>
        public IStorage Storage => _storage;

        /// <inheritdoc/>
        public IInstance Instance => Parent.Instance;

        /// <inheritdoc/>
        public StrongIdentifierValue Owner => Parent.Owner;

        /// <inheritdoc/>
        public IStorage OwnerStorage => Parent.OwnerStorage;

        /// <inheritdoc/>
        public KindOfLocalCodeExecutionContext Kind => KindOfLocalCodeExecutionContext.HtnBuildPlanIteration;

        /// <inheritdoc/>
        public KindOfAddFactOrRuleResult KindOfAddFactResult { get; set; }

        /// <inheritdoc/>
        public RuleInstanceReference AddedRuleInstance => Parent.AddedRuleInstance;

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public BuildPlanIterationLocalCodeExecutionContext Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public BuildPlanIterationLocalCodeExecutionContext Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (BuildPlanIterationLocalCodeExecutionContext)context[this];
            }

            var result = new BuildPlanIterationLocalCodeExecutionContext();
            context[this] = result;

            result.Parent = Parent;
            result._context = _context;
            result._storage = _storage.Clone(context);

            return result;
        }

        /// <inheritdoc/>
        public ILocalCodeExecutionContext GetFirstNonRealItemFromChain()
        {
            return this;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Parent), Parent);
            sb.AppendLine($"{spaces}{nameof(UseParentInResolving)} = {UseParentInResolving}");
            sb.AppendLine($"{spaces}{nameof(IsIsolated)} = {IsIsolated}");
            sb.AppendLine($"{spaces}{nameof(IsReal)} = {IsReal}");
            sb.PrintObjProp(n, nameof(Holder), Holder);
            sb.AppendLine($"{spaces}{nameof(Storage)}.{nameof(Storage.Kind)} = {Storage?.Kind}");
            sb.AppendLine($"{spaces}{nameof(Instance)}.{nameof(Instance.Name)} = {Instance?.Name?.ToHumanizedLabel()}");
            sb.PrintObjProp(n, nameof(Owner), Owner);
            sb.AppendLine($"{spaces}{nameof(OwnerStorage)}.{nameof(OwnerStorage.Kind)} = {OwnerStorage?.Kind}");
            sb.AppendLine($"{spaces}{nameof(OwnerStorage)}.{nameof(OwnerStorage.TargetClassName)} = {OwnerStorage?.TargetClassName?.ToHumanizedLabel()}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfAddFactResult)} = {KindOfAddFactResult}");
            sb.PrintObjProp(n, nameof(AddedRuleInstance), AddedRuleInstance);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Parent), Parent);
            sb.AppendLine($"{spaces}{nameof(UseParentInResolving)} = {UseParentInResolving}");
            sb.AppendLine($"{spaces}{nameof(IsIsolated)} = {IsIsolated}");
            sb.AppendLine($"{spaces}{nameof(IsReal)} = {IsReal}");
            sb.PrintShortObjProp(n, nameof(Holder), Holder);
            sb.AppendLine($"{spaces}{nameof(Storage)}.{nameof(Storage.Kind)} = {Storage?.Kind}");
            sb.AppendLine($"{spaces}{nameof(Instance)}.{nameof(Instance.Name)} = {Instance?.Name?.ToHumanizedLabel()}");
            sb.PrintShortObjProp(n, nameof(Owner), Owner);
            sb.AppendLine($"{spaces}{nameof(OwnerStorage)}.{nameof(OwnerStorage.Kind)} = {OwnerStorage?.Kind}");
            sb.AppendLine($"{spaces}{nameof(OwnerStorage)}.{nameof(OwnerStorage.TargetClassName)} = {OwnerStorage?.TargetClassName?.ToHumanizedLabel()}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfAddFactResult)} = {KindOfAddFactResult}");
            sb.PrintShortObjProp(n, nameof(AddedRuleInstance), AddedRuleInstance);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintExisting(n, nameof(Parent), Parent);
            sb.AppendLine($"{spaces}{nameof(UseParentInResolving)} = {UseParentInResolving}");
            sb.AppendLine($"{spaces}{nameof(IsIsolated)} = {IsIsolated}");
            sb.AppendLine($"{spaces}{nameof(IsReal)} = {IsReal}");
            sb.PrintBriefObjProp(n, nameof(Holder), Holder);
            sb.AppendLine($"{spaces}{nameof(Storage)}.{nameof(Storage.Kind)} = {Storage?.Kind}");
            sb.PrintBriefObjProp(n, nameof(Owner), Owner);
            sb.AppendLine($"{spaces}{nameof(Instance)}.{nameof(Instance.Name)} = {Instance?.Name?.ToHumanizedLabel()}");
            sb.AppendLine($"{spaces}{nameof(OwnerStorage)}.{nameof(OwnerStorage.Kind)} = {OwnerStorage?.Kind}");
            sb.AppendLine($"{spaces}{nameof(OwnerStorage)}.{nameof(OwnerStorage.TargetClassName)} = {OwnerStorage?.TargetClassName?.ToHumanizedLabel()}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfAddFactResult)} = {KindOfAddFactResult}");
            sb.PrintBriefObjProp(n, nameof(AddedRuleInstance), AddedRuleInstance);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public void DbgPrintContextChain(IMonitorLogger logger, string messagePointId)
        {
            logger.Info(messagePointId, "Begin ContextChain");

            ILocalCodeExecutionContext tmpLocalCodeExecutionContext = this;

            while (tmpLocalCodeExecutionContext != null)
            {
                logger.Info(messagePointId, $"tmpLocalCodeExecutionContext = {tmpLocalCodeExecutionContext}");

                tmpLocalCodeExecutionContext = tmpLocalCodeExecutionContext.Parent;
            }

            logger.Info(messagePointId, "End ContextChain");
        }
    }
}
