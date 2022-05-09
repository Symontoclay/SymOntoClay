using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.CG
{
    public abstract class BaseConceptCGNode : BaseCGNode
    {
        public void AddInputNode(RelationCGNode node)
        {
            NAddInputNode(node);
            node.NAddOutputNode(this);
        }

        public void RemoveInputNode(RelationCGNode node)
        {
            NRemoveInputNode(node);
            node.NRemoveOutputNode(this);
        }

        public void AddOutputNode(RelationCGNode node)
        {
            NAddOutputNode(node);
            node.NAddInputNode(this);
        }

        public void RemoveOutputNode(RelationCGNode node)
        {
            NRemoveOutputNode(node);
            node.NRemoveInputNode(this);
        }
    }
}
