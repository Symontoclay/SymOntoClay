using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class PropertiesNode : BaseNode
    {
        public PropertiesNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(List<Property> properties)
        {
            foreach (var item in properties)
            {

            }

#if DEBUG
            DbgPrintCommands();
#endif

            throw new NotImplementedException("B31892E2-8D57-49F8-B60D-6530CEE4F5F1");
        }
    }
}
