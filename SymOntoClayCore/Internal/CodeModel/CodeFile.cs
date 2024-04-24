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

using SymOntoClay.Common;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class CodeFile: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public string FileName { get; set; }
        public bool IsMain { get; set; }
        public bool IsLocator { get; set; }
        public List<CodeItem> CodeEntities { get; set; } = new List<CodeItem>();

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
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(FileName)} = {FileName}");
            sb.AppendLine($"{spaces}{nameof(IsMain)} = {IsMain}");
            sb.AppendLine($"{spaces}{nameof(IsLocator)} = {IsLocator}");
            sb.PrintObjListProp(n, nameof(CodeEntities), CodeEntities);
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
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(FileName)} = {FileName}");
            sb.AppendLine($"{spaces}{nameof(IsMain)} = {IsMain}");
            sb.AppendLine($"{spaces}{nameof(IsLocator)} = {IsLocator}");
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
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(FileName)} = {FileName}");
            sb.AppendLine($"{spaces}{nameof(IsMain)} = {IsMain}");
            sb.AppendLine($"{spaces}{nameof(IsLocator)} = {IsLocator}");
            return sb.ToString();
        }
    }
}
