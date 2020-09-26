using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class OptionsOfFillExecutingCard : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public bool EntityIdOnly { get; set; }
        public bool UseAccessPolicy { get; set; }
        public bool UseInheritance { get; set; }
        public InheritanceResolver InheritanceResolver { get; set; }
        public LocalCodeExecutionContext LocalCodeExecutionContext { get; set; }

#if DEBUG
        public IEntityLogger Logger { get; set; }
        public IEntityDictionary EntityDictionary { get; set; }
#endif

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

            sb.AppendLine($"{spaces}{nameof(EntityIdOnly)} = {EntityIdOnly}");
            sb.AppendLine($"{spaces}{nameof(UseAccessPolicy)} = {UseAccessPolicy}");
            sb.AppendLine($"{spaces}{nameof(UseInheritance)} = {UseInheritance}");            

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

            sb.AppendLine($"{spaces}{nameof(EntityIdOnly)} = {EntityIdOnly}");
            sb.AppendLine($"{spaces}{nameof(UseAccessPolicy)} = {UseAccessPolicy}");
            sb.AppendLine($"{spaces}{nameof(UseInheritance)} = {UseInheritance}");

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

            sb.AppendLine($"{spaces}{nameof(EntityIdOnly)} = {EntityIdOnly}");
            sb.AppendLine($"{spaces}{nameof(UseAccessPolicy)} = {UseAccessPolicy}");
            sb.AppendLine($"{spaces}{nameof(UseInheritance)} = {UseInheritance}");

            return sb.ToString();
        }
    }
}
