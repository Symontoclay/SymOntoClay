using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class LogicalExecutableExpression: BaseExecutableExpression
    {
        private static List<StrongIdentifierValue> _builtInSuperTypes = new List<StrongIdentifierValue> 
        { 
            NameHelper.CreateName(StandardNamesConstants.FuzzyTypeName),
            NameHelper.CreateName(StandardNamesConstants.BooleanTypeName) 
        };

        private LogicalExecutableExpression()
            : base()
        {
        }

        public LogicalExecutableExpression(AstExpression expression, CompiledFunctionBody compiledFunctionBody)
            : base(expression, _builtInSuperTypes, compiledFunctionBody)
        {
        }

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
            if (context.ContainsKey(this))
            {
                return (LogicalExecutableExpression)context[this];
            }

            var result = new LogicalExecutableExpression();
            context[this] = result;

            result.AppendExecutableExpression(this, context);

            return result;
        }
    }
}
