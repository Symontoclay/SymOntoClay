using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.Compiling.Internal;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class PrimitiveTaskOperator : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public AstStatement Statement { get; set; }
        public List<IntermediateScriptCommand> IntermediateCommandsList { get; set; }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public PrimitiveTaskOperator Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public PrimitiveTaskOperator Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (PrimitiveTaskOperator)context[this];
            }

            var result = new PrimitiveTaskOperator();
            context[this] = result;

            result.Statement = Statement.CloneAstStatement(context);
            result.IntermediateCommandsList = IntermediateCommandsList?.ToList();

            return result;
        }

        public void DiscoverAllAnnotations(IList<Annotation> result)
        {
            Statement?.DiscoverAllAnnotations(result);
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
            sb.PrintObjProp(n, nameof(Statement), Statement);
            sb.PrintObjListProp(n, nameof(IntermediateCommandsList), IntermediateCommandsList);

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

            sb.PrintShortObjProp(n, nameof(Statement), Statement);
            sb.PrintShortObjListProp(n, nameof(IntermediateCommandsList), IntermediateCommandsList);

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

            sb.PrintBriefObjProp(n, nameof(Statement), Statement);
            sb.PrintBriefObjListProp(n, nameof(IntermediateCommandsList), IntermediateCommandsList);

            return sb.ToString();
        }

        public string ToHumanizedString(DebugHelperOptions options)
        {
            var sb = new StringBuilder();

            if (Statement != null)
            {
                sb.Append(Statement.ToHumanizedString(options));
            }

            sb.Append(";");

            return sb.ToString();
        }

        public string ToHumanizedLabel(DebugHelperOptions options)
        {
            return ToHumanizedString(options);
        }
    }
}
