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
using SymOntoClay.Common.DebugHelpers;
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

        private ConceptualGraph _parent;

        public ConceptualGraph Parent
        {
            get
            {
                return _parent;
            }

            set
            {
                if (_parent == value)
                {
                    return;
                }

                if (_parent != null)
                {
                    _parent.NRemoveChild(this);
                }

                _parent = value;

                if (_parent != null)
                {
                    _parent.NAddChild(this);
                }
            }
        }

        internal void NSetParent(ConceptualGraph parent)
        {
            if (_parent != parent)
            {
                _parent = parent;
            }
        }

        internal void NRemoveParent(ConceptualGraph parent)
        {
            if (_parent == parent)
            {
                _parent = null;
            }
        }

        public virtual IList<ICGNode> ChildrenNodes => new List<ICGNode>();

        private IList<BaseCGNode> _inputsNodes = new List<BaseCGNode>();
        private IList<BaseCGNode> _outputsNodes = new List<BaseCGNode>();

        public IList<BaseCGNode> Inputs
        {
            get
            {
                return _inputsNodes;
            }
        }

        public IList<ICGNode> InputNodes => _inputsNodes.Cast<ICGNode>().ToList();

        public IList<BaseCGNode> Outputs
        {
            get
            {
                return _outputsNodes;
            }
        }

        public IList<ICGNode> OutputNodes => _outputsNodes.Cast<ICGNode>().ToList();

        internal void NAddInputNode(BaseCGNode node)
        {
            if (!_inputsNodes.Contains(node))
            {
                _inputsNodes.Add(node);
            }
        }

        internal void NRemoveInputNode(BaseCGNode node)
        {
            if (_inputsNodes.Contains(node))
            {
                _inputsNodes.Remove(node);
            }
        }

        internal void NAddOutputNode(BaseCGNode node)
        {
            if (!_outputsNodes.Contains(node))
            {
                _outputsNodes.Add(node);
            }
        }

        internal void NRemoveOutputNode(BaseCGNode node)
        {
            if (_outputsNodes.Contains(node))
            {
                _outputsNodes.Remove(node);
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
