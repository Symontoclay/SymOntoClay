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
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.PlatformImplementations
{
    public class ConsoleLogger: IPlatformLogger
    {
        private static readonly ConsoleLogger __instance = new ConsoleLogger();

        /// <summary>
        /// Gets instance of the class.
        /// </summary>
        public static ConsoleLogger Instance => __instance;

        private ConsoleLogger()
        {
        }

        /// <inheritdoc/>
        public void WriteLnRawOutput(string message)
        {
            Console.WriteLine(message);
        }

        /// <inheritdoc/>
        public void WriteLnRawTrace(string message)
        {
            Console.WriteLine(message);
        }

        /// <inheritdoc/>
        public void WriteLnRawDebug(string message)
        {
            Console.WriteLine(message);
        }

        /// <inheritdoc/>
        public void WriteLnRawInfo(string message)
        {
            Console.WriteLine(message);
        }

        /// <inheritdoc/>
        public void WriteLnRawWarn(string message)
        {
            Console.WriteLine(message);
        }

        /// <inheritdoc/>
        public void WriteLnRawError(string message)
        {
            Console.WriteLine(message);
        }

        /// <inheritdoc/>
        public void WriteLnRawFatal(string message)
        {
            Console.WriteLine(message);
        }
    }
}
