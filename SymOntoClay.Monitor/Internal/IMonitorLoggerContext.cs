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

using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Internal.FileCache;
using SymOntoClay.Serialization.SmartValues;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Monitor.Internal
{
    public interface IMonitorLoggerContext
    {
        MessageProcessor MessageProcessor { get; }
        IMonitorFeatures Features { get; }
        SmartValue<IList<IPlatformLogger>> PlatformLoggers { get; }
        IFileCache FileCache { get; }
        MessageNumberGenerator GlobalMessageNumberGenerator { get; }
        MessageNumberGenerator MessageNumberGenerator { get; }
        string NodeId { get; }
        string ThreadId { get; }
        bool EnableRemoteConnection { get; }
        bool EnableFullCallInfo { get; }
        bool EnableAsyncMessageCreation { get; }
        KindOfLogicalSearchExplain KindOfLogicalSearchExplain { get; }
        bool EnableAddingRemovingFactLoggingInStorages { get; }

        CancellationToken CancellationToken { get; }
        SmartValue<CustomThreadPoolSettings> ThreadingSettings { get; }
    }
}
