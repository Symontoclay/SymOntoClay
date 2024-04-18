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

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public abstract class BaseSetStatusMessage : BaseMessage
    {
        public string ObjId { get; set; }
        public int Status { get; set; }
        public string StatusStr { get; set; }
        public int PrevStatus { get; set; }
        public string PrevStatusStr { get; set; }
        public List<Changer> Changers { get; set; }
        public string CallMethodId { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(ObjId)} = {ObjId}");
            sb.AppendLine($"{spaces}{nameof(Status)} = {Status}");
            sb.AppendLine($"{spaces}{nameof(StatusStr)} = {StatusStr}");
            sb.AppendLine($"{spaces}{nameof(PrevStatus)} = {PrevStatus}");
            sb.AppendLine($"{spaces}{nameof(PrevStatusStr)} = {PrevStatusStr}");
            sb.PrintObjListProp(n, nameof(Changers), Changers);
            sb.AppendLine($"{spaces}{nameof(CallMethodId)} = {CallMethodId}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
