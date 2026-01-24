using MessagePack;
using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Monitor.Common.Data;
using System;
using System.Text;

namespace TestSandbox.MessagePacking
{
    //[MessagePackObject]
    //[Union(0, typeof(TstLogMessage))]
    public abstract class TstBaseLogMessage : IObjectToString
    {
        public abstract KindOfMessage KindOfMessage { get; }

        [Key(0)]
        public DateTime Timestamp { get; set; }

        [Key(1)]
        public string Level { get; set; }

        [Key(2)]
        //[IgnoreMember]
        public string Message { get; set; }

        [Key(3)]
        public string Exception { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(KindOfMessage)} = {KindOfMessage}");
            sb.AppendLine($"{spaces}{nameof(Timestamp)} = {Timestamp}");
            sb.AppendLine($"{spaces}{nameof(Level)} = {Level}");
            sb.AppendLine($"{spaces}{nameof(Message)} = {Message}");
            sb.AppendLine($"{spaces}{nameof(Exception)} = {Exception}");

            return sb.ToString();
        }
    }
}
