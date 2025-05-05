using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class StrongIdentifierValueParser : BaseInternalParserCore
    {
        private enum State
        {
            Init
        }

        public StrongIdentifierValueParser(InternalParserCoreContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        private List<StrongIdentifierPart> _items;
        private List<StrongIdentifierPart> _currentItemsList;
        private StrongIdentifierPart _currentItem;
        private Stack<List<StrongIdentifierPart>> _itemsListStack = new Stack<List<StrongIdentifierPart>>();

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            base.OnEnter();

            _items = new List<StrongIdentifierPart>();
            _currentItemsList = _items;
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            base.OnFinish();

#if DEBUG
            Info("1B5ACEB2-D3E6-4E0A-8341-53BF158773C4", $"_items = {_items.WriteListToString()}");
#endif

            if(_currentItemsList != _items)
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Info("E11076A3-E467-41F3-A228-B95D0F17FB5F", $"_state = {_state}");
            //Info("60F3CC3D-9E06-4F5C-8E0C-1C92DFF8A226", $"_currToken = {_currToken}");
            //Info("120DA08F-7326-4AB0-A1FB-EBF9DDB85631", $"_items = {_items.WriteListToString()}");
            //Info("55696EE0-358F-4F13-ABE9-47278918A598", $"_currentItemsList = {_currentItemsList.WriteListToString()}");
            //Info("0E8EB277-F964-4E68-B02A-DD6C12AE195B", $"_currentItem = {_currentItem}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            {
                                var item = new StrongIdentifierPart
                                {
                                    Token = _currToken,
                                    StrongIdentifierLevel = ParseStrongIdentifierLevel(_currToken.KeyWordTokenKind)
                                };

                                AddItem(item);
                            }
                            break;

                        case TokenKind.DoubleColon:
                        case TokenKind.Gravis:
                        case TokenKind.IdentifierPrefix:
                        case TokenKind.EntityConditionPrefix:
                        case TokenKind.ConceptPrefix:
                        case TokenKind.OnceResolvedEntityConditionPrefix:
                        case TokenKind.RuleOrFactIdentifierPrefix:
                        case TokenKind.LinguisticVarPrefix:
                        case TokenKind.LogicalVarPrefix:
                        case TokenKind.VarPrefix:
                        case TokenKind.SystemVarPrefix:
                        case TokenKind.ChannelVarPrefix:
                        case TokenKind.PropertyPrefix:
                            {
                                var item = new StrongIdentifierPart
                                {
                                    Token = _currToken
                                };

                                AddItem(item);
                            }
                            break;

                        case TokenKind.OpenRoundBracket:
                            StartParsingSubParts();
                            break;

                        case TokenKind.CloseRoundBracket:
                            StopParsingSubParts();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private StrongIdentifierLevel ParseStrongIdentifierLevel(KeyWordTokenKind keyWordTokenKind)
        {
#if DEBUG
            //Info("34250D69-480C-42E6-B335-9578BF8E402D", $"keyWordTokenKind = {keyWordTokenKind}");
#endif

            switch (keyWordTokenKind)
            {
                case KeyWordTokenKind.Global:
                    return StrongIdentifierLevel.Global;

                case KeyWordTokenKind.Root:
                    return StrongIdentifierLevel.Root;

                case KeyWordTokenKind.Strategic:
                    return StrongIdentifierLevel.Strategic;

                case KeyWordTokenKind.Tactical:
                    return StrongIdentifierLevel.Tactical;

                default:
                    return StrongIdentifierLevel.None;
            }
        }

        private void AddItem(StrongIdentifierPart item)
        {
#if DEBUG
            //Info("3463403E-245A-4E33-95BA-D0EBEF267B41", $"item = {item}");
#endif

            _currentItem = item;
            _currentItemsList.Add(item);
        }

        private void StartParsingSubParts()
        {
            _itemsListStack.Push(_currentItemsList);
            _currentItemsList = _currentItem.SubParts;
        }

        private void StopParsingSubParts()
        {
            _currentItemsList = _itemsListStack.Pop();
        }
    }
}
