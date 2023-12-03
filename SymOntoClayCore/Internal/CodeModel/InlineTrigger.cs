/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.Converters;
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
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;

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
        public List<AstStatement> SetStatements { get; set; } = new List<AstStatement>();
        public CompiledFunctionBody SetCompiledFunctionBody { get; set; }

        public List<AstStatement> ResetStatements { get; set; } = new List<AstStatement>();
        public CompiledFunctionBody ResetCompiledFunctionBody { get; set; }

        public List<RuleInstance> RuleInstancesList { get; set; } = new List<RuleInstance>();

        public DoubleConditionsStrategy DoubleConditionsStrategy { get; set; } = DoubleConditionsStrategy.PriorSet;

        public void AddAliasRange(List<StrongIdentifierValue> aliasList)
        {
            _aliasesList.AddRange(aliasList);
        }

        public IList<StrongIdentifierValue> NamesList => _namesList;

        private List<StrongIdentifierValue> _namesList;
        private List<StrongIdentifierValue> _aliasesList = new List<StrongIdentifierValue>();

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            var result =  base.CalculateLongHashCode(options) ^ (ulong)Math.Abs(KindOfInlineTrigger.GetHashCode()) ^ (ulong)Math.Abs(KindOfSystemEvent.GetHashCode());

            foreach (var alias in _aliasesList)
            {
                alias.CheckDirty(options);
            }

            _namesList = new List<StrongIdentifierValue>() { Name };
            _namesList.AddRange(_aliasesList);

            if (SetCondition != null)
            {
                SetCondition.CheckDirty(options);

                result ^= SetCondition.GetLongHashCode(options);
            }

            if (ResetCondition != null)
            {
                ResetCondition.CheckDirty(options);

                result ^= ResetCondition.GetLongHashCode(options);
            }

            if(!RuleInstancesList.IsNullOrEmpty())
            {
                foreach(var item in RuleInstancesList)
                {
                    item.CheckDirty(options);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public InlineTrigger Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
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

            result.SetStatements = SetStatements.Select(p => p.CloneAstStatement(context)).ToList();
            result.SetCompiledFunctionBody = SetCompiledFunctionBody?.Clone(context);

            result.ResetStatements = ResetStatements.Select(p => p.CloneAstStatement(context)).ToList();            
            result.ResetCompiledFunctionBody = ResetCompiledFunctionBody?.Clone(context);

            result.RuleInstancesList = RuleInstancesList?.Select(p => p.Clone(context)).ToList();

            result.DoubleConditionsStrategy = DoubleConditionsStrategy;

            result._aliasesList = _aliasesList?.Select(p => p.Clone(context)).ToList();
            result._namesList = _namesList?.Select(p => p.Clone(context)).ToList();

            result.AppendCodeItem(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            if (!SetStatements.IsNullOrEmpty())
            {
                foreach (var item in SetStatements)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }

            if (!ResetStatements.IsNullOrEmpty())
            {
                foreach (var item in ResetStatements)
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

            sb.PrintObjListProp(n, nameof(NamesList), NamesList);

            sb.PrintBriefObjProp(n, nameof(SetCondition), SetCondition);
            sb.PrintObjProp(n, nameof(SetBindingVariables), SetBindingVariables);
            sb.PrintBriefObjProp(n, nameof(ResetCondition), ResetCondition);
            sb.PrintObjProp(n, nameof(ResetBindingVariables), ResetBindingVariables);

            sb.PrintObjListProp(n, nameof(SetStatements), SetStatements);
            sb.PrintObjProp(n, nameof(SetCompiledFunctionBody), SetCompiledFunctionBody);

            sb.PrintObjListProp(n, nameof(ResetStatements), ResetStatements);
            sb.PrintObjProp(n, nameof(ResetCompiledFunctionBody), ResetCompiledFunctionBody);

            sb.PrintObjListProp(n, nameof(RuleInstancesList), RuleInstancesList);            

            sb.AppendLine($"{spaces}{nameof(DoubleConditionsStrategy)} = {DoubleConditionsStrategy}");            

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

            sb.PrintShortObjListProp(n, nameof(NamesList), NamesList);

            sb.PrintBriefObjProp(n, nameof(SetCondition), SetCondition);
            sb.PrintShortObjProp(n, nameof(SetBindingVariables), SetBindingVariables);
            sb.PrintBriefObjProp(n, nameof(ResetCondition), ResetCondition);
            sb.PrintShortObjProp(n, nameof(ResetBindingVariables), ResetBindingVariables);

            sb.PrintShortObjListProp(n, nameof(SetStatements), SetStatements);
            sb.PrintShortObjProp(n, nameof(SetCompiledFunctionBody), SetCompiledFunctionBody);

            sb.PrintShortObjListProp(n, nameof(ResetStatements), ResetStatements);
            sb.PrintShortObjProp(n, nameof(ResetCompiledFunctionBody), ResetCompiledFunctionBody);

            sb.PrintShortObjListProp(n, nameof(RuleInstancesList), RuleInstancesList);

            sb.AppendLine($"{spaces}{nameof(DoubleConditionsStrategy)} = {DoubleConditionsStrategy}");

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

            sb.PrintExistingList(n, nameof(NamesList), NamesList);

            sb.PrintBriefObjProp(n, nameof(SetCondition), SetCondition);
            sb.PrintBriefObjProp(n, nameof(SetBindingVariables), SetBindingVariables);
            sb.PrintBriefObjProp(n, nameof(ResetCondition), ResetCondition);
            sb.PrintBriefObjProp(n, nameof(ResetBindingVariables), ResetBindingVariables);

            sb.PrintBriefObjListProp(n, nameof(SetStatements), SetStatements);
            sb.PrintBriefObjProp(n, nameof(SetCompiledFunctionBody), SetCompiledFunctionBody);

            sb.PrintBriefObjListProp(n, nameof(ResetStatements), ResetStatements);
            sb.PrintBriefObjProp(n, nameof(ResetCompiledFunctionBody), ResetCompiledFunctionBody);

            sb.PrintExistingList(n, nameof(RuleInstancesList), RuleInstancesList);

            sb.AppendLine($"{spaces}{nameof(DoubleConditionsStrategy)} = {DoubleConditionsStrategy}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            throw new NotImplementedException();
        }
    }
}
