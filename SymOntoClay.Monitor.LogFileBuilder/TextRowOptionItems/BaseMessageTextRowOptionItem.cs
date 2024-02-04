using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.TextRowOptionItems
{
    public abstract class BaseMessageTextRowOptionItem : IObjectToString
    {
        public TextTransformations TextTransformation { get; set; } = TextTransformations.None;

        public virtual string GetText(BaseMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            var content = GetContent(message, logFileCreatorContext);

            switch (TextTransformation)
            {
                case TextTransformations.None:
                    return content;

                case TextTransformations.UpperCase:
                    return content.ToUpper();

                case TextTransformations.LowerCase:
                    return content.ToLower();

                default:
                    throw new ArgumentOutOfRangeException(nameof(TextTransformation), TextTransformation, null);
            }
        }

        protected abstract string GetContent(BaseMessage message, ILogFileCreatorContext logFileCreatorContext);

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
            sb.AppendLine($"{spaces}{nameof(TextTransformation)} = {TextTransformation}");
            return sb.ToString();
        }
    }
}
