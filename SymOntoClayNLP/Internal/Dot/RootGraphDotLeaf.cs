using SymOntoClay.NLP.Internal.CG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.Dot
{
    public class RootGraphDotLeaf : BaseContainerLeaf
    {
        public RootGraphDotLeaf(DotContext context, ICGNode node)
            : base(context, node)
        {
        }

        protected override void PringBegin()
        {
            Sb.Append("digraph ");
            Sb.Append(Name);
            Sb.AppendLine("{");
            Sb.AppendLine("compound=true;");
        }
    }
}
