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

using SymOntoClay.Serialization.SmartValues;
using SymOntoClay.Serialization.SmartValues.Functors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Internal.FileCache
{
    public class ThreadLoggerFileCache : BaseFileCache
    {
#if DEBUG
        //private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public ThreadLoggerFileCache(SmartValue<string> absoluteDirectory, string relativeDirectory, string nodeId, string threadId)
            : base(new PathCombineSmartValue(absoluteDirectory, new ConstSmartValue<string>(threadId)), Path.Combine(relativeDirectory, threadId))
        {
#if DEBUG
            //_globalLogger.Info($"absoluteDirectory = {absoluteDirectory}");
            //_globalLogger.Info($"nodeId = {nodeId}");
            //_globalLogger.Info($"threadId = {threadId}");
#endif
        }
    }
}
