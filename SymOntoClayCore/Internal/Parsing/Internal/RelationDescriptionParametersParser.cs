/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;

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
            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            _state = State.WaitForParameter;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
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
                            throw new UnexpectedTokenException(Text, _currToken);
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

                                if(meaningRolesList.IsNullOrEmpty())
                                {
                                    throw new NotImplementedException("4900187D-B5AF-45FC-AA67-3ED6A9FCB4F6");
                                }

                                _curentArgumentInfo.MeaningRolesList.AddRange(meaningRolesList);

                                _state = State.GotParameterAnnotation;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
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
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, $"In `{Text}`.");
            }
        }
    }
}
