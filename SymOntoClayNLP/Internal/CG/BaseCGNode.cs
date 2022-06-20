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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.CG
{
    public abstract class BaseCGNode : ICGNode
    {
        public abstract KindOfCGNode Kind { get; }
        public string Name { get; set; }

        private ConceptualGraph mParent;

        public ConceptualGraph Parent
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

        internal void NSetParent(ConceptualGraph parent)
        {
            if (mParent != parent)
            {
                mParent = parent;
            }
        }

        internal void NRemoveParent(ConceptualGraph parent)
        {
            if (mParent == parent)
            {
                mParent = null;
            }
        }

        public virtual IList<ICGNode> ChildrenNodes => new List<ICGNode>();

        private IList<BaseCGNode> mInputsNodes = new List<BaseCGNode>();
        private IList<BaseCGNode> mOutputsNodes = new List<BaseCGNode>();

        public IList<BaseCGNode> Inputs
        {
            get
            {
                return mInputsNodes;
            }
        }

        public IList<ICGNode> InputNodes => mInputsNodes.Cast<ICGNode>().ToList();

        public IList<BaseCGNode> Outputs
        {
            get
            {
                return mOutputsNodes;
            }
        }

        public IList<ICGNode> OutputNodes => mOutputsNodes.Cast<ICGNode>().ToList();

        internal void NAddInputNode(BaseCGNode node)
        {
            if (!mInputsNodes.Contains(node))
            {
                mInputsNodes.Add(node);
            }
        }

        internal void NRemoveInputNode(BaseCGNode node)
        {
            if (mInputsNodes.Contains(node))
            {
                mInputsNodes.Remove(node);
            }
        }

        internal void NAddOutputNode(BaseCGNode node)
        {
            if (!mOutputsNodes.Contains(node))
            {
                mOutputsNodes.Add(node);
            }
        }

        internal void NRemoveOutputNode(BaseCGNode node)
        {
            if (mOutputsNodes.Contains(node))
            {
                mOutputsNodes.Remove(node);
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

        public virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");

            sb.PrintShortObjProp(n, nameof(Parent), Parent);

            sb.PrintShortObjListProp(n, nameof(Inputs), Inputs);

            sb.PrintShortObjListProp(n, nameof(Outputs), Outputs);

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
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
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
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            return sb.ToString();
        }
    }
}
