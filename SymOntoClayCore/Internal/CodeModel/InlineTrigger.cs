/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class InlineTrigger : CodeItem
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.InlineTrigger;

        /// <inheritdoc/>
        public override bool IsInlineTrigger => true;

        /// <inheritdoc/>
        public override InlineTrigger AsInlineTrigger => this;

        public KindOfInlineTrigger KindOfInlineTrigger { get; set; } = KindOfInlineTrigger.Unknown;
        public KindOfSystemEventOfInlineTrigger KindOfSystemEvent { get; set; } = KindOfSystemEventOfInlineTrigger.Unknown;
        public TriggerConditionNode SetCondition { get; set; }
        public BindingVariables SetBindingVariables { get; set; } = new BindingVariables();
        public TriggerConditionNode ResetCondition { get; set; }
        public BindingVariables ResetBindingVariables { get; set; } = new BindingVariables();
        public List<AstStatement> Statements { get; set; } = new List<AstStatement>();

        public CompiledFunctionBody CompiledFunctionBody { get; set; }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            var result =  base.CalculateLongHashCode(options) ^ (ulong)Math.Abs(KindOfInlineTrigger.GetHashCode()) ^ (ulong)Math.Abs(KindOfSystemEvent.GetHashCode());

            if(SetCondition != null)
            {
                SetCondition.CheckDirty(options);

                result ^= SetCondition.GetLongHashCode(options);
            }

            if (ResetCondition != null)
            {
                ResetCondition.CheckDirty(options);

                result ^= ResetCondition.GetLongHashCode(options);
            }

            return result;
        }

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> cloneContext)
        {
            return Clone(cloneContext);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public InlineTrigger Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public InlineTrigger Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (InlineTrigger)context[this];
            }

            var result = new InlineTrigger();
            context[this] = result;

            result.KindOfInlineTrigger = KindOfInlineTrigger;
            result.KindOfSystemEvent = KindOfSystemEvent;

            result.SetCondition = SetCondition?.Clone(context);
            result.SetBindingVariables = SetBindingVariables.Clone(context);

            result.ResetCondition = ResetCondition?.Clone(context);
            result.ResetBindingVariables = ResetBindingVariables.Clone(context);

            result.Statements = Statements.Select(p => p.CloneAstStatement(context)).ToList();

            result.CompiledFunctionBody = CompiledFunctionBody.Clone(context);
            result.AppendCodeItem(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            if (!Statements.IsNullOrEmpty())
            {
                foreach (var item in Statements)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(KindOfInlineTrigger)} = {KindOfInlineTrigger}");
            sb.AppendLine($"{spaces}{nameof(KindOfSystemEvent)} = {KindOfSystemEvent}");

            sb.PrintBriefObjProp(n, nameof(SetCondition), SetCondition);
            sb.PrintObjProp(n, nameof(SetBindingVariables), SetBindingVariables);
            sb.PrintBriefObjProp(n, nameof(ResetCondition), ResetCondition);
            sb.PrintObjProp(n, nameof(ResetBindingVariables), ResetBindingVariables);

            sb.PrintObjListProp(n, nameof(Statements), Statements);
            sb.PrintObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(KindOfInlineTrigger)} = {KindOfInlineTrigger}");
            sb.AppendLine($"{spaces}{nameof(KindOfSystemEvent)} = {KindOfSystemEvent}");

            sb.PrintBriefObjProp(n, nameof(SetCondition), SetCondition);
            sb.PrintShortObjProp(n, nameof(SetBindingVariables), SetBindingVariables);
            sb.PrintBriefObjProp(n, nameof(ResetCondition), ResetCondition);
            sb.PrintShortObjProp(n, nameof(ResetBindingVariables), ResetBindingVariables);

            sb.PrintShortObjListProp(n, nameof(Statements), Statements);
            sb.PrintShortObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(KindOfInlineTrigger)} = {KindOfInlineTrigger}");
            sb.AppendLine($"{spaces}{nameof(KindOfSystemEvent)} = {KindOfSystemEvent}");

            sb.PrintBriefObjProp(n, nameof(SetCondition), SetCondition);
            sb.PrintBriefObjProp(n, nameof(SetBindingVariables), SetBindingVariables);
            sb.PrintBriefObjProp(n, nameof(ResetCondition), ResetCondition);
            sb.PrintBriefObjProp(n, nameof(ResetBindingVariables), ResetBindingVariables);

            sb.PrintBriefObjListProp(n, nameof(Statements), Statements);
            sb.PrintBriefObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
