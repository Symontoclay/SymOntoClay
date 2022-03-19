using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class CodeItemDirective: AnnotatedItem
    {
        public abstract KindOfCodeItemDirective KindOfCodeItemDirective { get; }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public CodeItemDirective CloneCodeItemDirective()
        {
            var context = new Dictionary<object, object>();
            return CloneCodeItemDirective(context);
        }


    }
}
