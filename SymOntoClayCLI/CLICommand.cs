/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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
using SymOntoClayProjectFiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CLI
{
    public class CLICommand : IObjectToString
    {
        public KindOfCLICommand Kind { get; set; } = KindOfCLICommand.Unknown;
        public KindOfNewCommand KindOfNewCommand { get; set; } = KindOfNewCommand.Unknown;
        public string InputFile { get; set; }
        public string InputDir { get; set; }
        public string ProjectName { get; set; }
        public bool NoLogo { get; set; }
        public bool IsValid { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfNewCommand)} = {KindOfNewCommand}");
            sb.AppendLine($"{spaces}{nameof(InputFile)} = {InputFile}");
            sb.AppendLine($"{spaces}{nameof(InputDir)} = {InputDir}");
            sb.AppendLine($"{spaces}{nameof(ProjectName)} = {ProjectName}");
            sb.AppendLine($"{spaces}{nameof(NoLogo)} = {NoLogo}");
            sb.AppendLine($"{spaces}{nameof(IsValid)} = {IsValid}");

            return sb.ToString();
        }
    }
}
