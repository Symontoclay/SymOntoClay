using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class HostPaser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotHost,
            GotName,
            ContentStarted
        }

        public HostPaser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public CodeEntity Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
#if DEBUG
            //Log("Begin");
#endif
            Result = CreateCodeEntity();

            Result.Kind = KindOfCodeEntity.Host;
            Result.CodeFile = _context.CodeFile;

            Result.ParentCodeEntity = CurrentCodeEntity;
            SetCurrentCodeEntity(Result);

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

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");
            Log($"_state = {_state}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.Host:
                            _state = State.GotHost;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotHost:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            Result.Name = ParseName(_currToken.Content);
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
                                    {
                                        _context.Recovery(_currToken);
                                        var parser = new InheritanceParser(_context, Result.Name);
                                        parser.Run();
                                        Result.InheritanceItems.AddRange(parser.Result);
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

                case State.ContentStarted:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseFigureBracket:
                            Exit();
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
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }
    }
}
