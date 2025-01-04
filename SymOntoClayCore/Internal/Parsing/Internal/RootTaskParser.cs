using SymOntoClay.Core.Internal.CodeModel;
using System;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class RootTaskParser : BaseCompoundTaskParser
    {
        private enum State
        {
            Init,
            GotRoot,
            GotRootTask,
            GotName,
            ContentStarted
        }

        public RootTaskParser(InternalParserContext context)
            : base(context)
        {
        }

        private RootTask _rootTask;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _rootTask = new RootTask();
            Result = _rootTask;

            SetCurrentCodeItem(Result);
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            RemoveCurrentCodeEntity();
        }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Info("1AC9590B-BB19-4713-ABCE-DCDD96804F4B", $"_state = {_state}");
            //Info("99BB0ECA-2A70-472C-AC0B-AF6F266F2D03", $"_currToken = {_currToken}");
            //Info(, $"Result = {Result}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Root:
                                    _state = State.GotRoot;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotRoot:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Task:
                                    _state = State.GotRootTask;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotRootTask:
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
    }
}
