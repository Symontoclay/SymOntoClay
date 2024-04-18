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
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor
{
    public class MonitorSettings : BaseMonitorSettings
    {
        public string MessagesDir { get; set; }
        public IRemoteMonitor RemoteMonitor { get; set; }
        public Action<string> OutputHandler { get; set; }
        public Action<string> ErrorHandler { get; set; }
        /// <summary>
        /// Gets or sets list of platform specific loggers.
        /// It alows us to add, for example, console logger for Unity3D.
        /// </summary>
        public IList<IPlatformLogger> PlatformLoggers { get; set; }
        public IDictionary<string, BaseMonitorSettings> NodesSettings { get; set; }
        public bool EnableOnlyDirectlySetUpNodes { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(MessagesDir)} = {MessagesDir}");
            sb.PrintExisting(n, nameof(RemoteMonitor), RemoteMonitor);
            sb.PrintExisting(n, nameof(OutputHandler), OutputHandler);
            sb.PrintExisting(n, nameof(ErrorHandler), ErrorHandler);
            sb.PrintExistingList(n, nameof(PlatformLoggers), PlatformLoggers);
            sb.PrintObjDict_3_Prop(n, nameof(NodesSettings), NodesSettings);
            sb.AppendLine($"{spaces}{nameof(EnableOnlyDirectlySetUpNodes)} = {EnableOnlyDirectlySetUpNodes}");            
            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
