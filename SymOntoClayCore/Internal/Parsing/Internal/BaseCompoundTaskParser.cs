using SymOntoClay.Core.Internal.CodeModel;
using System;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public abstract class BaseCompoundTaskParser : BaseInternalParser
    {
        protected BaseCompoundTaskParser(InternalParserContext context)
            : base(context)
        {
        }

        public BaseCompoundTask Result { get; protected set; }

        protected void ProcessGeneralContent()
        {
#if DEBUG
            //Info("100904DC-698C-45AA-A704-B268A9712F01", $"62DA50CF-1985-4CD1-BDF4-5E654AD4C6BD _currToken = {_currToken}");
#endif

            switch (_currToken.TokenKind)
            {
                case TokenKind.CloseFigureBracket:
                    Exit();
                    break;

                case TokenKind.Word:
                    switch (_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.Case:
                            {
                                _context.Recovery(_currToken);

                                var parser = new CompoundTaskCaseParser(_context);
                                parser.Run();

                                Result.Cases.Add(parser.Result);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new UnexpectedTokenException(_currToken);
            }
        }
    }
}
