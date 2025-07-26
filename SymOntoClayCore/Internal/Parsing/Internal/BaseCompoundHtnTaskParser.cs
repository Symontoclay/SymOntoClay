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

                        case KeyWordTokenKind.Before:
                            {
                                _context.Recovery(_currToken);

                                var parser = new CompoundHtnTaskBeforeParser(_context);
                                parser.Run();

                                Result.Before = parser.Result;
                            }
                            break;

                        case KeyWordTokenKind.After:
                            {
                                _context.Recovery(_currToken);

                                var parser = new CompoundHtnTaskAfterParser(_context);
                                parser.Run();

                                Result.After = parser.Result;
                            }
                            break;

                        case KeyWordTokenKind.Background:
                            {
                                _context.Recovery(_currToken);

                                var parser = new CompoundHtnTaskBackgroundParser(_context);
                                parser.Run();

                                var parsingResult = parser.Result;

#if DEBUG
                                //Info("62D56D5A-937D-4241-9B02-4AF70948401F", $"parsingResult = {parsingResult}");
                                //throw new NotImplementedException("7FD4F4FA-ADCF-4611-B9CE-0153096B325D");
#endif

                                parsingResult.Holder = Result.Name;

                                Result.Backgrounds.Add(parsingResult);
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

            var defaultCases = cases.Where(p => p.Condition == null);

            var defaultCasesCount = defaultCases.Count();

#if DEBUG
            //Info("26DB4C8B-9BE4-415D-BE1F-5C6CE0252DB0", $"defaultCasesCount = {defaultCasesCount}");
#endif

            if(defaultCasesCount > 1)
            {
                throw new DuplicatedCompoundHtnTaskCaseException(Result.Name.ToHumanizedLabel(), defaultCases.First().ToHumanizedLabel(), defaultCasesCount);
            }

            Result.Cases = cases.OrderBy(p => p.Condition == null).ToList();
        }
    }
}
