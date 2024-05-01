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
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.TextRowOptionItems
{
    public class MessageContentTextRowOptionItem : BaseMessageTextRowOptionItem, IMessageContentToTextConverterOptions
    {
        /// <inheritdoc/>
        public bool EnableCallMethodIdOfMethodLabel { get; set; }

        /// <inheritdoc/>
        public bool EnableMethodSignatureArguments { get; set; }

        /// <inheritdoc/>
        public bool EnableTypesListOfMethodSignatureArguments { get; set; }

        /// <inheritdoc/>
        public bool EnableDefaultValueOfMethodSignatureArguments { get; set; }

        /// <inheritdoc/>
        public bool EnablePassedVauesOfMethodLabel { get; set; }

        /// <inheritdoc/>
        public bool EnableChainOfProcessInfo { get; set; }

        public MessageContentTextRowOptionItem() 
        {
            _messageContentToTextConverter = new MessageContentToTextConverter(this);
        }

        private readonly MessageContentToTextConverter _messageContentToTextConverter;

        /// <inheritdoc/>
        protected override string GetContent(BaseMessage message, ILogFileCreatorContext logFileCreatorContext, string targetFileName)
        {
            return logFileCreatorContext.NormalizeText(_messageContentToTextConverter.GetText(message, logFileCreatorContext, targetFileName));
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(EnableCallMethodIdOfMethodLabel)} = {EnableCallMethodIdOfMethodLabel}");
            sb.AppendLine($"{spaces}{nameof(EnableMethodSignatureArguments)} = {EnableMethodSignatureArguments}");
            sb.AppendLine($"{spaces}{nameof(EnableTypesListOfMethodSignatureArguments)} = {EnableTypesListOfMethodSignatureArguments}");
            sb.AppendLine($"{spaces}{nameof(EnableDefaultValueOfMethodSignatureArguments)} = {EnableDefaultValueOfMethodSignatureArguments}");
            sb.AppendLine($"{spaces}{nameof(EnablePassedVauesOfMethodLabel)} = {EnablePassedVauesOfMethodLabel}");
            sb.AppendLine($"{spaces}{nameof(EnableChainOfProcessInfo)} = {EnableChainOfProcessInfo}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
