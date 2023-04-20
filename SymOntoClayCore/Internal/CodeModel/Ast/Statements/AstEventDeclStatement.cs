using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Statements
{
    public class AstEventDeclStatement : AstStatement
    {
        /// <inheritdoc/>
        public override KindOfAstStatement Kind => KindOfAstStatement.EventDeclStatement;

        public AstExpression Expression { get; set; }

        public KindOfAnnotationSystemEvent KindOfAnnotationSystemEvent { get; set; }

        public NamedFunction Handler { get; set; }

        //public List<AstStatement> Statements { get; set; } = new List<AstStatement>();
        //public CompiledFunctionBody CompiledFunctionBody { get; set; }

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

            var result = new AstEventDeclStatement();
            context[this] = result;

            result.Expression = Expression?.CloneAstExpression(context);
            result.KindOfAnnotationSystemEvent = KindOfAnnotationSystemEvent;
            result.Handler = Handler.Clone(context);

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            Expression?.DiscoverAllAnnotations(result);
            Handler?.DiscoverAllAnnotations(result);
        }

        /// <inheritdoc/>
        public override void CalculateLongHashCodes(CheckDirtyOptions options)
        {
            base.CalculateLongHashCodes(options);

            Expression?.CheckDirty(options);
            Handler?.CheckDirty(options);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Expression), Expression);
            sb.AppendLine($"{spaces}{nameof(KindOfAnnotationSystemEvent)} = {KindOfAnnotationSystemEvent}");

            sb.PrintObjProp(n, nameof(Handler), Handler);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Expression), Expression);
            sb.AppendLine($"{spaces}{nameof(KindOfAnnotationSystemEvent)} = {KindOfAnnotationSystemEvent}");

            sb.PrintShortObjProp(n, nameof(Handler), Handler);

            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Expression), Expression);
            sb.AppendLine($"{spaces}{nameof(KindOfAnnotationSystemEvent)} = {KindOfAnnotationSystemEvent}");

            sb.PrintBriefObjProp(n, nameof(Handler), Handler);

            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            var sb = new StringBuilder("on");

            if(Expression != null)
            {
                sb.Append($" {Expression.ToHumanizedString(options)}");
            }

            switch(KindOfAnnotationSystemEvent)
            {
                case KindOfAnnotationSystemEvent.Unknown:
                    sb.Append(" unknown");
                    break;

                case KindOfAnnotationSystemEvent.Complete:
                    sb.Append(" complete");
                    break;

                case KindOfAnnotationSystemEvent.Cancel:
                    sb.Append(" cancel");
                    break;

                case KindOfAnnotationSystemEvent.WeakCancel:
                    sb.Append(" weak cancel");
                    break;

                case KindOfAnnotationSystemEvent.Error:
                    sb.Append(" error");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(KindOfAnnotationSystemEvent), KindOfAnnotationSystemEvent, null);
            }

            if(Handler != null)
            {
                var handlerHumanizedOptions = options.Clone();
                handlerHumanizedOptions.EnableMark = false;
                handlerHumanizedOptions.EnableParamsIfEmpty = false;

                sb.Append($" {Handler.ToHumanizedString(handlerHumanizedOptions)}");
            }

            return sb.ToString();
        }
    }
}
