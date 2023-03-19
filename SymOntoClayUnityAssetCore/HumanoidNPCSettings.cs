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
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Biped NPC (Non-Player Character) setting.
    /// </summary>
    public class HumanoidNPCSettings: BaseManualControllingGameComponentSettings
    {
        /// <summary>
        /// Gets or sets file name of SymOntoClay logic file.
        /// The file describes active logic which will be executed on the NPC.
        /// </summary>
        public string LogicFile { get; set; }

        public IPlatformSupport PlatformSupport { get; set; }
        public IVisionProvider VisionProvider { get; set; }

        public List<string> Categories { get; set; }
        public bool EnableCategories { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(LogicFile)} = {LogicFile}");

            sb.PrintExisting(n, nameof(PlatformSupport), PlatformSupport);
            sb.PrintExisting(n, nameof(VisionProvider), VisionProvider);

            sb.PrintPODListProp(n, nameof(Categories), Categories);
            sb.AppendLine($"{spaces}{nameof(EnableCategories)} = {EnableCategories}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }
    }
}
