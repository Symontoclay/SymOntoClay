using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CLI
{
    public class CLIPlatformLogger: IPlatformLogger
    {
        /// <inheritdoc/>
        public void WriteLn(string message)
        {
            ConsoleWrapper.WriteText(message);
        }

        /// <inheritdoc/>
        public void WriteLnRawLogChannel(string message)
        {
            ConsoleWrapper.WriteLogChannel(message);
        }

        /// <inheritdoc/>
        public void WriteLnRawLog(string message)
        {
            Console.WriteLine(message);
        }

        /// <inheritdoc/>
        public void WriteLnRawWarning(string message)
        {
            ConsoleWrapper.WriteText(message);
        }

        /// <inheritdoc/>
        public void WriteLnRawError(string message)
        {
            ConsoleWrapper.WriteError(message);
        }
    }
}
