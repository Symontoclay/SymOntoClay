using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class ActionInstanceValue: Value, IExecutable
    {
        public ActionInstanceValue(ActionInstance actionInstance)
        {
            ActionInstance = actionInstance;
        }

        public ActionInstance ActionInstance { get; private set; }
    }
}
