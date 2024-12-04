/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class IdleActionItem : CodeItem
    {
        public IdleActionItem()
        {
            TypeOfAccess = TypeOfAccess.Public;
        }

        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.IdleActionItem;

        /// <inheritdoc/>
        public override bool IsIdleActionItem => true;
        
        /// <inheritdoc/>
        public override IdleActionItem AsIdleActionItem => this;

        public RuleInstance RuleInstance { get; set; }
        public List<AstStatement> Statements { get; set; } = new List<AstStatement>();
        public CompiledFunctionBody CompiledFunctionBody { get; set; }        

        public CompiledFunctionBody GetCompiledFunctionBody(IMonitorLogger logger, IEngineContext context, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            if(CompiledFunctionBody != null)
            {
                return CompiledFunctionBody;
            }

            if(RuleInstance != null)
            {
                CompiledFunctionBody = context.ConvertersFactory.GetConverterFactToImperativeCode().Convert(logger, RuleInstance, localCodeExecutionContext);
                return CompiledFunctionBody;
            }

            throw new NotImplementedException("37BC27AD-2749-46DE-B085-2470298315B5");
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            var result = base.CalculateLongHashCode(options) /*^ SubName.GetLongHashCode(options) ^ SuperName.GetLongHashCode(options)*/;


            return result;
        }

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public IdleActionItem Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public IdleActionItem Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (IdleActionItem)context[this];
            }

            var result = new IdleActionItem();
            context[this] = result;
            result.RuleInstance = RuleInstance?.Clone(context);
            result.Statements = Statements?.Select(p => p.CloneAstStatement(context)).ToList();
            result.CompiledFunctionBody = CompiledFunctionBody?.Clone(context);
            
            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            if (!Statements.IsNullOrEmpty())
            {
                foreach (var item in Statements)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }

            if(RuleInstance != null)
            {
                RuleInstance.DiscoverAllAnnotations(result);
            }
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(RuleInstance), RuleInstance);
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

            sb.PrintShortObjProp(n, nameof(RuleInstance), RuleInstance);
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

            sb.PrintBriefObjProp(n, nameof(RuleInstance), RuleInstance);
            sb.PrintBriefObjListProp(n, nameof(Statements), Statements);
            sb.PrintBriefObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            if(RuleInstance != null)
            {
                return RuleInstance.ToHumanizedString(options);
            }

            return Statements.ToHumanizedString(options);
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return ToHumanizedString(options);
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = ToHumanizedString()
            };
        }
    }
}
