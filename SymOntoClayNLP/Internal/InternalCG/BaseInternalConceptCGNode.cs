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
