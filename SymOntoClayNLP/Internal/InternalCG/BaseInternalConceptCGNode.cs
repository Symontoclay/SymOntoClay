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
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.InternalCG
{
    public abstract class BaseInternalConceptCGNode : BaseInternalCGNode
    {
        public override bool IsGraphOrConceptNode => true;
        public override BaseInternalConceptCGNode AsGraphOrConceptNode => this;
        public KindOfInternalGraphOrConceptNode KindOfGraphOrConcept { get; set; } = KindOfInternalGraphOrConceptNode.Undefined;
        public GrammaticalNumberOfWord Number { get; set; } = GrammaticalNumberOfWord.Neuter;
        public GrammaticalComparison Comparison { get; set; } = GrammaticalComparison.None;

        public void AddInputNode(InternalRelationCGNode node)
        {
            NAddInputNode(node);
            node.NAddOutputNode(this);
        }

        public void RemoveInputNode(InternalRelationCGNode node)
        {
            NSRemoveInputNode(node);
        }

        protected void NSRemoveInputNode(BaseInternalCGNode node)
        {
            NRemoveInputNode(node);
            node.NRemoveOutputNode(this);
        }

        public void AddOutputNode(InternalRelationCGNode node)
        {
            NAddOutputNode(node);
            node.NAddInputNode(this);
        }

        public void RemoveOutputNode(InternalRelationCGNode node)
        {
            NSRemoveOutputNode(node);
        }
        protected void NSRemoveOutputNode(BaseInternalCGNode node)
        {
            NRemoveOutputNode(node);
            node.NRemoveInputNode(this);
        }

        public override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(KindOfGraphOrConcept)} = {KindOfGraphOrConcept}");
            sb.AppendLine($"{spaces}{nameof(Comparison)} = {Comparison}");
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        public override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(KindOfGraphOrConcept)} = {KindOfGraphOrConcept}");
            sb.AppendLine($"{spaces}{nameof(Comparison)} = {Comparison}");
            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }
    }
}
