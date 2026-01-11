/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

        private Dictionary<ICGNode, BaseLeaf> mLeavesDict = new Dictionary<ICGNode, BaseLeaf>();

        public void RegLeaf(ICGNode node, BaseLeaf leaf)
        {
            if (mLeavesDict.ContainsKey(node))
            {
                return;
            }

            mLeavesDict.Add(node, leaf);
        }

        public BaseLeaf GetLeaf(ICGNode node)
        {
            return mLeavesDict[node];
        }
    }
}
