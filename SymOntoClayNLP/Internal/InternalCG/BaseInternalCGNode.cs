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
using SymOntoClay.NLP.Internal.CG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.InternalCG
{
    public abstract class BaseInternalCGNode : ICGNode
    {
        public abstract KindOfCGNode Kind { get; }
        public string Name { get; set; }
        public bool IsNegation { get; set; }
        public KindOfSpecialRelation KindOfSpecialRelation { get; set; }
        public virtual bool IsConceptualGraph => false;
        public virtual InternalConceptualGraph AsConceptualGraph => null;
        public virtual bool IsConceptNode => false;
        public virtual InternalConceptCGNode AsConceptNode => null;
        public virtual bool IsRelationNode => false;
        public virtual InternalRelationCGNode AsRelationNode => null;
        public virtual bool IsGraphOrConceptNode => false;
        public virtual BaseInternalConceptCGNode AsGraphOrConceptNode => null;

        public abstract void Destroy();

        private InternalConceptualGraph mParent;

        public InternalConceptualGraph Parent
        {
            get
            {
                return mParent;
            }

            set
            {
                if (mParent == value)
                {
                    return;
                }

                if (mParent != null)
                {
                    mParent.NRemoveChild(this);
                }

                mParent = value;

                if (mParent != null)
                {
                    mParent.NAddChild(this);
                }
            }
        }

        internal void NSetParent(InternalConceptualGraph parent)
        {
            if (mParent != parent)
            {
                mParent = parent;
            }
        }

        internal void NRemoveParent(InternalConceptualGraph parent)
        {
            if (mParent == parent)
            {
                mParent = null;
            }
        }

        public virtual IList<ICGNode> ChildrenNodes => new List<ICGNode>();

        private IList<BaseInternalCGNode> mInputsNodes = new List<BaseInternalCGNode>();
        private IList<BaseInternalCGNode> mOutputsNodes = new List<BaseInternalCGNode>();

        public IList<BaseInternalCGNode> Inputs
        {
            get
            {
                return mInputsNodes.ToList();
            }
        }

        public IList<ICGNode> InputNodes => mInputsNodes.Cast<ICGNode>().ToList();

        public IList<BaseInternalCGNode> Outputs
        {
            get
            {
                return mOutputsNodes.ToList();
            }
        }

        public IList<ICGNode> OutputNodes => mOutputsNodes.Cast<ICGNode>().ToList();

        internal void NAddInputNode(BaseInternalCGNode node)
        {
            if (!mInputsNodes.Contains(node))
            {
                mInputsNodes.Add(node);
            }
        }

        internal void NRemoveInputNode(BaseInternalCGNode node)
        {
            if (mInputsNodes.Contains(node))
            {
                mInputsNodes.Remove(node);
            }
        }

        internal void NAddOutputNode(BaseInternalCGNode node)
        {
            if (!mOutputsNodes.Contains(node))
            {
                mOutputsNodes.Add(node);
            }
        }

        internal void NRemoveOutputNode(BaseInternalCGNode node)
        {
            if (mOutputsNodes.Contains(node))
            {
                mOutputsNodes.Remove(node);
            }
        }

        private IList<BaseInternalCGNode> mAnnotations = new List<BaseInternalCGNode>();
        public IList<BaseInternalCGNode> Annotations
        {
            get
            {
                return mAnnotations;
            }
        }

        public void AddAnnotation(BaseInternalCGNode annotation)
        {
            if (annotation == null)
            {
                return;
            }

            if (!mAnnotations.Contains(annotation))
            {
                mAnnotations.Add(annotation);
            }
        }

        public void RemoveAnnotation(BaseInternalCGNode annotation)
        {
            if (annotation == null)
            {
                return;
            }

            if (mAnnotations.Contains(annotation))
            {
                mAnnotations.Remove(annotation);
            }
        }

        public override string ToString()
        {
            return ToString(0u);
        }

        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        public virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            sb.AppendLine($"{spaces}{nameof(IsNegation)} = {IsNegation}");
            sb.AppendLine($"{spaces}{nameof(KindOfSpecialRelation)} = {KindOfSpecialRelation}");

            if (Parent == null)
            {
                sb.AppendLine($"{spaces}{nameof(Parent)} = null");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(Parent)}");
                sb.Append(Parent.PropertiesToShortString(nextN));
                sb.AppendLine($"{spaces}End {nameof(Parent)}");
            }

            sb.AppendLine($"{spaces}Begin {nameof(Inputs)}");
            foreach (var inputNode in Inputs)
            {
                sb.Append(inputNode.PropertiesToShortString(nextN));
            }
            sb.AppendLine($"{spaces}End {nameof(Inputs)}");

            sb.AppendLine($"{spaces}Begin {nameof(Outputs)}");
            foreach (var outputNode in Outputs)
            {
                sb.Append(outputNode.PropertiesToShortString(nextN));
            }
            sb.AppendLine($"{spaces}End {nameof(Outputs)}");
            sb.AppendLine($"{spaces}Begin {nameof(Annotations)}");
            foreach (var annotation in Annotations)
            {
                sb.Append(annotation.ToString(nextN));
            }
            sb.AppendLine($"{spaces}End {nameof(Annotations)}");
            return sb.ToString();
        }

        public string ToShortString()
        {
            return ToShortString(0u);
        }

        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        public virtual string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            sb.AppendLine($"{spaces}{nameof(KindOfSpecialRelation)} = {KindOfSpecialRelation}");
            return sb.ToString();
        }

        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        public virtual string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            sb.AppendLine($"{spaces}{nameof(KindOfSpecialRelation)} = {KindOfSpecialRelation}");
            return sb.ToString();
        }
    }
}
