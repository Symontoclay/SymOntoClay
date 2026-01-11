/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using Newtonsoft.Json.Linq;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class AnnotationParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForItem,
            GotSettingsKey,
            WaitForSettingsValue,
            GotItem
        }

        public AnnotationParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public Annotation Result { get; private set; } = new Annotation();
        private Dictionary<StrongIdentifierValue, Value> _settingsDict;
        private List<Value> _meaningRolesList;
        private StrongIdentifierValue _settingsKey;
        private StrongIdentifierValue _lastSettingsKey;
        private StrongIdentifierValue _lastMeaningRole;

        protected override void OnEnter()
        {
            _meaningRolesList = Result.MeaningRolesList;
            _settingsDict = Result.SettingsDict;
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            Result.CheckDirty();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenAnnotationBracket:
                            _state = State.WaitForItem;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.WaitForItem:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            ProcessWaitForItem();
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotSettingsKey:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Assign:
                            _state = State.WaitForSettingsValue;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.WaitForSettingsValue:
                    {
                        var value = ParseValue();

                        _settingsDict[_settingsKey] = value;
                        _lastSettingsKey = _settingsKey;
                        _settingsKey = null;
                        _lastMeaningRole = null;

                        _state = State.GotItem;
                    }
                    break;

                case State.GotItem:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            {
                                var value = ParseValue();

                                if (_lastSettingsKey == null)
                                {
                                    if (_lastMeaningRole.IsSequenceValue)
                                    {
                                        _lastMeaningRole.AsSequenceValue.AddValue(value);
                                    }
                                    else
                                    {
                                        var sequenceValue = new SequenceValue(_lastMeaningRole);
                                        _meaningRolesList[_meaningRolesList.IndexOf(_lastMeaningRole)] = sequenceValue;
                                        sequenceValue.AddValue(value);
                                    }                                    
                                }
                                else
                                {
                                    var lastValue = _settingsDict[_lastSettingsKey];

                                    if (lastValue.IsSequenceValue)
                                    {
                                        lastValue.AsSequenceValue.AddValue(value);
                                    }
                                    else
                                    {
                                        var sequenceValue = new SequenceValue(lastValue);
                                        lastValue = sequenceValue;
                                        sequenceValue.AddValue(value);
                                        _settingsDict[_lastSettingsKey] = sequenceValue;
                                    }
                                }
                            }
                            break;

                        case TokenKind.CloseAnnotationBracket:
                            Exit();
                            break;

                        case TokenKind.Comma:
                            _lastSettingsKey = null;
                            _lastMeaningRole = null;
                            _state = State.WaitForItem;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, $"In `{Text}`.");
            }
        }

        private void ProcessWaitForItem()
        {
            var nextToken = _context.GetToken();
            _context.Recovery(nextToken);

            switch(nextToken.TokenKind)
            {
                case TokenKind.Word:
                    switch(nextToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.Complete:
                        case KeyWordTokenKind.Completed:
                            {
                                if(Result.AnnotationSystemEventsDict.ContainsKey(KindOfAnnotationSystemEvent.Complete))
                                {
                                    throw new UnexpectedTokenException(Text, _currToken, "Event `Complete` already exists.");
                                }

                                _context.Recovery(_currToken);

                                var parser = new AnnotationSystemEventParser(_context);
                                parser.Run();

                                Result.AnnotationSystemEventsDict[KindOfAnnotationSystemEvent.Complete] = parser.Result;

                                _state = State.GotItem;
                            }
                            return;

                        case KeyWordTokenKind.Weak:
                            {
                                if (Result.AnnotationSystemEventsDict.ContainsKey(KindOfAnnotationSystemEvent.WeakCancel))
                                {
                                    throw new UnexpectedTokenException(Text, _currToken, "Event `Weak cancel` already exists.");
                                }

                                _context.Recovery(_currToken);

                                var parser = new AnnotationSystemEventParser(_context);
                                parser.Run();

                                Result.AnnotationSystemEventsDict[KindOfAnnotationSystemEvent.WeakCancel] = parser.Result;

                                _state = State.GotItem;
                            }
                            return;

                        default:
                            break;
                    }
                    break;

                case TokenKind.Assign:
                    _settingsKey = ParseName(_currToken.Content);
                    _state = State.GotSettingsKey;
                    return;

                default:
                    break;
            }

            _lastMeaningRole = ParseName(_currToken.Content);
            _meaningRolesList.Add(_lastMeaningRole);
            _state = State.GotItem;
        }
    }
}
