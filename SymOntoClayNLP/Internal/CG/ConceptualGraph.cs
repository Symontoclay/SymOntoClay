using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.CG
{
    public class ConceptualGraph : BaseConceptCGNode
    {
        /// <inheritdoc/>
        public override KindOfCGNode Kind => KindOfCGNode.Graph;

        private IList<BaseCGNode> mChildren = new List<BaseCGNode>();

        private readonly object mPrevGraphLockObj = new object();
        private ConceptualGraph mPrevGraph;
        internal void NSetPrevGraph(ConceptualGraph graph)
        {
            lock (mPrevGraphLockObj)
            {
                if (mPrevGraph == graph)
                {
                    return;
                }

                mPrevGraph = graph;
            }
        }

        internal void NResetPrevGraph(ConceptualGraph graph)
        {
            lock (mPrevGraphLockObj)
            {
                if (mPrevGraph == graph)
                {
                    mPrevGraph = null;
                }
            }
        }

        private readonly object mNextGraphLockObj = new object();
        private ConceptualGraph mNextGraph;
        internal void NSetNextGraph(ConceptualGraph graph)
        {
            lock (mNextGraphLockObj)
            {
                if (mNextGraph == graph)
                {
                    return;
                }

                mNextGraph = graph;
            }
        }

        internal void NResetNextGraph(ConceptualGraph graph)
        {
            lock (mNextGraphLockObj)
            {
                if (mNextGraph == graph)
                {
                    mNextGraph = null;
                }
            }
        }

        public ConceptualGraph PrevGraph
        {
            get
            {
                lock (mPrevGraphLockObj)
                {
                    return mPrevGraph;
                }
            }

            set
            {
                lock (mPrevGraphLockObj)
                {
                    if (mPrevGraph == value)
                    {
                        return;
                    }

                    if (mPrevGraph != null)
                    {
                        mPrevGraph.NResetNextGraph(this);
                    }

                    mPrevGraph = value;

                    if (mPrevGraph != null)
                    {
                        mPrevGraph.NSetNextGraph(this);
                    }
                }
            }
        }

        public ConceptualGraph NextGraph
        {
            get
            {
                lock (mNextGraphLockObj)
                {
                    return mNextGraph;
                }
            }

            set
            {
                lock (mNextGraphLockObj)
                {
                    if (mNextGraph == value)
                    {
                        return;
                    }

                    if (mNextGraph != null)
                    {
                        mNextGraph.NResetPrevGraph(this);
                    }

                    mNextGraph = value;

                    if (mNextGraph != null)
                    {
                        mNextGraph.NSetPrevGraph(this);
                    }
                }
            }
        }

        public IList<BaseCGNode> Children
        {
            get
            {
                return mChildren;
            }
        }

        /// <inheritdoc/>
        public override IList<ICGNode> ChildrenNodes => mChildren.Cast<ICGNode>().ToList();

        public void AddChild(BaseCGNode child)
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

        internal void NAddChild(BaseCGNode child)
        {
            if (!mChildren.Contains(child))
            {
                mChildren.Add(child);
            }
        }

        public void RemoveChild(BaseCGNode child)
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

        internal void NRemoveChild(BaseCGNode child)
        {
            if (mChildren.Contains(child))
            {
                mChildren.Remove(child);
            }
        }

        /// <inheritdoc/>
        public override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.Append(base.PropertiesToString(n));

            sb.PrintShortObjListProp(n, nameof(Children), Children);

            sb.PrintBriefObjProp(n, nameof(PrevGraph), PrevGraph);

            sb.PrintBriefObjProp(n, nameof(NextGraph), NextGraph);

            return sb.ToString();
        }
    }
}
