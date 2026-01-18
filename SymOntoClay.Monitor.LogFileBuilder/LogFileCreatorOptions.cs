/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.CoreHelper.SerializerAdapters;
using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems;
using SymOntoClay.Monitor.LogFileBuilder.TextRowOptionItems;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public class LogFileCreatorOptions : IObjectToString
    {
        public LogFileBuilderMode? Mode { get; set; } = LogFileBuilderMode.None;
        public string SourceDirectoryName { get; set; }
        public KindOfSerialization? SerializationMode { get; set; } = KindOfSerialization.MessagePack;
        public IEnumerable<string> TargetNodes { get; set; }
        public IEnumerable<string> TargetThreads { get; set; }
        public string OutputDirectory { get; set; }
        public IEnumerable<BaseFileNameTemplateOptionItem> FileNameTemplate { get; set; }
        public bool? SeparateOutputByNodes { get; set; }
        public bool? SeparateOutputByThreads { get; set; }
        public IEnumerable<KindOfMessage> KindOfMessages { get; set; }
        public IEnumerable<BaseMessageTextRowOptionItem> Layout { get; set; }
        public bool? Silent { get; set; }
        public string DotAppPath { get; set; }
        public bool? ToHtml { get; set; }
        public bool? IsAbsUrl { get; set; }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public LogFileCreatorOptions Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public LogFileCreatorOptions Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (LogFileCreatorOptions)context[this];
            }

            var result = new LogFileCreatorOptions();
            result.Mode = Mode;
            result.SourceDirectoryName = SourceDirectoryName;
            result.SerializationMode = SerializationMode;
            result.TargetNodes = TargetNodes?.ToList();
            result.TargetThreads = TargetThreads?.ToList();
            result.OutputDirectory = OutputDirectory;
            result.FileNameTemplate = FileNameTemplate?.ToList();
            result.SeparateOutputByNodes = SeparateOutputByNodes;
            result.SeparateOutputByThreads = SeparateOutputByThreads;
            result.KindOfMessages = KindOfMessages?.ToList();
            result.Layout = Layout?.ToList();
            result.Silent = Silent;
            result.DotAppPath = DotAppPath;
            result.ToHtml = ToHtml;
            result.IsAbsUrl = IsAbsUrl;

            return result;
        }

        public void Write(LogFileCreatorOptions source)
        {
            if (source.Mode != null)
            {
                Mode = source.Mode;
            }

            if (source.SourceDirectoryName != null)
            {
                SourceDirectoryName = source.SourceDirectoryName;
            }

            if (source.SerializationMode != null)
            {
                SerializationMode = source.SerializationMode;
            }

            if (source.TargetNodes != null)
            {
                TargetNodes = source.TargetNodes.ToList();
            }

            if (source.TargetThreads != null)
            {
                TargetThreads = source.TargetThreads.ToList();
            }

            if (source.OutputDirectory != null)
            {
                OutputDirectory = source.OutputDirectory;
            }

            if (source.FileNameTemplate != null)
            {
                FileNameTemplate = source.FileNameTemplate.ToList();
            }

            if (source.SeparateOutputByNodes != null)
            {
                SeparateOutputByNodes = source.SeparateOutputByNodes;
            }

            if (source.SeparateOutputByThreads != null)
            {
                SeparateOutputByThreads = source.SeparateOutputByThreads;
            }

            if (source.KindOfMessages != null)
            {
                KindOfMessages = source.KindOfMessages.ToList();
            }

            if (source.Layout != null)
            {
                Layout = source.Layout.ToList();
            }

            if(source.Silent.HasValue)
            {
                Silent = source.Silent;
            }

            if(source.DotAppPath != null)
            {
                DotAppPath = source.DotAppPath;
            }

            if (source.ToHtml.HasValue)
            {
                ToHtml = source.ToHtml;
            }

            if (source.IsAbsUrl.HasValue)
            {
                IsAbsUrl = source.IsAbsUrl;
            }
        }

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
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Mode)} = {Mode}");
            sb.AppendLine($"{spaces}{nameof(SourceDirectoryName)} = {SourceDirectoryName}");
            sb.AppendLine($"{spaces}{nameof(SerializationMode)} = {SerializationMode}");
            sb.PrintPODList(n, nameof(TargetNodes), TargetNodes);
            sb.PrintPODList(n, nameof(TargetThreads), TargetThreads);
            sb.AppendLine($"{spaces}{nameof(OutputDirectory)} = {OutputDirectory}");
            sb.PrintObjListProp(n, nameof(FileNameTemplate), FileNameTemplate);
            sb.AppendLine($"{spaces}{nameof(SeparateOutputByNodes)} = {SeparateOutputByNodes}");
            sb.AppendLine($"{spaces}{nameof(SeparateOutputByThreads)} = {SeparateOutputByThreads}");
            sb.PrintPODList(n, nameof(KindOfMessages), KindOfMessages);
            sb.PrintObjListProp(n, nameof(Layout), Layout); 
            sb.AppendLine($"{spaces}{nameof(Silent)} = {Silent}");
            sb.AppendLine($"{spaces}{nameof(DotAppPath)} = {DotAppPath}");
            sb.AppendLine($"{spaces}{nameof(ToHtml)} = {ToHtml}");
            sb.AppendLine($"{spaces}{nameof(IsAbsUrl)} = {IsAbsUrl}");
            //sb.AppendLine($"{spaces}{nameof()} = {}");
            return sb.ToString();
        }

        public static LogFileCreatorOptions DefaultOptions => new LogFileCreatorOptions()
        {
            FileNameTemplate = new List<BaseFileNameTemplateOptionItem>()
            {
                new NodeIdFileNameTemplateOptionItem(),
                new TextFileNameTemplateOptionItem()
                {
                    Text = "_",
                    IfNodeIdExists = true
                },
                new ThreadIdFileNameTemplateOptionItem(),
                new TextFileNameTemplateOptionItem()
                {
                    Text = "_",
                    IfThreadIdExists = true
                },
                new LongDateTimeFileNameTemplateOptionItem(),
                new ExtensionFileNameTemplateOptionItem()
                {
                    Text = ".log"
                }
            },
            SeparateOutputByNodes = true,
            SeparateOutputByThreads = false,
            KindOfMessages = new List<KindOfMessage>()
            {
                //KindOfMessage.Info
            },
            Layout = new List<BaseMessageTextRowOptionItem>
            {
                new GlobalMessageNumberTextRowOptionItem(),
                new LongDateTimeStampTextRowOptionItem(),
                new SpaceTextRowOptionItem(),
                new MessagePointIdTextRowOptionItem(),
                new SpaceTextRowOptionItem(),
                new ThreadIdTextRowOptionItem(),
                new SpaceTextRowOptionItem(),
                new ClassFullNameTextRowOptionItem(),
                new SpaceTextRowOptionItem(),
                new MemberNameTextRowOptionItem(),
                new SpaceTextRowOptionItem(),
                new KindOfMessageTextRowOptionItem
                {
                    TextTransformation = TextTransformations.UpperCase
                },
                new SpaceTextRowOptionItem(),
                new MessageContentTextRowOptionItem
                {
                    EnableCallMethodIdOfMethodLabel = true,
                    EnableMethodSignatureArguments = true,
                    EnableTypesListOfMethodSignatureArguments = true,
                    EnableDefaultValueOfMethodSignatureArguments = true,
                    EnablePassedValuesOfMethodLabel = true,
                    EnableChainOfProcessInfo = true
                }
            }
        };
    }
}
