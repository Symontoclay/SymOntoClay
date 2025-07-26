using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class CompoundHtnTaskBackgroundTriggerInstance : BaseComponent,
        IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public CompoundHtnTaskBackgroundTriggerInstance(CompoundHtnTaskBackground background, BaseInstance parent, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext)
            : base(context.Logger)
        {
            Id = NameHelper.GetNewEntityNameString();
        }

        public string Id { get; }

        public void Init(IMonitorLogger logger)
        {
            //throw new NotImplementedException("1D3F038F-04A6-4CAC-8221-97037FDDD577");
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
            sb.Append($"{spaces}{nameof(Id)} = {Id}");
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
            sb.Append($"{spaces}{nameof(Id)} = {Id}");
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
            sb.Append($"{spaces}{nameof(Id)} = {Id}");
            return sb.ToString();
        }
    }
}
