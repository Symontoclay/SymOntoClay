using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class ProcessInitialInfo: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public CompiledFunctionBody CompiledFunctionBody { get; set; }
        public LocalCodeExecutionContext LocalContext { get; set; }
        public CodeEntity Metadata { get; set; }

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
            var nextN = n + 4;
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.PrintObjProp(n, nameof(LocalContext), LocalContext);

            sb.PrintObjProp(n, nameof(Metadata), Metadata);

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
            var nextN = n + 4;
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.PrintShortObjProp(n, nameof(LocalContext), LocalContext);

            sb.PrintShortObjProp(n, nameof(Metadata), Metadata);

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
            var nextN = n + 4;
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.PrintBriefObjProp(n, nameof(LocalContext), LocalContext);

            sb.PrintBriefObjProp(n, nameof(Metadata), Metadata);

            return sb.ToString();
        }
    }
}
