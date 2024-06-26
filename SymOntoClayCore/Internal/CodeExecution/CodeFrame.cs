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

using SymOntoClay.Common;
using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.Threads;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class CodeFrame : IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString
    {
        public CompiledFunctionBody CompiledFunctionBody { get; set; }
        public int CurrentPosition { get; set; }
        public SEHGroup CurrentSEHGroup { get; set; }
        public Stack<Value> ValuesStack { get; private set; } = new Stack<Value>();
        public Stack<SEHGroup> SEHStack { get; private set; } = new Stack<SEHGroup>();        
        public ILocalCodeExecutionContext LocalContext { get; set; }
        public ProcessInfo ProcessInfo { get; set; }
        public CodeItem Metadata { get; set; }
        public Dictionary<StrongIdentifierValue, Value> Arguments { get; private set; } = new Dictionary<StrongIdentifierValue, Value>();
        public string CallMethodId { get; set; }
        public IInstance Instance { get; set; }
        public IExecutionCoordinator ExecutionCoordinator { get; set; }
        public SpecialMarkOfCodeFrame SpecialMark { get; set; } = SpecialMarkOfCodeFrame.None;
        public long? TargetDuration { get; set; }
        public long? EndOfTargetDuration { get; set; }
        public TimeoutCancellationMode TimeoutCancellationMode { get; set; } = TimeoutCancellationMode.WeakCancel;
        public List<StrongIdentifierValue> CalledCtorsList { get; set; } = new List<StrongIdentifierValue>();
        public Value PutToValueStackArterReturningBack { get; set; }

        public bool NeedsExecCallEvent { get; set; }
        public ProcessStatus? LastProcessStatus { get; set; }
        public AnnotationSystemEvent CompleteAnnotationSystemEvent { get; set; }
        public AnnotationSystemEvent CancelAnnotationSystemEvent { get; set; }
        public AnnotationSystemEvent WeakCancelAnnotationSystemEvent { get; set; }
        public AnnotationSystemEvent ErrorAnnotationSystemEvent { get; set; }
        public ThreadTask PseudoSyncTask { get; set; }

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
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.AppendLine($"{spaces}{nameof(CurrentPosition)} = {CurrentPosition}");

            sb.PrintObjProp(n, nameof(CurrentSEHGroup), CurrentSEHGroup);

            sb.PrintObjListProp(n, nameof(ValuesStack), ValuesStack.ToList());

            sb.PrintObjListProp(n, nameof(SEHStack), SEHStack.ToList());

            sb.PrintObjProp(n, nameof(LocalContext), LocalContext);

            sb.PrintBriefObjProp(n, nameof(ProcessInfo), ProcessInfo);

            sb.PrintObjProp(n, nameof(Metadata), Metadata);
            sb.PrintObjDict_1_Prop(n, nameof(Arguments), Arguments);
            sb.AppendLine($"{spaces}{nameof(CallMethodId)} = {CallMethodId}");
            sb.PrintObjProp(n, nameof(Instance), Instance);
            sb.PrintObjProp(n, nameof(ExecutionCoordinator), ExecutionCoordinator);
            sb.AppendLine($"{spaces}{nameof(SpecialMark)} = {SpecialMark}");
            sb.AppendLine($"{spaces}{nameof(TargetDuration)} = {TargetDuration}");
            sb.AppendLine($"{spaces}{nameof(EndOfTargetDuration)} = {EndOfTargetDuration}");
            sb.AppendLine($"{spaces}{nameof(TimeoutCancellationMode)} = {TimeoutCancellationMode}");
            sb.PrintObjListProp(n, nameof(CalledCtorsList), CalledCtorsList);
            sb.PrintObjProp(n, nameof(PutToValueStackArterReturningBack), PutToValueStackArterReturningBack);

            sb.AppendLine($"{spaces}{nameof(NeedsExecCallEvent)} = {NeedsExecCallEvent}");
            sb.AppendLine($"{spaces}{nameof(LastProcessStatus)} = {LastProcessStatus}");
            sb.PrintObjProp(n, nameof(CompleteAnnotationSystemEvent), CompleteAnnotationSystemEvent);
            sb.PrintObjProp(n, nameof(CancelAnnotationSystemEvent), CancelAnnotationSystemEvent);
            sb.PrintObjProp(n, nameof(WeakCancelAnnotationSystemEvent), WeakCancelAnnotationSystemEvent);
            sb.PrintObjProp(n, nameof(ErrorAnnotationSystemEvent), ErrorAnnotationSystemEvent);
            sb.PrintExisting(n, nameof(PseudoSyncTask), PseudoSyncTask);

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
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.AppendLine($"{spaces}{nameof(CurrentPosition)} = {CurrentPosition}");

            sb.PrintShortObjProp(n, nameof(CurrentSEHGroup), CurrentSEHGroup);

            sb.PrintShortObjListProp(n, nameof(ValuesStack), ValuesStack.ToList());

            sb.PrintShortObjListProp(n, nameof(SEHStack), SEHStack.ToList());

            sb.PrintShortObjProp(n, nameof(LocalContext), LocalContext);

            sb.PrintBriefObjProp(n, nameof(ProcessInfo), ProcessInfo);

            sb.PrintShortObjProp(n, nameof(Metadata), Metadata);
            sb.PrintShortObjDict_1_Prop(n, nameof(Arguments), Arguments);
            sb.AppendLine($"{spaces}{nameof(CallMethodId)} = {CallMethodId}");
            sb.PrintShortObjProp(n, nameof(Instance), Instance);
            sb.PrintShortObjProp(n, nameof(ExecutionCoordinator), ExecutionCoordinator);
            sb.AppendLine($"{spaces}{nameof(SpecialMark)} = {SpecialMark}");
            sb.AppendLine($"{spaces}{nameof(TargetDuration)} = {TargetDuration}");
            sb.AppendLine($"{spaces}{nameof(EndOfTargetDuration)} = {EndOfTargetDuration}");
            sb.AppendLine($"{spaces}{nameof(TimeoutCancellationMode)} = {TimeoutCancellationMode}");
            sb.PrintShortObjListProp(n, nameof(CalledCtorsList), CalledCtorsList);
            sb.PrintShortObjProp(n, nameof(PutToValueStackArterReturningBack), PutToValueStackArterReturningBack);

            sb.AppendLine($"{spaces}{nameof(NeedsExecCallEvent)} = {NeedsExecCallEvent}");
            sb.AppendLine($"{spaces}{nameof(LastProcessStatus)} = {LastProcessStatus}");
            sb.PrintShortObjProp(n, nameof(CompleteAnnotationSystemEvent), CompleteAnnotationSystemEvent);
            sb.PrintShortObjProp(n, nameof(CancelAnnotationSystemEvent), CancelAnnotationSystemEvent);
            sb.PrintShortObjProp(n, nameof(WeakCancelAnnotationSystemEvent), WeakCancelAnnotationSystemEvent);
            sb.PrintShortObjProp(n, nameof(ErrorAnnotationSystemEvent), ErrorAnnotationSystemEvent);
            sb.PrintExisting(n, nameof(PseudoSyncTask), PseudoSyncTask);

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
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.AppendLine($"{spaces}{nameof(CurrentPosition)} = {CurrentPosition}");

            sb.PrintBriefObjProp(n, nameof(CurrentSEHGroup), CurrentSEHGroup);

            sb.PrintBriefObjListProp(n, nameof(ValuesStack), ValuesStack.ToList());

            sb.PrintBriefObjListProp(n, nameof(SEHStack), SEHStack.ToList());

            sb.PrintBriefObjProp(n, nameof(LocalContext), LocalContext);

            sb.PrintExisting(n, nameof(ProcessInfo), ProcessInfo);

            sb.PrintBriefObjProp(n, nameof(Metadata), Metadata);
            sb.PrintBriefObjDict_1_Prop(n, nameof(Arguments), Arguments);
            sb.AppendLine($"{spaces}{nameof(CallMethodId)} = {CallMethodId}");
            sb.PrintBriefObjProp(n, nameof(Instance), Instance);
            sb.PrintBriefObjProp(n, nameof(ExecutionCoordinator), ExecutionCoordinator);
            sb.AppendLine($"{spaces}{nameof(SpecialMark)} = {SpecialMark}");
            sb.AppendLine($"{spaces}{nameof(TargetDuration)} = {TargetDuration}");
            sb.AppendLine($"{spaces}{nameof(EndOfTargetDuration)} = {EndOfTargetDuration}");
            sb.AppendLine($"{spaces}{nameof(TimeoutCancellationMode)} = {TimeoutCancellationMode}");
            sb.PrintBriefObjListProp(n, nameof(CalledCtorsList), CalledCtorsList);
            sb.PrintBriefObjProp(n, nameof(PutToValueStackArterReturningBack), PutToValueStackArterReturningBack);

            sb.AppendLine($"{spaces}{nameof(NeedsExecCallEvent)} = {NeedsExecCallEvent}");
            sb.AppendLine($"{spaces}{nameof(LastProcessStatus)} = {LastProcessStatus}");
            sb.PrintExisting(n, nameof(CompleteAnnotationSystemEvent), CompleteAnnotationSystemEvent);
            sb.PrintExisting(n, nameof(CancelAnnotationSystemEvent), CancelAnnotationSystemEvent);
            sb.PrintExisting(n, nameof(WeakCancelAnnotationSystemEvent), WeakCancelAnnotationSystemEvent);
            sb.PrintExisting(n, nameof(ErrorAnnotationSystemEvent), ErrorAnnotationSystemEvent);
            sb.PrintExisting(n, nameof(PseudoSyncTask), PseudoSyncTask);

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
            sb.AppendLine($"{spaces}CallMethodId: {CallMethodId}");
            if (Instance != null)
            {
                sb.AppendLine($"{spaces}Instance: {Instance.Name.NameValue}");
            }

            if(ExecutionCoordinator != null)
            {
                sb.AppendLine($"{spaces}ExecutionCoordinator: {ExecutionCoordinator.Id}");
            }

            if(Metadata != null)
            {
                var holder = Metadata.Holder;

                if(holder != null)
                {
                    sb.AppendLine($"{spaces}Holder: {holder.NameValue}");
                }

                sb.AppendLine($"{spaces}{Metadata.ToHumanizedLabel()}");
            }

            sb.AppendLine($"{spaces}Begin Arguments");
            if (!Arguments.IsNullOrEmpty())
            {
                foreach(var item in Arguments)
                {
                    sb.AppendLine($"{nextNSpaces}{item.Key.ToHumanizedLabel()} = {item.Value.ToHumanizedLabel()}");
                }
            }
            sb.AppendLine($"{spaces}End Arguments");

            sb.AppendLine($"{spaces}Begin Code");

            foreach (var commandItem in CompiledFunctionBody.Commands)
            {
                var currentMark = string.Empty;

                if(commandItem.Key == CurrentPosition)
                {
                    currentMark = "-> ";
                }

                sb.AppendLine($"{nextNSpaces}{currentMark}{commandItem.Key}: {commandItem.Value.ToDbgString()}");
            }
            sb.AppendLine($"{spaces}End Code");
            sb.AppendLine($"{spaces}Begin Values Stack");
            foreach(var stackItem in ValuesStack)
            {
                sb.AppendLine($"{nextNSpaces}{stackItem.ToDbgString()}");
            }
            sb.AppendLine($"{spaces}End Values Stack");
            return sb.ToString();
        }
    }
}
