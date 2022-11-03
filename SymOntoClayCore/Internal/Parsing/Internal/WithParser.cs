using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class WithParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForItem,
            GotItemName,
            WaitForItemValue,
            GotItemValue
        }

        public WithParser(CodeItem codeItem, InternalParserContext context, params TerminationToken[] terminators)
            : base(context, terminators)
        {
            _codeItem = codeItem;
        }

        private readonly CodeItem _codeItem;
        private State _state = State.Init;
        private StrongIdentifierValue _itemName;

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
            //Log($"_itemName = {_itemName}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.With:
                                    _state = State.WaitForItem;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForItem:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            _itemName = NameHelper.CreateName(_currToken.Content);
                            _state = State.GotItemName;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotItemName:
                    if (IsValueToken())
                    {
                        ProcessItemValue();
                        break;
                    }
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Assign:
                            _state = State.WaitForItemValue;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForItemValue:
                    if(IsValueToken())
                    {
                        ProcessItemValue();
                        break;
                    }
                    throw new UnexpectedTokenException(_currToken);

                case State.GotItemValue:
                    switch (_currToken.TokenKind)
                    {
                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ProcessItemValue()
        {
            var value = ParseValue();

#if DEBUG
            //Log($"value = {value}");
#endif

            var nameStr = _itemName?.NormalizedNameValue;

#if DEBUG
            //Log($"nameStr = {nameStr}");
#endif

            if (string.IsNullOrWhiteSpace(nameStr))
            {
                throw new Exception("Name of setting of code item can not be null or empty.");
            }

            switch (nameStr)
            {
                case "priority":
                    _codeItem.Priority = value;
                    break;

                default:
                    throw new Exception($"Unexpected name `{nameStr}` of setting of code item.");
            }

            _state = State.GotItemValue;
        }
    }
}
