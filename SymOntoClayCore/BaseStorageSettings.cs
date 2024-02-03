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

using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public abstract class BaseStorageSettings: BaseCoreSettings
    {
        /// <summary>
        /// Gets or sets unique Id.
        /// It allows us to identify each item of the game.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets app file.
        /// </summary>
        public string AppFile { get; set; }

        public IActivePeriodicObjectCommonContext SyncContext { get; set; }

        /// <summary>
        /// Gets or ses reference to shared modules storage.
        /// </summary>
        public IModulesStorage ModulesStorage { get; set; }

        /// <summary>
        /// Gets or sets parent storage.
        /// </summary>
        public IStandaloneStorage ParentStorage { get; set; }

        public ILogicQueryParseAndCache LogicQueryParseAndCache { get; set; }

        public List<string> Categories { get; set; }
        public bool EnableCategories { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Id)} = {Id}");
            sb.AppendLine($"{spaces}{nameof(AppFile)} = {AppFile}");

            sb.PrintExisting(n, nameof(SyncContext), SyncContext);
            sb.PrintExisting(n, nameof(ModulesStorage), ModulesStorage);
            sb.PrintExisting(n, nameof(ParentStorage), ParentStorage);
            sb.PrintExisting(n, nameof(LogicQueryParseAndCache), LogicQueryParseAndCache);

            sb.PrintPODListProp(n, nameof(Categories), Categories);
            sb.AppendLine($"{spaces}{nameof(EnableCategories)} = {EnableCategories}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }
    }
}
