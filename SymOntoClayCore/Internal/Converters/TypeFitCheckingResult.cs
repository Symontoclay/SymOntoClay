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

using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System.Text;
using System;

namespace SymOntoClay.Core.Internal.Converters
{
    public class TypeFitCheckingResult : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public static TypeFitCheckingResult Fit = new TypeFitCheckingResult(KindOfTypeFitCheckingResult.IsFit);
        public static TypeFitCheckingResult IsNotFit = new TypeFitCheckingResult(KindOfTypeFitCheckingResult.IsNotFit);

        public TypeFitCheckingResult(KindOfTypeFitCheckingResult kindOfResult)
        {
            KindOfResult = kindOfResult;
        }

        public TypeFitCheckingResult(KindOfTypeFitCheckingResult kindOfResult, StrongIdentifierValue suggestedType)
        {
            KindOfResult = kindOfResult;
            SuggestedType = suggestedType;
        }

        public KindOfTypeFitCheckingResult KindOfResult { get; set; } = KindOfTypeFitCheckingResult.Unknown;
        public StrongIdentifierValue SuggestedType { get; set; }

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
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfResult)} = {KindOfResult}");
            sb.PrintObjProp(n, nameof(SuggestedType), SuggestedType);
            //sb.PrintObjProp(n, nameof(Executable), Executable);
            //sb.AppendLine($"{spaces}{nameof(SystemException)} = {SystemException}");
            //sb.PrintObjProp(n, nameof(DslException), DslException);
            //sb.AppendLine($"{spaces}{nameof(IsError)} = {IsError}");

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
            return PropertiesToShortString(n);
        }

        protected virtual string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfResult)} = {KindOfResult}");
            sb.PrintShortObjProp(n, nameof(SuggestedType), SuggestedType);
            //sb.PrintShortObjProp(n, nameof(Executable), Executable);
            //sb.AppendLine($"{spaces}{nameof(SystemException)} = {SystemException}");
            //sb.PrintShortObjProp(n, nameof(DslException), Executable);
            //sb.AppendLine($"{spaces}{nameof(IsError)} = {IsError}");

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
            return PropertiesToBriefString(n);
        }

        protected virtual string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfResult)} = {KindOfResult}");
            sb.PrintBriefObjProp(n, nameof(SuggestedType), SuggestedType);
            //sb.PrintBriefObjProp(n, nameof(Executable), Executable);
            //sb.AppendLine($"{spaces}{nameof(SystemException)} = {SystemException}");
            //sb.PrintBriefObjProp(n, nameof(DslException), DslException);
            //sb.AppendLine($"{spaces}{nameof(IsError)} = {IsError}");

            return sb.ToString();
        }
    }
}
