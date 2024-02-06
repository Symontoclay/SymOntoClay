using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems;
using SymOntoClay.Monitor.LogFileBuilder.TextRowOptionItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public class LogFileCreatorOptions : IObjectToString
    {
        public string SourceDirectoryName { get; set; }
        public IEnumerable<string> TargetNodes { get; set; }
        public IEnumerable<string> TargetThreads { get; set; }
        public string OutputDirectory { get; set; }
        public IEnumerable<BaseFileNameTemplateOptionItem> FileNameTemplate { get; set; }
        public bool? SeparateOutputByNodes { get; set; }
        public bool? SeparateOutputByThreads { get; set; }
        public IEnumerable<KindOfMessage> KindOfMessages { get; set; }
        public IEnumerable<BaseMessageTextRowOptionItem> Layout { get; set; }
        public bool Silent { get; set; }
        public string DotAppPath { get; set; }

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
            result.SourceDirectoryName = SourceDirectoryName;
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

            return result;
        }

        public void Write(LogFileCreatorOptions source)
        {
            if(source.SourceDirectoryName != null)
            {
                SourceDirectoryName = source.SourceDirectoryName;
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

            Silent = source.Silent;

            if(source.DotAppPath != null)
            {
                DotAppPath = source.DotAppPath;
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
            sb.AppendLine($"{spaces}{nameof(SourceDirectoryName)} = {SourceDirectoryName}");
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
                new TextFileNameTemplateOptionItem()
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
                    EnablePassedVauesOfMethodLabel = true,
                    EnableChainOfProcessInfo = true
                }
            }
        };
    }
}
