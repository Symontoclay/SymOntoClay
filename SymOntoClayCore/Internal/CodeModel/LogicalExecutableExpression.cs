using System;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class LogicalExecutableExpression: BaseExecutableExpression
    {
        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <inheritdoc/>
        public override BaseExecutableExpression CloneExecutableExpression(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public LogicalExecutableExpression Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public LogicalExecutableExpression Clone(Dictionary<object, object> context)
        {
            throw new NotImplementedException("4AFEDB4F-4925-4FA9-9775-EA60779BD072");
        }
    }
}
