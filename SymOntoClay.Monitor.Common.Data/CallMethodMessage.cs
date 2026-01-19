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
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    [MessagePackObject]
    public class CallMethodMessage : BaseMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.CallMethod;

        [Key(10)]
        public string CallMethodId { get; set; }

        [Key(11)]
        public MonitoredHumanizedLabel MethodLabel { get; set; }

        [Key(12)]
        public string AltMethodName { get; set; }

        [Key(13)]
        public List<MonitoredHumanizedLabel> ChainOfProcessInfo { get; set; }

        [Key(14)]
        public bool IsSync { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(CallMethodId)} = {CallMethodId}");
            sb.PrintObjProp(n, nameof(MethodLabel), MethodLabel);
            sb.AppendLine($"{spaces}{nameof(AltMethodName)} = {AltMethodName}");
            sb.PrintObjListProp(n, nameof(ChainOfProcessInfo), ChainOfProcessInfo);
            sb.AppendLine($"{spaces}{nameof(IsSync)} = {IsSync}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
