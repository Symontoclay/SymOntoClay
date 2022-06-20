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

using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.CG
{
    public class RelationCGNode : BaseCGNode
    {
        /// <inheritdoc/>
        public override KindOfCGNode Kind => KindOfCGNode.Relation;

        public void AddInputNode(BaseConceptCGNode node)
        {
            NAddInputNode(node);
            node.NAddOutputNode(this);
        }

        public void AddInputNode(RelationCGNode node)
        {
            NAddInputNode(node);
            node.NAddOutputNode(this);
        }

        public void RemoveInputNode(BaseConceptCGNode node)
        {
            NRemoveInputNode(node);
            node.NRemoveOutputNode(this);
        }

        public void RemoveInputNode(RelationCGNode node)
        {
            NRemoveInputNode(node);
            node.NRemoveOutputNode(this);
        }

        public void AddOutputNode(BaseConceptCGNode node)
        {
            NAddOutputNode(node);
            node.NAddInputNode(this);
        }

        public void AddOutputNode(RelationCGNode node)
        {
            NAddOutputNode(node);
            node.NAddInputNode(this);
        }

        public void RemoveOutputNode(BaseConceptCGNode node)
        {
            NRemoveOutputNode(node);
            node.NRemoveInputNode(this);
        }

        public void RemoveOutputNode(RelationCGNode node)
        {
            NRemoveOutputNode(node);
            node.NRemoveInputNode(this);
        }
    }
}
