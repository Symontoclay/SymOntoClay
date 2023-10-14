using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems
{
    public class BaseFileNameTemplateOptionItem : IObjectToString
    {
        public virtual string GetText(string nodeId, string threadId)
        {
            return (_internalRef ??= InternalRefFactory()).GetText(nodeId, threadId);
        }

        private BaseFileNameTemplateOptionItem _internalRef;

        private BaseFileNameTemplateOptionItem InternalRefFactory()
        {
            switch(ItemName)
            {
                case "DateTimeStamp":
                    return new DateTimeStampFileNameTemplateOptionItem(Format);

                case "LongDateTime":
                    return new LongDateTimeFileNameTemplateOptionItem();

                case "ShortDateTime":
                    return new ShortDateTimeFileNameTemplateOptionItem();

                case "NodeId":
                    return new NodeIdFileNameTemplateOptionItem();

                case "SpaceText":
                    return new SpaceTextFileNameTemplateOptionItem()
                    {
                        Widht = Widht
                    };

                case "Text":
                    return new TextFileNameTemplateOptionItem(Text);

                case "ThreadId":
                    return new ThreadIdFileNameTemplateOptionItem();

                default:
                    throw new ArgumentOutOfRangeException(nameof(ItemName), ItemName, null);
            }
        }

        public virtual string ItemName { get; set; }
        public bool IfNodeIdExists { get; set; }
        public bool IfThreadIdExists { get; set; }
        public string Format { get; set; }
        public uint Widht { get; set; } = 1;
        public string Text { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(ItemName)} = {ItemName}");
            sb.AppendLine($"{spaces}{nameof(IfNodeIdExists)} = {IfNodeIdExists}");
            sb.AppendLine($"{spaces}{nameof(IfThreadIdExists)} = {IfThreadIdExists}");
            sb.AppendLine($"{spaces}{nameof(Text)} = {Text}");
            sb.AppendLine($"{spaces}{nameof(Widht)} = {Widht}");
            sb.AppendLine($"{spaces}{nameof(Format)} = {Format}");
            return sb.ToString();
        }
    }
}
