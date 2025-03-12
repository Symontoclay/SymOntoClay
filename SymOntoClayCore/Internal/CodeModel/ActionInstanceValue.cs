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

using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class ActionInstanceValue: Value, IExecutable
    {
        public ActionInstanceValue(ActionInstance actionInstance)
        {
             ActionInstance = actionInstance;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.ActionInstanceValue;

        /// <inheritdoc/>
        public override bool IsActionInstanceValue => true;

        /// <inheritdoc/>
        public override ActionInstanceValue AsActionInstanceValue => this;

        public ActionInstance ActionInstance { get; private set; }

        /// <inheritdoc/>
        public IExecutionCoordinator GetCoordinator(IMonitorLogger logger, IEngineContext context, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            throw new NotImplementedException("750532E9-432D-44E6-8C37-CCC35C40814E");
        }

        /// <inheritdoc/>
        public IExecutable Activate(IMonitorLogger logger, IEngineContext context, ILocalCodeExecutionContext localCodeExecutionContext, IExecutionCoordinator executionCoordinator)
        {
            throw new NotImplementedException("7C4E549E-3D08-425D-9C90-4286B6D40E68");
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return ActionInstance;
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException("9A6C00DE-2C0F-495B-8F35-0F10D30D5E70");
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            return base.CalculateLongHashCode(options) ^ ActionInstance.GetLongHashCode();
        }

        /// <inheritdoc/>
        public bool IsSystemDefined => false;

        /// <inheritdoc/>
        public IList<IFunctionArgument> Arguments => ActionInstance.Arguments;

        /// <inheritdoc/>
        public CompiledFunctionBody CompiledFunctionBody => ActionInstance.CompiledFunctionBody;

        /// <inheritdoc/>
        public CodeItem CodeItem => ActionInstance.CodeItem;

        /// <inheritdoc/>
        public ISystemHandler SystemHandler => ActionInstance.SystemHandler;

        /// <inheritdoc/>
        public bool ContainsArgument(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return ActionInstance.ContainsArgument(logger, name);
        }

        /// <inheritdoc/>
        public ILocalCodeExecutionContext OwnLocalCodeExecutionContext => ActionInstance.OwnLocalCodeExecutionContext;

        /// <inheritdoc/>
        public TypeInfo Holder => null;

        /// <inheritdoc/>
        public bool NeedActivation => true;

        /// <inheritdoc/>
        public bool IsActivated => true;

        /// <inheritdoc/>
        public bool IsInstance => throw new NotImplementedException("2D90DFF4-9B3B-40AC-8CC8-19AC9EB89286");

        /// <inheritdoc/>
        public IInstance AsInstance => throw new NotImplementedException("611F3569-3528-4247-B086-5B9893CAB084");

        /// <inheritdoc/>
        public UsingLocalCodeExecutionContextPreferences UsingLocalCodeExecutionContextPreferences => ActionInstance?.UsingLocalCodeExecutionContextPreferences ?? UsingLocalCodeExecutionContextPreferences.UseBothOwnAndCallerAsParent;

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneValue(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Value)context[this];
            }

            var result = new ActionInstanceValue(ActionInstance);
            context[this] = result;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(ActionInstance), ActionInstance);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(ActionInstance), ActionInstance);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(ActionInstance), ActionInstance);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}{ToHumanizedString()}";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        private string NToHumanizedString()
        {
            return $"ref: {ActionInstance.Name.NameValue}";
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = NToHumanizedString()
            };
        }
    }
}
