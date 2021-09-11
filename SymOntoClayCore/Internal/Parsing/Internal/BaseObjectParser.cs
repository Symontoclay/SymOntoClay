using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class BaseObjectParser : BaseInternalParser
    {
        protected BaseObjectParser(InternalParserContext context, KindOfCodeEntity kindOfCodeEntity)
            : base(context)
        {
            _kindOfCodeEntity = kindOfCodeEntity;
        }

        private readonly KindOfCodeEntity _kindOfCodeEntity;

        public CodeEntity Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
#if DEBUG
            //Log("Begin");
#endif
            Result = CreateCodeEntityAndSetAsCurrent(_kindOfCodeEntity);

#if DEBUG
            //Log("End");
#endif
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
#if DEBUG
            //Log("Begin");
#endif

            RemoveCurrentCodeEntity();

#if DEBUG
            //Log("End");
#endif
        }

        protected void ProcessGeneralContent()
        {
            switch (_currToken.TokenKind)
            {
                case TokenKind.CloseFigureBracket:
                    Exit();
                    break;

                case TokenKind.Word:
                    switch (_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.On:
                            {
                                _context.Recovery(_currToken);
                                var parser = new InlineTriggerParser(_context);
                                parser.Run();
                                Result.SubItems.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.Fun:
                            {
                                _context.Recovery(_currToken);
                                var parser = new NamedFunctionParser(_context);
                                parser.Run();
                                Result.SubItems.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.Operator:
                            {
                                _context.Recovery(_currToken);
                                var parser = new OperatorParser(_context);
                                parser.Run();
                                Result.SubItems.Add(parser.Result);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case TokenKind.OpenFactBracket:
                    {
                        _context.Recovery(_currToken);
                        var parser = new LogicalQueryAsCodeEntityParser(_context);
                        parser.Run();
                        Result.SubItems.Add(parser.Result);
                    }
                    break;

                default:
                    throw new UnexpectedTokenException(_currToken);
            }
        }
    }
}
