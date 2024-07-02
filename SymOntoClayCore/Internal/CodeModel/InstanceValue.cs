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
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class InstanceValue : Value
    {
        public InstanceValue(IInstance instanceInfo)
        {
            InstanceInfo = instanceInfo;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.InstanceValue;

        /// <inheritdoc/>
        public override bool IsInstanceValue => true;

        /// <inheritdoc/>
        public override InstanceValue AsInstanceValue => this;

        public IInstance InstanceInfo { get; private set; }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return InstanceInfo;
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException("D55C2903-3E58-405E-BFA1-EB5E36B8C0E8");
        }

        /// <inheritdoc/>
        public override IExecutable GetExecutable(IMonitorLogger logger, KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters)
        {
            return InstanceInfo.GetExecutable(logger, kindOfParameters, namedParameters, positionedParameters);
        }

        /// <inheritdoc/>
        protected override Value GetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName)
        {
            return InstanceInfo.GetPropertyValue(logger, propertyName);
        }

        /// <inheritdoc/>
        protected override Value GetVarValue(IMonitorLogger logger, StrongIdentifierValue varName)
        {
            return InstanceInfo.GetVarValue(logger, varName);
        }

        /// <inheritdoc/>
        protected override void SetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName, Value value)
        {
            InstanceInfo.SetPropertyValue(logger, propertyName, value);
        }

        /// <inheritdoc/>
        protected override void SetVarValue(IMonitorLogger logger, StrongIdentifierValue varName, Value value)
        {
            InstanceInfo.SetVarValue(logger, varName, value);
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            return base.CalculateLongHashCode(options) ^ (ulong)Math.Abs(InstanceInfo.GetHashCode());
        }

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

            var result = new InstanceValue(InstanceInfo);
            context[this] = result;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(InstanceInfo), InstanceInfo);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(InstanceInfo), InstanceInfo);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(InstanceInfo), InstanceInfo);

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
            return InstanceInfo.ToHumanizedString(options);
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return InstanceInfo.ToHumanizedLabel(options);
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return InstanceInfo.ToLabel(logger);
        }
    }
}
