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
            return _messageContentToTextConverter.GetText(message, logFileCreatorContext, targetFileName);
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
