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

using NLog;
using NLog.Fluent;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class PrimaryRulePart: BaseRulePart
    {
#if DEBUG
        private static readonly Logger _staticLogger = LogManager.GetCurrentClassLogger();
#endif

        public List<SecondaryRulePart> SecondaryParts { get; set; } = new List<SecondaryRulePart>();

        /// <inheritdoc/>
        public override IList<BaseRulePart> GetNextPartsList(IMonitorLogger logger)
        {
            return SecondaryParts.Select(p => (BaseRulePart)p).ToList();
        }

        public void Accept(ILogicalVisitor logicalVisitor)
        {
            logicalVisitor.VisitPrimaryRulePart(this);
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public PrimaryRulePart Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public PrimaryRulePart Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (PrimaryRulePart)context[this];
            }

            var result = new PrimaryRulePart();
            context[this] = result;

            result.SecondaryParts = SecondaryParts.Select(p => p.Clone(context)).ToList();

            result.AppendBaseRulePart(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
#if DEBUG
            _staticLogger.Info($"||||||||||||||");
            _staticLogger.Info($"this = {this.ToHumanizedString()}");
#endif

            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintExisting(n, nameof(SecondaryParts), SecondaryParts);

#if DEBUG
            _staticLogger.Info(">-->-->71B5D4C3-D4DF-4A0C-807D-A81938E157E2");
#endif

            sb.Append(base.PropertiesToString(n));

#if DEBUG
            _staticLogger.Info(">-->-->5B6C2108-4049-475A-B09F-D9B746B76C6A");
#endif

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintExisting(n, nameof(SecondaryParts), SecondaryParts);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
#if DEBUG
            _staticLogger.Info($"~~~~~~~~~~~~~~~~~~~~~~~~~");
            _staticLogger.Info($"this = {this.ToHumanizedString()}");
#endif

            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintExisting(n, nameof(SecondaryParts), SecondaryParts);

#if DEBUG
            _staticLogger.Info(">-->-->626497B2-4000-4363-925D-BB55E7F7BBF8");
#endif

            sb.Append(base.PropertiesToBriefString(n));

#if DEBUG
            _staticLogger.Info(">-->-->CDD740EB-E836-4EA0-B833-5816546BE33B");
#endif

            return sb.ToString();
        }
    }
}
