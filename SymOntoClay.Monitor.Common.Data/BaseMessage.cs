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
using System;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public abstract class BaseMessage : IObjectToString
    {
        public abstract KindOfMessage KindOfMessage { get; }
        public DateTime DateTimeStamp { get; set; }
        public string NodeId { get; set; }
        public string ThreadId { get; set; }
        public ulong GlobalMessageNumber { get; set; }
        public ulong MessageNumber { get; set; }
        public string MessagePointId { get; set; }
        public string ClassFullName { get; set; }
        public string MemberName { get; set; }
        public string SourceFilePath { get; set; }
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
