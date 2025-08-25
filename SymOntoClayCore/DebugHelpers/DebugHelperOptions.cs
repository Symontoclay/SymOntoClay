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
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.DebugHelpers
{
    public class DebugHelperOptions : IObjectToString
    {
        public HumanizedOptions HumanizedOptions { get; set; } = HumanizedOptions.ShowAll;
        public bool IsHtml { get; set; }
        public List<IObjectToString> ItemsForSelection { get; set; }
        public bool EnableMark { get; set; } = true;
        public bool EnableParamsIfEmpty { get; set; } = true;
        public bool ShowPrefixesForConceptLikeIdentifier { get; set; } = true;

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public DebugHelperOptions Clone()
        {
            var result = new DebugHelperOptions();

            result.HumanizedOptions = HumanizedOptions;
            result.IsHtml = IsHtml;
            result.ItemsForSelection = ItemsForSelection?.ToList();
            result.EnableMark = EnableMark;
            result.EnableParamsIfEmpty = EnableParamsIfEmpty;
            result.ShowPrefixesForConceptLikeIdentifier = ShowPrefixesForConceptLikeIdentifier;

            return result;
        }

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
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(HumanizedOptions)} = {HumanizedOptions}");
            sb.AppendLine($"{spaces}{nameof(IsHtml)} = {IsHtml}");
            sb.PrintObjListProp(n, nameof(ItemsForSelection), ItemsForSelection);
            sb.AppendLine($"{spaces}{nameof(EnableMark)} = {EnableMark}");
            sb.AppendLine($"{spaces}{nameof(EnableParamsIfEmpty)} = {EnableParamsIfEmpty}");
            sb.AppendLine($"{spaces}{nameof(ShowPrefixesForConceptLikeIdentifier)} = {ShowPrefixesForConceptLikeIdentifier}");

            return sb.ToString();
        }

        public static DebugHelperOptions FromHumanizedOptions(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return opt;
        }
    }
}
