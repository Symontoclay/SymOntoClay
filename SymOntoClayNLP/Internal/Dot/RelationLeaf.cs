using SymOntoClay.NLP.Internal.CG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.Dot
{
    public class RelationLeaf : BaseLeaf
    {
        public RelationLeaf(DotContext context, ICGNode node)
             : base(context)
        {
            mNode = node;
            Name = Context.GetNodeName();
            Context.RegLeaf(mNode, this);
        }

        private ICGNode mNode;

        public ICGNode Node
        {
            get
            {
                return mNode;
            }
        }

        protected override void OnRun()
        {
            Sb.Append(Name);
            Sb.Append("[shape=ellipse,label=\"");
            Sb.Append(Node.Name);
            Sb.Append("\"];");
        }
    }
}
