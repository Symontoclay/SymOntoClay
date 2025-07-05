using System;

namespace SymOntoClay.Core.Internal.Parsing
{
    public class DuplicatedCompoundHtnTaskCaseException: Exception
    {
        public DuplicatedCompoundHtnTaskCaseException(string htnTaskName, string caseLabel, int count)
            : base($"'{htnTaskName}': The case '{caseLabel}' has been declared {count} times.")
        {
        }
    }
}
