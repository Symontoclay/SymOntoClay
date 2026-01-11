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
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Htn
{
    public class BuildPlanIterationContext : IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString
    {
        public int ProcessedIndex { get; set; } = -1;
        public List<BuiltPlanItem> TasksToProcess { get; set; } = new List<BuiltPlanItem>();
        public List<StrongIdentifierValue> VisitedCompoundTasks { get; set; } = new List<StrongIdentifierValue>();
        public List<StrongIdentifierValue> PreviousRootTasks { get; set; } = new List<StrongIdentifierValue>();
        public StrongIdentifierValue RootTask { get; set; }
        public List<StrongIdentifierValue> AllRootTasks { get; set; } = new List<StrongIdentifierValue>();

        public BuildPlanIterationLocalCodeExecutionContext LocalCodeExecutionContext { get; set; }

        public Dictionary<Guid, Guid> TaskCaseDict { get; set; } = new Dictionary<Guid, Guid>();
        public List<CompoundHtnTaskCase> UsedCases { get; set; } = new List<CompoundHtnTaskCase>();

        public bool IsNormal => _isNormal;

        private bool _isNormal = true;

        public void MarkAsFailed()
        {
            _isNormal = false;
        }

        public List<BaseCompoundHtnTask> TasksWithBackground { get; set; } = new List<BaseCompoundHtnTask>();

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public BuildPlanIterationContext Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public BuildPlanIterationContext Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (BuildPlanIterationContext)context[this];
            }

            var result = new BuildPlanIterationContext();
            context[this] = result;

            result.ProcessedIndex = ProcessedIndex;
            result.TasksToProcess = TasksToProcess.ToList();
            result.VisitedCompoundTasks = VisitedCompoundTasks.ToList();
            result.LocalCodeExecutionContext = LocalCodeExecutionContext.Clone(context);
            result._isNormal = _isNormal;
            result.PreviousRootTasks = PreviousRootTasks.ToList();
            result.RootTask = RootTask;
            result.AllRootTasks = AllRootTasks.ToList();
            result.TasksWithBackground = TasksWithBackground.ToList();
            result.TaskCaseDict = TaskCaseDict.ToDictionary(p => p.Key, p => p.Value);
            result.UsedCases = UsedCases.ToList();

            return result;
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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsNormal)} = {IsNormal}");
            sb.AppendLine($"{spaces}{nameof(ProcessedIndex)} = {ProcessedIndex}");
            sb.PrintObjListProp(n, nameof(TasksToProcess), TasksToProcess);
            sb.PrintObjListProp(n, nameof(VisitedCompoundTasks), VisitedCompoundTasks);
            sb.PrintExisting(n, nameof(LocalCodeExecutionContext), LocalCodeExecutionContext);
            sb.PrintObjListProp(n, nameof(PreviousRootTasks), PreviousRootTasks);
            sb.PrintObjProp(n, nameof(RootTask), RootTask);
            sb.PrintObjListProp(n, nameof(AllRootTasks), AllRootTasks);
            sb.PrintObjListProp(n, nameof(TasksWithBackground), TasksWithBackground);
            sb.PrintPODDictProp(n, nameof(TaskCaseDict), TaskCaseDict); 
            sb.PrintObjListProp(n, nameof(UsedCases), UsedCases);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsNormal)} = {IsNormal}");
            sb.AppendLine($"{spaces}{nameof(ProcessedIndex)} = {ProcessedIndex}");
            sb.PrintShortObjListProp(n, nameof(TasksToProcess), TasksToProcess);
            sb.PrintShortObjListProp(n, nameof(VisitedCompoundTasks), VisitedCompoundTasks);
            sb.PrintExisting(n, nameof(LocalCodeExecutionContext), LocalCodeExecutionContext);
            sb.PrintShortObjListProp(n, nameof(PreviousRootTasks), PreviousRootTasks);
            sb.PrintShortObjProp(n, nameof(RootTask), RootTask);
            sb.PrintShortObjListProp(n, nameof(AllRootTasks), AllRootTasks);
            sb.PrintExisting(n, nameof(TasksWithBackground), TasksWithBackground);
            sb.PrintPODDictProp(n, nameof(TaskCaseDict), TaskCaseDict);
            sb.PrintShortObjListProp(n, nameof(UsedCases), UsedCases);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsNormal)} = {IsNormal}");
            sb.AppendLine($"{spaces}{nameof(ProcessedIndex)} = {ProcessedIndex}");
            sb.PrintBriefObjListProp(n, nameof(TasksToProcess), TasksToProcess);
            sb.PrintBriefObjListProp(n, nameof(VisitedCompoundTasks), VisitedCompoundTasks);
            sb.PrintExisting(n, nameof(LocalCodeExecutionContext), LocalCodeExecutionContext);
            sb.PrintBriefObjListProp(n, nameof(PreviousRootTasks), PreviousRootTasks);
            sb.PrintBriefObjProp(n, nameof(RootTask), RootTask);
            sb.PrintBriefObjListProp(n, nameof(AllRootTasks), AllRootTasks);
            sb.PrintExisting(n, nameof(TasksWithBackground), TasksWithBackground);
            sb.PrintPODDictProp(n, nameof(TaskCaseDict), TaskCaseDict);
            sb.PrintBriefObjListProp(n, nameof(UsedCases), UsedCases);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToDbgString()
        {
            return ToDbgString(0u);
        }

        /// <inheritdoc/>
        public string ToDbgString(uint n)
        {
            return this.GetDefaultToDbgStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToDbgString.PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);

            var nextN = n + DisplayHelper.IndentationStep;

            var nextNSpaces = DisplayHelper.Spaces(nextN);

            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsNormal)}: {IsNormal}");
            sb.AppendLine($"{spaces}{nameof(ProcessedIndex)}: {ProcessedIndex}");
            sb.AppendLine($"{spaces}Tasks to process:");
            var i = 0;
            foreach(var task in TasksToProcess)
            {
                var arrowMark = i == ProcessedIndex ? "->" : string.Empty;

                sb.AppendLine($"{nextNSpaces}{arrowMark}{i}: {task.ProcessedTask?.ToHumanizedLabel()}");
                i++;
            }

            sb.AppendLine($"{spaces}Visited compound tasks: [{string.Join(", ", VisitedCompoundTasks.Select(p => p.ToSystemString()))}]");
            sb.AppendLine($"{spaces}Previous Root tasks: [{string.Join(", ", PreviousRootTasks.Select(p => p.ToSystemString()))}]");
            sb.AppendLine($"{spaces}Root task: {RootTask.ToHumanizedString()}");
            sb.AppendLine($"{spaces}All Root tasks: [{string.Join(", ", AllRootTasks.Select(p => p.ToSystemString()))}]");
            sb.AppendLine($"{spaces}Tasks with background: [{string.Join(", ", TasksWithBackground.Select(p => p.ToHumanizedString()))}]");
            //sb.PrintPODDictProp(n, nameof(TaskCaseDict), TaskCaseDict);

            return sb.ToString();
        }
    }
}
