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

using MessagePack;
using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    [MessagePackObject]
    [Union(0, typeof(CreateMonitorNodeMessage))]
    [Union(1, typeof(CreateThreadLoggerMessage))]
    [Union(2, typeof(AddEndpointMessage))]
    [Union(3, typeof(CallMethodMessage))]
    [Union(4, typeof(BaseValueMessage))]
    [Union(5, typeof(BaseMethodMessage))]
    //[Union(, typeof())]
    public abstract class BaseMessage : IObjectToString
    {
        public abstract KindOfMessage KindOfMessage { get; }

        [Key(0)]
        public DateTime DateTimeStamp { get; set; }

        [Key(1)]
        public string NodeId { get; set; }

        [Key(2)]
        public string ThreadId { get; set; }

        [Key(3)]
        public ulong GlobalMessageNumber { get; set; }

        [Key(4)]
        public ulong MessageNumber { get; set; }

        [Key(5)]
        public string MessagePointId { get; set; }

        [Key(6)]
        public string ClassFullName { get; set; }

        [Key(7)]
        public string MemberName { get; set; }

        [Key(8)]
        public string SourceFilePath { get; set; }

        [Key(9)]
        public int SourceLineNumber { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(DateTimeStamp)} = {DateTimeStamp}");
            sb.AppendLine($"{spaces}{nameof(NodeId)} = {NodeId}");
            sb.AppendLine($"{spaces}{nameof(ThreadId)} = {ThreadId}");
            sb.AppendLine($"{spaces}{nameof(GlobalMessageNumber)} = {GlobalMessageNumber}");
            sb.AppendLine($"{spaces}{nameof(ClassFullName)} = {ClassFullName}");
            sb.AppendLine($"{spaces}{nameof(MessageNumber)} = {MessageNumber}");
            sb.AppendLine($"{spaces}{nameof(MessagePointId)} = {MessagePointId}");
            sb.AppendLine($"{spaces}{nameof(MemberName)} = {MemberName}");
            sb.AppendLine($"{spaces}{nameof(SourceFilePath)} = {SourceFilePath}");
            sb.AppendLine($"{spaces}{nameof(SourceLineNumber)} = {SourceLineNumber}");

            return sb.ToString();
        }
    }
}
