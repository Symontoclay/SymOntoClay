/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Logging settings.
    /// </summary>
    public class LoggingSettings: IObjectToString
    {
        /// <summary>
        /// Gets or sets root dir for file logging.
        /// </summary>
        public string LogDir { get; set; }

        /// <summary>
        /// Gets or sets root contract name for remote logging or debugging game logic.
        /// </summary>
        public string RootContractName { get; set; }

        /// <summary>
        /// Gets or sets value of enable logging.
        /// It alows enable or disable logging or remote connection for whole components synchronously.
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// Gets or sets value of enable remote connection.
        /// It alows enable or disable remote connection for whole components synchronously.
        /// It doesn't touch local logging.
        /// </summary>
        public bool EnableRemoteConnection { get; set; }

        /// <summary>
        /// Gets or sets list of platform specific loggers.
        /// It alows us to add, for example, console logger for Unity3D.
        /// </summary>
        public IList<IPlatformLogger> PlatformLoggers { get; set; }

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
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(LogDir)} = {LogDir}");
            sb.AppendLine($"{spaces}{nameof(RootContractName)} = {RootContractName}");
            sb.AppendLine($"{spaces}{nameof(Enable)} = {Enable}");
            sb.AppendLine($"{spaces}{nameof(EnableRemoteConnection)} = {EnableRemoteConnection}");
            sb.PrintExistingList(n, nameof(PlatformLoggers), PlatformLoggers);
            return sb.ToString();
        }
    }
}
