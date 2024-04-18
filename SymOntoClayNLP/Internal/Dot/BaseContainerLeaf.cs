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

using SymOntoClay.NLP.Internal.CG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.Dot
{
    public abstract class BaseContainerLeaf : BaseLeaf
    {
        protected BaseContainerLeaf(DotContext context, ICGNode node)
            : base(context)
        {
            mNode = node;
            Name = Context.GetClusterName();
            Context.RegLeaf(mNode, this);
        }

        public override bool IsContainer
        {
            get
            {
                return true;
            }
        }

        private ICGNode mNode;

        public ICGNode Node
        {
            get
            {
                return mNode;
            }
        }

        protected virtual void PringBegin()
        {
            Sb.Append("digraph ");
            Sb.Append(Name);
            Sb.AppendLine("{");
            Sb.AppendLine("compound=true;");
        }

        protected virtual void PrintEnd()
        {
            Sb.AppendLine("}");
        }

        protected override void OnRun()
        {
            FillChildren();

            PringBegin();

            ProcessConcepts();
            ProcessRelations();
            ProcessSubGraphs();

            ProcessLinks();

            PrintEnd();
        }

        private List<BaseContainerLeaf> mSubGraphs = null;

        private List<ConceptualLeaf> mConceptions = null;

        private List<RelationLeaf> mRelations = null;

        private BaseLeaf mSomeChildLeaf = null;

        public override BaseLeaf SomeChildLeaf
        {
            get
            {
                return mSomeChildLeaf;
            }
        }

        protected virtual void FillChildren()
        {
            mSubGraphs = new List<BaseContainerLeaf>();

            mConceptions = new List<ConceptualLeaf>();

            mRelations = new List<RelationLeaf>();

            var tmpChilNodes = new List<BaseLeaf>();

            foreach (var node in Node.ChildrenNodes)
            {
                var tmpNode = Context.CreateLeaf(node);

                tmpChilNodes.Add(tmpNode);
            }

            mSomeChildLeaf = tmpChilNodes.FirstOrDefault();

            mSubGraphs = tmpChilNodes.Where(p => p is BaseContainerLeaf).Select(p => p as BaseContainerLeaf).ToList();

            mConceptions = tmpChilNodes.Where(p => p is ConceptualLeaf).Select(p => p as ConceptualLeaf).ToList();

            mRelations = tmpChilNodes.Where(p => p is RelationLeaf).Select(p => p as RelationLeaf).ToList();
        }

        protected virtual void ProcessConcepts()
        {
            foreach (var leaf in mConceptions)
            {
                leaf.Run();

                Sb.AppendLine(leaf.Text);
            }
        }

        protected virtual void ProcessRelations()
        {
            foreach (var leaf in mRelations)
            {
                leaf.Run();

                Sb.AppendLine(leaf.Text);
            }
        }

        protected virtual void ProcessSubGraphs()
        {
            foreach (var leaf in mSubGraphs)
            {
                leaf.Run();

                Sb.AppendLine(leaf.Text);
            }
        }

        protected virtual void ProcessLinks()
        {
            var tmpOutPutLists = Node.ChildrenNodes.Where(p => p.OutputNodes.Count > 0).ToList();

            foreach (var node in tmpOutPutLists)
            {
                var tmpLeaf = Context.GetLeaf(node);

                foreach (var second in node.OutputNodes)
                {
                    var tmpSecondLeaf = Context.GetLeaf(second);

                    ProcessLink(tmpLeaf, tmpSecondLeaf);
                }
            }
        }

        protected virtual void ProcessLink(BaseLeaf begin, BaseLeaf end)
        {
            if (begin.IsContainer)
            {
                if (begin.SomeChildLeaf == null)
                {
                    return;
                }
                Sb.Append(begin.SomeChildLeaf.Name);
            }
            else
            {
                Sb.Append(begin.Name);
            }

            Sb.Append(" -> ");

            if (end.IsContainer)
            {
                if (end.SomeChildLeaf == null)
                {
                    return;
                }
                Sb.Append(end.SomeChildLeaf.Name);
            }
            else
            {
                Sb.Append(end.Name);
            }

            if (begin.IsContainer || end.IsContainer)
            {
                Sb.Append("[");

                if (begin.IsContainer)
                {
                    Sb.Append("ltail=");
                    Sb.Append(begin.Name);
                    Sb.Append(",");
                }

                if (end.IsContainer)
                {
                    Sb.Append("lhead=");
                    Sb.Append(end.Name);
                }

                Sb.Append("]");
            }

            Sb.AppendLine(";");
        }
    }
}
