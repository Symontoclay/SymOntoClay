using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Statements
{
    public class AstTryStatement : AstStatement
    {
        public List<AstStatement> TryStatements { get; set; } = new List<AstStatement>();
        public List<AstCatchStatement> CatchStatements { get; set; } = new List<AstCatchStatement>();
        public List<AstStatement> ElseStatements { get; set; } = new List<AstStatement>();
        public List<AstStatement> EnsureStatements { get; set; } = new List<AstStatement>();

        /// <inheritdoc/>
        public override KindOfAstStatement Kind => KindOfAstStatement.TryStatement;

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneAstStatement(context);
        }

        /// <inheritdoc/>
        public override AstStatement CloneAstStatement(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (AstStatement)context[this];
            }

            var result = new AstTryStatement();
            context[this] = result;

            result.TryStatements = TryStatements?.Select(p => p.CloneAstStatement(context)).ToList();
            result.CatchStatements = CatchStatements?.Select(p => p.Clone(context)).ToList();
            result.ElseStatements = ElseStatements?.Select(p => p.CloneAstStatement(context)).ToList();
            result.EnsureStatements = EnsureStatements?.Select(p => p.CloneAstStatement(context)).ToList();

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void CalculateLongHashCodes()
        {
            //RuleInstanceValue.CheckDirty();

            base.CalculateLongHashCodes();
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjListProp(n, nameof(TryStatements), TryStatements);
            sb.PrintObjListProp(n, nameof(CatchStatements), CatchStatements);
            sb.PrintObjListProp(n, nameof(ElseStatements), ElseStatements);
            sb.PrintObjListProp(n, nameof(EnsureStatements), EnsureStatements);
            
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjListProp(n, nameof(TryStatements), TryStatements);
            sb.PrintShortObjListProp(n, nameof(CatchStatements), CatchStatements);
            sb.PrintShortObjListProp(n, nameof(ElseStatements), ElseStatements);
            sb.PrintShortObjListProp(n, nameof(EnsureStatements), EnsureStatements);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjListProp(n, nameof(TryStatements), TryStatements);
            sb.PrintBriefObjListProp(n, nameof(CatchStatements), CatchStatements);
            sb.PrintBriefObjListProp(n, nameof(ElseStatements), ElseStatements);
            sb.PrintBriefObjListProp(n, nameof(EnsureStatements), EnsureStatements);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
