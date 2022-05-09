using SymOntoClay.NLP.Internal.CG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.Dot
{
    public class DotContext
    {
        public BaseLeaf CreateRootLeaf(ICGNode node)
        {
            return new RootGraphDotLeaf(this, node);
        }

        public BaseLeaf CreateConceptualLeaf(ICGNode node)
        {
            return new ConceptualLeaf(this, node);
        }

        public BaseLeaf CreateSubGraphLeaf(ICGNode node)
        {
            return new SubGraphDotLeaf(this, node);
        }

        public BaseLeaf CreateRelationLeaf(ICGNode node)
        {
            return new RelationLeaf(this, node);
        }

        public BaseLeaf CreateLeaf(ICGNode node)
        {
            var kind = node.Kind;

            switch (kind)
            {
                case KindOfCGNode.Graph:
                    return CreateSubGraphLeaf(node);

                case KindOfCGNode.Concept:
                    return CreateConceptualLeaf(node);

                case KindOfCGNode.Relation:
                    return CreateRelationLeaf(node);

                default: throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private uint mN = 0;

        public virtual string GetNodeName()
        {
            mN++;

            return $"n_{mN}";
        }

        private uint mClusterN = 0;

        public virtual string GetClusterName()
        {
            mClusterN++;

            return $"cluster_{mClusterN}";
        }

        private Dictionary<ICGNode, BaseLeaf> mLeafsDict = new Dictionary<ICGNode, BaseLeaf>();

        public void RegLeaf(ICGNode node, BaseLeaf leaf)
        {
            if (mLeafsDict.ContainsKey(node))
            {
                return;
            }

            mLeafsDict.Add(node, leaf);
        }

        public BaseLeaf GetLeaf(ICGNode node)
        {
            return mLeafsDict[node];
        }
    }
}
