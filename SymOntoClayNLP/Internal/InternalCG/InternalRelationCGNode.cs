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

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.CG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.InternalCG
{
    public class InternalRelationCGNode : BaseInternalCGNode
    {
        public string LinkedVarName { get; set; }
        public override KindOfCGNode Kind => KindOfCGNode.Relation;
        public override bool IsRelationNode => true;
        public override InternalRelationCGNode AsRelationNode => this;

        public void AddInputNode(BaseInternalConceptCGNode node)
        {
            NAddInputNode(node);
            node.NAddOutputNode(this);
        }

        public void AddInputNode(InternalRelationCGNode node)
        {
            NAddInputNode(node);
            node.NAddOutputNode(this);
        }

        public void RemoveInputNode(BaseInternalConceptCGNode node)
        {
            NRemoveInputNode(node);
            node.NRemoveOutputNode(this);
        }

        public void RemoveInputNode(InternalRelationCGNode node)
        {
            NSRemoveInputNode(node);
        }

        private void NSRemoveInputNode(BaseInternalCGNode node)
        {
            NRemoveInputNode(node);
            node.NRemoveOutputNode(this);
        }

        public void AddOutputNode(BaseInternalConceptCGNode node)
        {
            NAddOutputNode(node);
            node.NAddInputNode(this);
        }

        public void AddOutputNode(InternalRelationCGNode node)
        {
            NAddOutputNode(node);
            node.NAddInputNode(this);
        }

        public void RemoveOutputNode(BaseInternalConceptCGNode node)
        {
            NRemoveOutputNode(node);
            node.NRemoveInputNode(this);
        }

        public void RemoveOutputNode(InternalRelationCGNode node)
        {
            NSRemoveOutputNode(node);
        }

        private void NSRemoveOutputNode(BaseInternalCGNode node)
        {
            NRemoveOutputNode(node);
            node.NRemoveInputNode(this);
        }

        public override void Destroy()
        {
            Parent = null;
            foreach (var node in Inputs)
            {
                NSRemoveInputNode(node);
            }

            foreach (var node in Outputs)
            {
                NSRemoveOutputNode(node);
            }
        }

        public override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.Append(base.PropertiesToString(n));
            sb.AppendLine($"{spaces}{nameof(LinkedVarName)} = {LinkedVarName}");
            return sb.ToString();
        }

        public override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.Append(base.PropertiesToShortString(n));
            sb.AppendLine($"{spaces}{nameof(LinkedVarName)} = {LinkedVarName}");
            return sb.ToString();
        }

        public override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.Append(base.PropertiesToBriefString(n));
            sb.AppendLine($"{spaces}{nameof(LinkedVarName)} = {LinkedVarName}");
            return sb.ToString();
        }
    }
}
