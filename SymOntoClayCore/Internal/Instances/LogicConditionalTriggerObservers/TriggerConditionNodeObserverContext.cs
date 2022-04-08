using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public class TriggerConditionNodeObserverContext: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public TriggerConditionNodeObserverContext(IEngineContext engineContext, IStorage storage, StrongIdentifierValue holder)
        {
            EngineContext = engineContext;
            Storage = storage;
            Holder = holder;
        }

        public IEngineContext EngineContext { get; private set; }
        public IStorage Storage { get; private set; }
        public StrongIdentifierValue Holder { get; private set; }

        public long? SetSeconds { get; set; }

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

            sb.PrintObjProp(n, nameof(Holder), Holder);
            sb.AppendLine($"{spaces}{nameof(SetSeconds)} = {SetSeconds}");

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

            sb.PrintShortObjProp(n, nameof(Holder), Holder);
            sb.AppendLine($"{spaces}{nameof(SetSeconds)} = {SetSeconds}");

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

            sb.PrintBriefObjProp(n, nameof(Holder), Holder);
            sb.AppendLine($"{spaces}{nameof(SetSeconds)} = {SetSeconds}");

            return sb.ToString();
        }
    }
}
