using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class RelationDescriptionParametersParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForParameter,
            GotParameterName,
            GotParameterAnnotation
        }

        public RelationDescriptionParametersParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public List<RelationParameterDescription> Result { get; set; } = new List<RelationParameterDescription>();
        private RelationParameterDescription _curentArgumentInfo;

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result.WriteListToString()}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            _state = State.WaitForParameter;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForParameter:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.LogicalVar:
                            {
                                _curentArgumentInfo = new RelationParameterDescription();
                                Result.Add(_curentArgumentInfo);

                                _curentArgumentInfo.Name = ParseName(_currToken.Content);

                                _state = State.GotParameterName;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotParameterName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenAnnotationBracket:
                            {
                                _context.Recovery(_currToken);
                                var parser = new AnnotationParser(_context);
                                parser.Run();

                                var annotation = parser.Result;

                                var meaningRolesList = annotation.MeaningRolesList;

#if DEBUG
                                //Log($"meaningRolesList = {meaningRolesList.WriteListToString()}");
#endif

                                if(meaningRolesList.IsNullOrEmpty())
                                {
                                    throw new NotImplementedException();
                                }

                                _curentArgumentInfo.MeaningRolesList.AddRange(meaningRolesList);

                                _state = State.GotParameterAnnotation;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotParameterAnnotation:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Comma:
                            _state = State.WaitForParameter;
                            break;

                        case TokenKind.CloseRoundBracket:
                            Exit();
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
