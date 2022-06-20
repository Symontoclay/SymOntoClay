/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.InternalCG
{
    public class InternalConceptualGraph : BaseInternalConceptCGNode
    {
        public override bool IsConceptualGraph => true;
        public override InternalConceptualGraph AsConceptualGraph => this;
        public override KindOfCGNode Kind => KindOfCGNode.Graph;
        public KindOfQuestion KindOfQuestion { get; set; } = KindOfQuestion.Undefined;
        public GrammaticalTenses Tense { get; set; } = GrammaticalTenses.Undefined;
        public GrammaticalAspect Aspect { get; set; } = GrammaticalAspect.Undefined;
        public GrammaticalVoice Voice { get; set; } = GrammaticalVoice.Undefined;
        public GrammaticalMood Mood { get; set; } = GrammaticalMood.Undefined;
        public AbilityModality AbilityModality { get; set; } = AbilityModality.Undefined;
        public PermissionModality PermissionModality { get; set; } = PermissionModality.Undefined;
        public ObligationModality ObligationModality { get; set; } = ObligationModality.Undefined;
        public ProbabilityModality ProbabilityModality { get; set; } = ProbabilityModality.Undefined;
        public ConditionalModality ConditionalModality { get; set; } = ConditionalModality.Undefined;

        private IList<BaseInternalCGNode> mChildren = new List<BaseInternalCGNode>();

        public IList<BaseInternalCGNode> Children
        {
            get
            {
                return mChildren.ToList();
            }
        }

        public override IList<ICGNode> ChildrenNodes => mChildren.Cast<ICGNode>().ToList();

        public void AddChild(BaseInternalCGNode child)
        {
            if (child == null)
            {
                return;
            }

            if (mChildren.Contains(child))
            {
                return;
            }

            NAddChild(child);
            child.NSetParent(this);
        }

        internal void NAddChild(BaseInternalCGNode child)
        {
            if (!mChildren.Contains(child))
            {
                mChildren.Add(child);
            }
        }

        public void RemoveChild(BaseInternalCGNode child)
        {
            if (child == null)
            {
                return;
            }

            if (!mChildren.Contains(child))
            {
                return;
            }

            NRemoveChild(child);
            NRemoveParent(this);
        }

        internal void NRemoveChild(BaseInternalCGNode child)
        {
            if (mChildren.Contains(child))
            {
                mChildren.Remove(child);
            }
        }

        public override void Destroy()
        {
            Parent = null;
            foreach (var child in Children)
            {
                RemoveChild(child);
            }
            foreach (var node in Inputs)
            {
                NSRemoveInputNode(node);
            }

            foreach (var node in Outputs)
            {
                NSRemoveOutputNode(node);
            }
        }

        private Dictionary<string, string> mRealtionLinkedVarNameDict = new Dictionary<string, string>();
        public void AddLinkRelationToVarName(string relationName, string varName)
        {
            mRealtionLinkedVarNameDict[relationName] = varName;
        }

        public string GetVarNameForRelation(string relationName)
        {
            if (mRealtionLinkedVarNameDict.ContainsKey(relationName))
            {
                return mRealtionLinkedVarNameDict[relationName];
            }

            return string.Empty;
        }

        public int MaxVarCount { get; set; }

        public override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var nextNSpace = DisplayHelper.Spaces(nextN);
            var sb = new StringBuilder();
            sb.Append(base.PropertiesToString(n));
            sb.AppendLine($"{spaces}{nameof(Number)} = {Number}");
            sb.AppendLine($"{spaces}{nameof(KindOfQuestion)} = {KindOfQuestion}");
            sb.AppendLine($"{spaces}{nameof(Tense)} = {Tense}");
            sb.AppendLine($"{spaces}{nameof(Aspect)} = {Aspect}");
            sb.AppendLine($"{spaces}{nameof(Voice)} = {Voice}");
            sb.AppendLine($"{spaces}{nameof(AbilityModality)} = {AbilityModality}");
            sb.AppendLine($"{spaces}{nameof(PermissionModality)} = {PermissionModality}");
            sb.AppendLine($"{spaces}{nameof(ObligationModality)} = {ObligationModality}");
            sb.AppendLine($"{spaces}{nameof(ProbabilityModality)} = {ProbabilityModality}");
            sb.AppendLine($"{spaces}{nameof(ConditionalModality)} = {ConditionalModality}");
            sb.AppendLine($"{spaces}{nameof(Mood)} = {Mood}");
            sb.AppendLine($"{spaces}Begin {nameof(Children)}");
            foreach (var child in Children)
            {
                sb.Append(child.PropertiesToShortString(nextN));
            }
            sb.AppendLine($"{spaces}End {nameof(Children)}");
            sb.AppendLine($"{spaces}Begin RealtionLinkedVarNameDict");
            foreach (var relationLinkedVarKVPItem in mRealtionLinkedVarNameDict)
            {
                sb.AppendLine($"{nextNSpace}{relationLinkedVarKVPItem.Key} = {relationLinkedVarKVPItem.Value}");
            }
            sb.AppendLine($"{spaces}End RealtionLinkedVarNameDict");
            sb.AppendLine($"{spaces}{nameof(MaxVarCount)} = {MaxVarCount}");
            return sb.ToString();
        }

        public override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var nextNSpace = DisplayHelper.Spaces(nextN);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Number)} = {Number}");
            sb.AppendLine($"{spaces}{nameof(KindOfQuestion)} = {KindOfQuestion}");
            sb.AppendLine($"{spaces}{nameof(Tense)} = {Tense}");
            sb.AppendLine($"{spaces}{nameof(Aspect)} = {Aspect}");
            sb.AppendLine($"{spaces}{nameof(Voice)} = {Voice}");
            sb.AppendLine($"{spaces}{nameof(AbilityModality)} = {AbilityModality}");
            sb.AppendLine($"{spaces}{nameof(PermissionModality)} = {PermissionModality}");
            sb.AppendLine($"{spaces}{nameof(ObligationModality)} = {ObligationModality}");
            sb.AppendLine($"{spaces}{nameof(ProbabilityModality)} = {ProbabilityModality}");
            sb.AppendLine($"{spaces}{nameof(ConditionalModality)} = {ConditionalModality}");
            sb.AppendLine($"{spaces}{nameof(Mood)} = {Mood}");
            sb.AppendLine($"{spaces}Begin RealtionLinkedVarNameDict");
            foreach (var relationLinkedVarKVPItem in mRealtionLinkedVarNameDict)
            {
                sb.AppendLine($"{nextNSpace}{relationLinkedVarKVPItem.Key} = {relationLinkedVarKVPItem.Value}");
            }
            sb.AppendLine($"{spaces}End RealtionLinkedVarNameDict");
            sb.AppendLine($"{spaces}{nameof(MaxVarCount)} = {MaxVarCount}");
            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }
    }
}
