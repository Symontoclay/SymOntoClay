using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class AnnotationSystemEvent: IExecutable, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        private static List<IFunctionArgument> EmptyArgumentsList = new List<IFunctionArgument>();

        public KindOfAnnotationSystemEvent Kind { get; set; } = KindOfAnnotationSystemEvent.Unknown;
        public bool IsSync { get; set; } = true;
        public List<AstStatement> Statements { get; set; } = new List<AstStatement>();

        /// <inheritdoc/>
        public CompiledFunctionBody CompiledFunctionBody { get; set; }

        /// <inheritdoc/>
        public bool IsSystemDefined => true;

        /// <inheritdoc/>
        public IList<IFunctionArgument> Arguments => EmptyArgumentsList;

        /// <inheritdoc/>
        public CodeItem CodeItem => null;

        /// <inheritdoc/>
        public ISystemHandler SystemHandler => null;

        /// <inheritdoc/>
        public IExecutionCoordinator TryActivate(IEngineContext context)
        {
            return null;
        }

        /// <inheritdoc/>
        public bool ContainsArgument(StrongIdentifierValue name)
        {
            return false;
        }

        /// <inheritdoc/>
        public LocalCodeExecutionContext OwnLocalCodeExecutionContext => null;

        /// <inheritdoc/>
        public StrongIdentifierValue Holder => null;

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public AnnotationSystemEvent Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public AnnotationSystemEvent Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (AnnotationSystemEvent)context[this];
            }

            var result = new AnnotationSystemEvent();
            context[this] = result;

            result.Kind = Kind;
            result.IsSync = IsSync;

            result.Statements = Statements.Select(p => p.CloneAstStatement(context)).ToList();
            result.CompiledFunctionBody = CompiledFunctionBody?.Clone(context);

            return result;
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

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(IsSync)} = {IsSync}");

            sb.PrintObjListProp(n, nameof(Statements), Statements);
            sb.PrintObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

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

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(IsSync)} = {IsSync}");

            sb.PrintShortObjListProp(n, nameof(Statements), Statements);
            sb.PrintShortObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

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

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(IsSync)} = {IsSync}");

            sb.PrintBriefObjListProp(n, nameof(Statements), Statements);
            sb.PrintBriefObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            return sb.ToString();
        }
    }
}
