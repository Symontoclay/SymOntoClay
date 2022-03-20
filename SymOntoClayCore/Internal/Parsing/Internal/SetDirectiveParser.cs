using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class SetDirectiveParser : BaseInternalParser
    {
        public SetDirectiveParser(InternalParserContext context)
            : base(context)
        {
        }

        public List<CodeItemDirective> Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new List<CodeItemDirective>();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_currToken = {_currToken}");
#endif
            _context.Recovery(_currToken);
            var parser = new SetStatementParser(_context);
            parser.Run();

            var result = parser.Result;

#if DEBUG
            Log($"result = {result}");
#endif

            var kindOfStatement = result.Kind;

            switch (kindOfStatement)
            {
                case KindOfAstStatement.SetDefaultState:
                    {
                        var item = new SetDefaultStateDirective();
                        item.StateName = (result as AstSetDefaultStateStatement).StateName;
                        Result.Add(item);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfStatement), kindOfStatement, null);
            }

            Exit();
        }        
    }
}
