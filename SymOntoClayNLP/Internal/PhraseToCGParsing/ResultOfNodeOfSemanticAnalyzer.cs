using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.CG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseToCGParsing
{
    public class ResultOfNodeOfSemanticAnalyzer : IObjectToString
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
            var nextN = n + 4;
            var sb = new StringBuilder();
            if (RootConcept == null)
            {
                sb.AppendLine($"{spaces}{nameof(RootConcept)} = null");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(RootConcept)}");
                sb.Append(RootConcept.ToBriefString(nextN));
                sb.AppendLine($"{spaces}End {nameof(RootConcept)}");
            }
            if (PrimaryRolesDict == null)
            {
                sb.AppendLine($"{spaces}{nameof(PrimaryRolesDict)} = null");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(PrimaryRolesDict)}");
                sb.Append(PrimaryRolesDict.ToString(nextN));
                sb.AppendLine($"{spaces}End {nameof(PrimaryRolesDict)}");
            }
            if (SecondaryRolesDict == null)
            {
                sb.AppendLine($"{spaces}{nameof(SecondaryRolesDict)} = null");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(SecondaryRolesDict)}");
                sb.Append(SecondaryRolesDict.ToString(nextN));
                sb.AppendLine($"{spaces}End {nameof(SecondaryRolesDict)}");
            }
            return sb.ToString();
        }
    }
}
