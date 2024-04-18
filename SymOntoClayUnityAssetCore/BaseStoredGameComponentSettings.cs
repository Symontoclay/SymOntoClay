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
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public abstract class BaseStoredGameComponentSettings: BaseGameComponentSettings
    {
        /// <summary>
        /// Gets or sets file name of SymOntoClay host file.
        /// The file describes facts which are visible for other NPCs or can be recognized in some way by player.
        /// </summary>
        public string HostFile { get; set; }
        public IPlatformSupport PlatformSupport { get; set; }

        public List<string> Categories { get; set; }
        public bool EnableCategories { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(HostFile)} = {HostFile}");
            sb.PrintExisting(n, nameof(PlatformSupport), PlatformSupport);

            sb.PrintPODListProp(n, nameof(Categories), Categories);
            sb.AppendLine($"{spaces}{nameof(EnableCategories)} = {EnableCategories}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }
    }
}
