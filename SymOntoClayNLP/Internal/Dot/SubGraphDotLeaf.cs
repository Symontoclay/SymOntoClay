using SymOntoClay.NLP.Internal.CG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.Dot
{
    public class SubGraphDotLeaf : BaseContainerLeaf
    {
        public SubGraphDotLeaf(DotContext context, ICGNode node)
            : base(context, node)
        {
        }

        protected override void PringBegin()
        {
            Sb.Append("subgraph ");
            Sb.Append(Name);
            Sb.AppendLine("{");
        }
    }
}
