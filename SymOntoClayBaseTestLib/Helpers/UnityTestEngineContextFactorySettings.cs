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
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClayBaseTestLib.Helpers
{
    public class UnityTestEngineContextFactorySettings : IObjectToString
    {
        public string BaseDir { get; set; }
        public bool UseDefaultBaseDir { get; set; } = true;
        public string WorldFile { get; set; }
        public string NPCAppFile { get; set; }
        public bool UseDefaultAppFiles { get; set; } = true;
        public object HostListener { get; set; }
        public bool UseDefaultHostListener { get; set; } = true;
        public IPlatformLogger PlatformLogger { get; set; }
        public bool UseDefaultPlatformLogger { get; set; } = true;
        public Vector3? CurrentAbsolutePosition { get; set; }
        public bool UseDefaultCurrentAbsolutePosition { get; set; } = true;
        public IList<string> DictsPaths { get; set; }
        public IList<IWordsDict> DictsList { get; set; }
        public bool UseDefaultNLPSettings { get; set; } = true;
        public bool UseStandardLibrary { get; set; }

        public List<string> Categories { get; set; }
        public bool EnableCategories { get; set; }

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
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(BaseDir)} = {BaseDir}");
            sb.AppendLine($"{spaces}{nameof(UseDefaultBaseDir)} = {UseDefaultBaseDir}");

            sb.AppendLine($"{spaces}{nameof(WorldFile)} = {WorldFile}");
            sb.AppendLine($"{spaces}{nameof(NPCAppFile)} = {NPCAppFile}");
            sb.AppendLine($"{spaces}{nameof(UseDefaultAppFiles)} = {UseDefaultAppFiles}");

            sb.PrintExisting(n, nameof(HostListener), HostListener);
            sb.AppendLine($"{spaces}{nameof(UseDefaultHostListener)} = {UseDefaultHostListener}");

            sb.PrintExisting(n, nameof(PlatformLogger), PlatformLogger);
            sb.AppendLine($"{spaces}{nameof(UseDefaultPlatformLogger)} = {UseDefaultPlatformLogger}");

            sb.PrintExisting(n, nameof(CurrentAbsolutePosition), CurrentAbsolutePosition);
            sb.AppendLine($"{spaces}{nameof(UseDefaultCurrentAbsolutePosition)} = {UseDefaultCurrentAbsolutePosition}");

            sb.PrintPODList(n, nameof(DictsPaths), DictsPaths);
            sb.PrintExistingList(n, nameof(DictsList), DictsList);
            sb.AppendLine($"{spaces}{nameof(UseDefaultNLPSettings)} = {UseDefaultNLPSettings}");
            sb.AppendLine($"{spaces}{nameof(UseStandardLibrary)} = {UseStandardLibrary}");

            sb.PrintPODListProp(n, nameof(Categories), Categories);
            sb.AppendLine($"{spaces}{nameof(EnableCategories)} = {EnableCategories}");

            return sb.ToString();
        }
    }
}
