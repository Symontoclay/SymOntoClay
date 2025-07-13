using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public abstract class BaseCompoundHtnTaskItemsSectionParser : BaseInternalParser
    {
        protected BaseCompoundHtnTaskItemsSectionParser(InternalParserContext context)
            : base(context)
        {
        }

        protected void RegisterResult(CompoundHtnTaskItemsSection result)
        {
            _result = result;
        }

        private CompoundHtnTaskItemsSection _result;

        protected void ParseCompoundHtnTaskItemsSectionContent()
        {
            switch (_currToken.TokenKind)
            {
                case TokenKind.CloseFigureBracket:
                    Exit();
                    break;

                case TokenKind.Word:
                    {
                        _context.Recovery(_currToken);

                        var parser = new CompoundHtnTaskCaseItemParser(_context);
                        parser.Run();

                        _result.Items.Add(parser.Result);
                    }
                    break;

                default:
                    throw new UnexpectedTokenException(Text, _currToken);
            }
        }
    }
}
