/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using System;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// General settings of a game world.
    /// </summary>
    public class WorldSettings: IObjectToString
    {
        /// <summary>
        /// Gets or sets list of file paths for describing places for searching shared modules.
        /// </summary>
        public IList<string> LibsDirs { get; set; }

        /// <summary>
        /// Gets or sets root dir for saving and loading images of executed code.
        /// </summary>
        public string ImagesRootDir { get; set; }

        /// <summary>
        /// Gets or sets list of file paths for describing places searching dictionaries of translation text to facts.
        /// </summary>
        public IList<string> DictionariesDirs { get; set; }

        public string BuiltInStandardLibraryDir { get; set; }

        public string TmpDir { get; set; }

        public IMonitor Monitor { get; set; }

        /// <summary>
        /// Gets or sets logging settings.
        /// </summary>
        public LoggingSettings Logging { get; set; }

        /// <summary>
        /// Gets or sets file name of SymOntoClay host file.
        /// The file describes facts which are visible for other NPCs or can be recognized in some way by player.
        /// </summary>
        public string HostFile { get; set; }

        public IInvokerInMainThread InvokerInMainThread { get; set; }

        public ISoundBus SoundBus { get; set; }

        public INLPConverterProvider NLPConverterProvider { get; set; }

        public IStandardFactsBuilder StandardFactsBuilder { get; set; }

        public bool EnableAutoloadingConvertors { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Logging), Logging);
            sb.PrintPODList(n, nameof(LibsDirs), LibsDirs);
            sb.AppendLine($"{spaces}{nameof(ImagesRootDir)} = {ImagesRootDir}");
            sb.AppendLine($"{spaces}{nameof(BuiltInStandardLibraryDir)} = {BuiltInStandardLibraryDir}");
            sb.PrintPODList(n, nameof(DictionariesDirs), DictionariesDirs);
            sb.AppendLine($"{spaces}{nameof(TmpDir)} = {TmpDir}");
            sb.PrintExisting(n, nameof(Monitor), Monitor);
            sb.AppendLine($"{spaces}{nameof(HostFile)} = {HostFile}");
            sb.PrintExisting(n, nameof(InvokerInMainThread), InvokerInMainThread);
            sb.PrintExisting(n, nameof(SoundBus), SoundBus);
            sb.PrintExisting(n, nameof(NLPConverterProvider), NLPConverterProvider);
            sb.PrintExisting(n, nameof(StandardFactsBuilder), StandardFactsBuilder);

            sb.AppendLine($"{spaces}{nameof(EnableAutoloadingConvertors)} = {EnableAutoloadingConvertors}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }
    }
}
