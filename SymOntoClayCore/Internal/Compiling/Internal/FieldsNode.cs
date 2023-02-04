using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class FieldsNode : BaseNode
    {
        public FieldsNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(List<Field> fields)
        {
#if DEBUG
            //Log($"fields = {fields.WriteListToString()}");
#endif

            foreach(var field in fields)
            {
#if DEBUG
                Log($"field = {field}");
#endif

                CompileVarDecl(field);

#if DEBUG
                DbgPrintCommands();
#endif
            }

            throw new NotImplementedException();
        }
    }
}
