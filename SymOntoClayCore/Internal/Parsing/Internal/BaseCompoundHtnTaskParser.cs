using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Linq;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public abstract class BaseCompoundHtnTaskParser : BaseInternalParser
    {
        protected BaseCompoundHtnTaskParser(InternalParserContext context)
            : base(context)
        {
        }

        public BaseCompoundHtnTask Result { get; protected set; }

        protected void ProcessGeneralContent()
        {
#if DEBUG
            //Info("100904DC-698C-45AA-A704-B268A9712F01", $"_currToken = {_currToken}");
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

                                var parser = new CompoundHtnTaskCaseParser(_context);
                                parser.Run();

                                Result.Cases.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.Prop:
                            {
                                _context.Recovery(_currToken);

                                var parser = new PropertyParser(_context);
                                parser.Run();

                                Result.SubItems.Add(parser.Result);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                default:
                    throw new UnexpectedTokenException(Text, _currToken);
            }
        }

        protected virtual void Validate()
        {
            ValidateCases();
        }

        private void ValidateCases()
        {
            var cases = Result.Cases;

            if(cases.IsNullOrEmpty())
            {
                return;
            }

            var defaultCasesCount = cases.Count(p => p.Condition == null);

#if DEBUG
            Info("26DB4C8B-9BE4-415D-BE1F-5C6CE0252DB0", $"defaultCasesCount = {defaultCasesCount}");
#endif

            if(defaultCasesCount > 1)
            {
                throw new NotImplementedException();
            }
        }
    }
}
