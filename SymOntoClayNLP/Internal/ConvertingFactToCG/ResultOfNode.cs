using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.PhraseToCGParsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToCG
{
    public class ResultOfNode : IObjectToString
    {
        public ConceptCGNode RootConcept { get; set; }
        public RolesStorageOfSemanticAnalyzer PrimaryRolesDict { get; set; } = new RolesStorageOfSemanticAnalyzer();
        public RolesStorageOfSemanticAnalyzer SecondaryRolesDict { get; set; } = new RolesStorageOfSemanticAnalyzer();

        public override string ToString()
        {
            return ToString(0u);
        }

        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        public string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(RootConcept), RootConcept);

            sb.PrintObjProp(n, nameof(PrimaryRolesDict), PrimaryRolesDict);

            sb.PrintObjProp(n, nameof(SecondaryRolesDict), SecondaryRolesDict);

            return sb.ToString();
        }
    }
}
