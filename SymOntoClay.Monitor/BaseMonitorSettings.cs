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
using SymOntoClay.Monitor.Common;
using System.Text;

namespace SymOntoClay.Monitor
{
    public class BaseMonitorSettings : IObjectToString
    {
        /// <summary>
        /// Gets or sets value of enable logging.
        /// It allows enable or disable logging or remote connection for whole components synchronously.
        /// </summary>
        public bool Enable { get; set; }
        public bool EnableRemoteConnection { get; set; }

        public KindOfLogicalSearchExplain KindOfLogicalSearchExplain { get; set; } = KindOfLogicalSearchExplain.None;
        public bool EnableAddingRemovingFactLoggingInStorages { get; set; }

        public bool EnableFullCallInfo { get; set; }

        public bool EnableAsyncMessageCreation { get; set; }

        public MonitorFeatures Features { get; set; }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public BaseMonitorSettings Clone()
        {
            var result = new BaseMonitorSettings();

            result.Enable = Enable;
            result.EnableRemoteConnection = EnableRemoteConnection;
            result.KindOfLogicalSearchExplain = KindOfLogicalSearchExplain;
            result.EnableAddingRemovingFactLoggingInStorages = EnableAddingRemovingFactLoggingInStorages;
            result.EnableFullCallInfo = EnableFullCallInfo;
            result.EnableAsyncMessageCreation = EnableAsyncMessageCreation;
            result.Features = Features?.Clone();

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
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Enable)} = {Enable}");
            sb.AppendLine($"{spaces}{nameof(EnableRemoteConnection)} = {EnableRemoteConnection}");
            sb.AppendLine($"{spaces}{nameof(KindOfLogicalSearchExplain)} = {KindOfLogicalSearchExplain}");
            sb.AppendLine($"{spaces}{nameof(EnableAddingRemovingFactLoggingInStorages)} = {EnableAddingRemovingFactLoggingInStorages}");
            sb.AppendLine($"{spaces}{nameof(EnableFullCallInfo)} = {EnableFullCallInfo}");
            sb.AppendLine($"{spaces}{nameof(EnableAsyncMessageCreation)} = {EnableAsyncMessageCreation}");
            sb.PrintObjProp(n, nameof(Features), Features);
            return sb.ToString();
        }
    }
}
