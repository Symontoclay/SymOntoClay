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
using System.Text;
using System;
using SymOntoClay.Core.Internal.CodeModel;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class StrongIdentifierPart : IObjectToString
    {
        public Token Token { get; set; }
        public StrongIdentifierLevel StrongIdentifierLevel { get; set; } = StrongIdentifierLevel.None;
        public List<StrongIdentifierPart> SubParts { get; set; } = new List<StrongIdentifierPart>();
        public int? Capacity { get; set; }
        public bool HasInfiniteCapacity { get; set; }

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
            sb.PrintObjProp(n, nameof(Token), Token);
            sb.AppendLine($"{spaces}{nameof(StrongIdentifierLevel)} = {StrongIdentifierLevel}");
            sb.PrintObjListProp(n, nameof(SubParts), SubParts);
            sb.AppendLine($"{spaces}{nameof(Capacity)} = {Capacity}");
            sb.AppendLine($"{spaces}{nameof(HasInfiniteCapacity)} = {HasInfiniteCapacity}");
            //sb.AppendLine($"{spaces}{nameof(Pos)} = {Pos}");
            //sb.AppendLine($"{spaces}{nameof(Line)} = {Line}");
            return sb.ToString();
        }
    }
}
