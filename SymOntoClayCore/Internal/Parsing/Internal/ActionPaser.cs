using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class ActionPaser : BaseObjectParser
    {
        private enum State
        {
            Init,
            GotActionMark,
            GotName,
            GotInheritance,
            GotAlias,
            ContentStarted
        }

        public ActionPaser(InternalParserContext context)
            : base(context, KindOfCodeEntity.Action)
        {
        }

        private State _state = State.Init;

        private ActionDef _action;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            base.OnEnter();

            _action = CreateAction();
            _action.CodeEntity = Result;

            Result.Action = _action;
        }

        /// <inheritdoc/>
        protected override void OnAddOperator(Operator op, CodeEntity codeEntity)
        {
            base.OnAddOperator(op, codeEntity);

#if DEBUG
            //Log($"op = {op}");
#endif

            if(op.KindOfOperator == KindOfOperator.CallFunction)
            {
                _action.AddOperator(op);
            }
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");            
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.Action:
                            _state = State.GotActionMark;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotActionMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            var name = ParseName(_currToken.Content);

                            Result.Name = name;

                            _action.Name = name;

                            _state = State.GotName;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _state = State.ContentStarted;
                            break;

                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Is:
                                    ProcessInheritance();
                                    break;

                                case KeyWordTokenKind.Alias:
                                    {
                                        _context.Recovery(_currToken);

                                        var parser = new AliasPaser(_context);
                                        parser.Run();

                                        _action.AddAliasRange(parser.Result);

                                        _state = State.GotAlias;
                                    }
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotAlias:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _state = State.ContentStarted;
                            break;

                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Is:
                                    ProcessInheritance();
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotInheritance:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _state = State.ContentStarted;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.ContentStarted:
                    ProcessGeneralContent();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ProcessInheritance()
        {
            _context.Recovery(_currToken);
            var parser = new InheritanceParser(_context, Result.Name);
            parser.Run();
            Result.InheritanceItems.AddRange(parser.Result);

            _state = State.GotInheritance;
        }
    }
}
