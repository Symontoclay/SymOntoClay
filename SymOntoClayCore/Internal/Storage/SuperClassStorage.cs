/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.CodeModel;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class SuperClassStorage : RealStorage
    {
        public SuperClassStorage(RealStorageSettings settings, StrongIdentifierValue targetClassName, IInstance instance)
            : base(KindOfStorage.SuperClass, settings)
        {
            _targetClassName = targetClassName;
            _instance = instance;
            _instanceName= instance.Name;
        }

        private readonly StrongIdentifierValue _targetClassName;
        private readonly StrongIdentifierValue _instanceName;
        private readonly IInstance _instance;

        /// <inheritdoc/>
        public override StrongIdentifierValue TargetClassName => _targetClassName;

        /// <inheritdoc/>
        public override StrongIdentifierValue InstanceName => _instanceName;

        /// <inheritdoc/>
        public override IInstance Instance => _instance;

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(TargetClassName)} = {TargetClassName?.ToHumanizedLabel()}");
            sb.AppendLine($"{spaces}{nameof(InstanceName)} = {InstanceName?.ToHumanizedLabel()}");
            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(TargetClassName)} = {TargetClassName?.ToHumanizedLabel()}");
            sb.AppendLine($"{spaces}{nameof(InstanceName)} = {InstanceName?.ToHumanizedLabel()}");
            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(TargetClassName)} = {TargetClassName?.ToHumanizedLabel()}");
            sb.AppendLine($"{spaces}{nameof(InstanceName)} = {InstanceName?.ToHumanizedLabel()}");
            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }
    }
}
