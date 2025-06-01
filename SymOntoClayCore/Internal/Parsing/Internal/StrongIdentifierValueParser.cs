using Newtonsoft.Json;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class StrongIdentifierValueParser : BaseInternalParserCore
    {
        private enum State
        {
            Init,
            InCapacity
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

        public StrongIdentifierValue Result { get; private set; }

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
            //Info("1B5ACEB2-D3E6-4E0A-8341-53BF158773C4", $"_items = {_items.WriteListToString()}");
#endif

            if(_currentItemsList != _items)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"9BA9A4D2-2A42-446D-BC57-2AE6A720952A: Invalid sting. Please check closing brackets.");
                sb.Append($"in `{Text}`.");

                throw new Exception(sb.ToString());
            }

            if(_items.Count == 0)
            {
                Result = BuildEmptyStrongIdentifierValue();
            }
            else
            {
                var resultsList = PostProcessParts(_items, true);

#if DEBUG
                //Info("290F1D9A-3BE7-49BD-9AAE-3FC2B104B691", $"resultsList.Count = {resultsList.Count}");
                //Info("B6AAE834-1E66-47B5-848A-13B63E74745F", $"resultsList = {resultsList.WriteListToString()}");
#endif

                if (resultsList.Count > 1)
                {
                    throw new NotImplementedException($"D3EE3F0E-17CB-4F08-9EEC-CE10E6A574B3 in `{Text}`.");
                }

                Result = resultsList.Single();
            }

            //Result = PostProcessDoubleColonParts(_items, true);

#if DEBUG
            //Info("806612EB-DB86-4C60-BB81-F3A261E876E6", $"Result = {Result}");
#endif
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
                        case TokenKind.Or:
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
                        case TokenKind.Number:
                        case TokenKind.Minus:
                            {
                                var item = new StrongIdentifierPart
                                {
                                    Token = _currToken
                                };

                                AddItem(item);
                            }
                            break;

                        case TokenKind.OpenRoundBracket:
                            if(_currentItem == null)
                            {
                                var item = new StrongIdentifierPart();

                                AddItem(item);
                            }
                            StartParsingSubParts();
                            break;

                        case TokenKind.CloseRoundBracket:
                            StopParsingSubParts();
                            break;

                        case TokenKind.OpenSquareBracket:
                            _state = State.InCapacity;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.InCapacity:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            if(_currentItem.HasInfiniteCapacity || _currentItem.Capacity.HasValue)
                            {
                                throw new UnexpectedTokenException(_currToken);
                            }
                            else
                            {
                                CoreContext.Recovery(_currToken);
                                var parser = NumberParser.CreateFromInternalParserCoreContext(CoreContext);
                                parser.Run();

                                _currentItem.Capacity = Convert.ToInt32(parser.Result.GetSystemValue());

#if DEBUG
                                //Info("58C32FFC-C0D0-4204-ADF7-B676CF226A8B", $"parser.Result = {parser.Result}");
                                //Info("55FB64C7-3548-4C61-BAE9-2BBF0CB9540A", $"_currentItem = {_currentItem}");
#endif
                            }
                            break;

                        case TokenKind.Multiplication:
                        case TokenKind.Infinity:
                        case TokenKind.PositiveInfinity:
                            if(_currentItem.HasInfiniteCapacity || _currentItem.Capacity.HasValue)
                            {
                                throw new UnexpectedTokenException(Text, _currToken);
                            }
                            else
                            {
                                _currentItem.HasInfiniteCapacity = true;
                            }                                
                            break;

                        case TokenKind.CloseSquareBracket:
#if DEBUG
                            //Info("C0E6914E-5582-49D5-B481-D9E8D0F8B14E", $"_currentItem = {_currentItem}");
#endif

                            if(!_currentItem.Capacity.HasValue)
                            {
                                _currentItem.HasInfiniteCapacity = true;
                            }

                            _state = State.Init;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, $"In `{Text}`.");
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

        private List<StrongIdentifierValue> PostProcessParts(List<StrongIdentifierPart> items, bool isRootItem)
        {
#if DEBUG
            //Info("7844EC21-0330-47A2-98A3-BA00BA7DF1BC", $"isRootItem = {isRootItem}");
            //Info("39DDF0F0-08A7-4D88-93D3-E6F88A666659", $"items = {items.WriteListToString()}");
#endif

            var result = new List<StrongIdentifierValue>();

            var groupedItems = GroupPartsByOr(items);

#if DEBUG
            //Info("9DEA4560-3AC3-4617-9315-6E0514D0CA9D", $"groupedItems = {JsonConvert.SerializeObject(groupedItems, Formatting.Indented)}");
#endif

            foreach (var groupedItem in groupedItems)
            {
#if DEBUG
                //Info("D9533B06-3B27-4129-A3EF-D2645FE5AE47", $"groupedItem = {groupedItem.WriteListToString()}");
#endif

                var subItem = PostProcessDoubleColonParts(groupedItem, isRootItem);

#if DEBUG
                //Info("42D91583-C2D1-4651-84F6-A501F4CCC7CB", $"subItem = {subItem}");
#endif

                result.Add(subItem);
            }

#if DEBUG
            //Info("8375F3C3-1C8E-479E-8FD1-9E04AAED2F86", $"result = { result.WriteListToString()}");
#endif

            return result;
        }

        private StrongIdentifierValue PostProcessDoubleColonParts(List<StrongIdentifierPart> items, bool isRootItem)
        {
#if DEBUG
            //Info("8BEF1C26-16E1-456F-BB81-AA54ABAE63D5", $"isRootItem = {isRootItem}");
            //Info("9416E080-D9AE-4E49-92F1-E25E7378A110", $"items = {items.WriteListToString()}");
#endif

            var groupedItems = GroupPartsByDoubleColon(items);

#if DEBUG
            //Info("8297EA2B-34BA-4008-84B1-7CFF9F8A9FE0", $"groupedItems = {JsonConvert.SerializeObject(groupedItems, Formatting.Indented)}");
#endif

            if(groupedItems.Count == 1)
            {
                return BuildStrongIdentifierValue(groupedItems.Single(), StrongIdentifierLevel.None, isRootItem);
            }

            var level = StrongIdentifierLevel.None;

            var firstGroupedItem = groupedItems.First();

#if DEBUG
            //Info("7D565976-1184-4052-A475-4C4C4055155A", $"firstGroupedItem = {firstGroupedItem.WriteListToString()}");
#endif

            if(firstGroupedItem.Count == 1)
            {
                var singleItem = firstGroupedItem.Single();

#if DEBUG
                //Info("61B2CCCE-3E3D-4DD4-B17B-8E78C9EA6B21", $"singleItem = {singleItem}");
#endif

                if(singleItem.StrongIdentifierLevel != StrongIdentifierLevel.None && singleItem.SubParts.Count == 0)
                {
                    level = singleItem.StrongIdentifierLevel;
                    groupedItems.Remove(firstGroupedItem);
                }
            }

#if DEBUG
            //Info("39AEC91C-1D82-4162-AEF0-55429807CF5E", $"level = {level}");
#endif

            var lastGroupedItem = groupedItems.Last();

#if DEBUG
            //Info("179C398C-CD91-4D50-9608-23C798150C3B", $"lastGroupedItem = {lastGroupedItem.WriteListToString()}");
#endif

            groupedItems.Remove(lastGroupedItem);

            var result = BuildStrongIdentifierValue(lastGroupedItem, level, isRootItem);

#if DEBUG
            //Info("5F303670-5F25-4438-ACF8-E74463E762F8", $"result = {result}");
#endif

            if(groupedItems.Any())
            {
                var subItems = new List<StrongIdentifierValue>();

                foreach (var groupedItem in groupedItems)
                {
#if DEBUG
                    //Info("4D7769DD-88C4-4711-A892-CF9C98C8B19F", $"groupedItem = {groupedItem.WriteListToString()}");
#endif

                    var subItem = BuildStrongIdentifierValue(groupedItem, StrongIdentifierLevel.None, false);

#if DEBUG
                    //Info("B9B363FA-CC4A-4A16-B5C4-6EBE5BD42CE8", $"subItem = {subItem}");
#endif

                    if (subItem.IsEmpty)
                    {
                        if (subItem.Namespaces.Any())
                        {
                            result.Namespaces.AddRange(subItem.Namespaces);
                        }
                    }
                    else
                    {
                        subItems.Add(subItem);
                    }
                }

#if DEBUG
                //Info("AB559700-C03E-4244-8156-90D80DC82D0E", $"subItems = {subItems.WriteListToString()}");
#endif

                switch(subItems.Count)
                {
                    case 0:
                        break;

                    case 1:
                        result.Namespaces.AddRange(subItems);
                        break;

                    default:
                        {
                            subItems.Reverse();

#if DEBUG
                            //Info("70A528E2-137A-47A4-9989-3C43D80AEEB8", $"subItems (after) = {subItems.WriteListToString()}");
#endif
                            var firstItem = subItems.First();
                            var currentItem = firstItem;

                            subItems.Remove(firstItem);

                            foreach (var subItem in subItems)
                            {
#if DEBUG
                                //Info("021B2076-48DA-42EA-A8A9-7ADF0933FE5F", $"subItem = {subItem}");
#endif

                                currentItem.Namespaces.Add(subItem);
                                currentItem = subItem;
                            }

                            result.Namespaces.Add(firstItem);
                        }
                        break;
                }
            }

            return result;
        }

        private StrongIdentifierValue BuildEmptyStrongIdentifierValue()
        {
            var result = new StrongIdentifierValue() { IsEmpty = true };
            result.CheckDirty();

            return result;
        }

        private StrongIdentifierValue BuildStrongIdentifierValue(List<StrongIdentifierPart> items, StrongIdentifierLevel level, bool isRootItem)
        {
#if DEBUG
            //Info("E5B5F691-BF07-4176-A2B7-73BD6037BD2D", $"items = {items.WriteListToString()}");
            //Info("A04C90F7-95F5-4904-AE6F-AB3685F7030E", $"isRootItem = {isRootItem}");
#endif

            var nameValueSb = new StringBuilder();
            var normalizedNameValueSb = new StringBuilder();
            var kindOfName = KindOfName.CommonConcept;
            int? capacity = null;
            var hasInfiniteCapacity = false;
            var subItems = new List<StrongIdentifierValue>();

            var wasWord = false;

            foreach(var item in items)
            {
#if DEBUG
                //Info("B556484D-EA3F-4575-B70A-EE5603FC67AC", $"item = {item}");
#endif

                var token = item.Token;

                if(token == null)
                {
                    if (item.SubParts.Any())
                    {
#if DEBUG
                        //Info("5FDE1B77-0AF4-4D1E-98F1-FA2573541716", $"item.SubParts = {item.SubParts.WriteListToString()}");
#endif

                        var convertedSubItems = PostProcessParts(item.SubParts, false);

#if DEBUG
                        //Info("77B059D6-AB04-47D9-8CAD-8A0E32D2E87A", $"convertedSubItems = {convertedSubItems.WriteListToString()}");
#endif

                        subItems.AddRange(convertedSubItems);
                    }

                    //throw new NotImplementedException();

                    continue;
                }

                var tokenKind = token.TokenKind;

                switch (tokenKind)
                {
                    case TokenKind.IdentifierPrefix:
                        if (kindOfName == KindOfName.CommonConcept)
                        {
                            if (wasWord)
                            {
                                throw new Exception(BuildExceptionMessage1(nameof(TokenKind.IdentifierPrefix)));
                            }
                            else
                            {
                                kindOfName = KindOfName.Entity;
                                nameValueSb.Append(item.Token.Content);
                                normalizedNameValueSb.Append(item.Token.Content);
                            }
                        }
                        else
                        {
                            throw new Exception(BuildExceptionMessage2(nameof(TokenKind.IdentifierPrefix), kindOfName, item));
                        }
                        break;

                    case TokenKind.EntityConditionPrefix:
                        if (kindOfName == KindOfName.CommonConcept)
                        {
                            if (wasWord)
                            {
                                throw new Exception(BuildExceptionMessage1(nameof(TokenKind.EntityConditionPrefix)));
                            }
                            else
                            {
                                kindOfName = KindOfName.EntityCondition;
                                nameValueSb.Append(item.Token.Content);
                                normalizedNameValueSb.Append(item.Token.Content);
                            }
                        }
                        else
                        {
                            throw new Exception(BuildExceptionMessage2(nameof(TokenKind.EntityConditionPrefix), kindOfName, item));
                        }
                        break;

                    case TokenKind.ConceptPrefix:
                        if (kindOfName == KindOfName.CommonConcept)
                        {
                            if (wasWord)
                            {
                                throw new Exception(BuildExceptionMessage1(nameof(TokenKind.ConceptPrefix)));
                            }
                            else
                            {
                                kindOfName = KindOfName.Concept;
                                nameValueSb.Append(item.Token.Content);
                                normalizedNameValueSb.Append(item.Token.Content);
                            }
                        }
                        else
                        {
                            throw new Exception(BuildExceptionMessage2(nameof(TokenKind.ConceptPrefix), kindOfName, item));
                        }
                        break;

                    case TokenKind.OnceResolvedEntityConditionPrefix:
                        if (kindOfName == KindOfName.CommonConcept)
                        {
                            if (wasWord)
                            {
                                throw new Exception(BuildExceptionMessage1(nameof(TokenKind.OnceResolvedEntityConditionPrefix)));
                            }
                            else
                            {
                                kindOfName = KindOfName.OnceResolvedEntityCondition;
                                nameValueSb.Append(item.Token.Content);
                                normalizedNameValueSb.Append(item.Token.Content);
                            }
                        }
                        else
                        {
                            throw new Exception(BuildExceptionMessage2(nameof(TokenKind.OnceResolvedEntityConditionPrefix), kindOfName, item));
                        }
                        break;

                    case TokenKind.RuleOrFactIdentifierPrefix:
                        if (kindOfName == KindOfName.CommonConcept)
                        {
                            if (wasWord)
                            {
                                throw new Exception(BuildExceptionMessage1(nameof(TokenKind.RuleOrFactIdentifierPrefix)));
                            }
                            else
                            {
                                kindOfName = KindOfName.RuleOrFact;
                                nameValueSb.Append(item.Token.Content);
                                normalizedNameValueSb.Append(item.Token.Content);
                            }
                        }
                        else
                        {
                            throw new Exception(BuildExceptionMessage2(nameof(TokenKind.RuleOrFactIdentifierPrefix), kindOfName, item));
                        }
                        break;

                    case TokenKind.LinguisticVarPrefix:
                        if (kindOfName == KindOfName.CommonConcept)
                        {
                            if (wasWord)
                            {
                                throw new Exception(BuildExceptionMessage1(nameof(TokenKind.LinguisticVarPrefix)));
                            }
                            else
                            {
                                kindOfName = KindOfName.LinguisticVar;
                                nameValueSb.Append(item.Token.Content);
                                normalizedNameValueSb.Append(item.Token.Content);
                            }
                        }
                        else
                        {
                            throw new Exception(BuildExceptionMessage2(nameof(TokenKind.LinguisticVarPrefix), kindOfName, item));
                        }
                        break;

                    case TokenKind.LogicalVarPrefix:
                        if (kindOfName == KindOfName.CommonConcept)
                        {
                            if (wasWord)
                            {
                                throw new Exception(BuildExceptionMessage1(nameof(TokenKind.LogicalVarPrefix)));
                            }
                            else
                            {
                                kindOfName = KindOfName.LogicalVar;
                                nameValueSb.Append(item.Token.Content);
                                normalizedNameValueSb.Append(item.Token.Content);
                            }
                        }
                        else
                        {
                            throw new Exception(BuildExceptionMessage2(nameof(TokenKind.LogicalVarPrefix), kindOfName, item));
                        }
                        break;

                    case TokenKind.VarPrefix:
                        if (kindOfName == KindOfName.CommonConcept)
                        {
                            if (wasWord)
                            {
                                throw new Exception(BuildExceptionMessage1(nameof(TokenKind.VarPrefix)));
                            }
                            else
                            {
                                kindOfName = KindOfName.Var;
                                nameValueSb.Append(item.Token.Content);
                                normalizedNameValueSb.Append(item.Token.Content);
                            }
                        }
                        else
                        {
                            throw new Exception(BuildExceptionMessage2(nameof(TokenKind.VarPrefix), kindOfName, item));
                        }
                        break;

                    case TokenKind.SystemVarPrefix:
                        if (kindOfName == KindOfName.CommonConcept)
                        {
                            if (wasWord)
                            {
                                throw new Exception(BuildExceptionMessage1(nameof(TokenKind.SystemVarPrefix)));
                            }
                            else
                            {
                                kindOfName = KindOfName.SystemVar;
                                nameValueSb.Append(item.Token.Content);
                                normalizedNameValueSb.Append(item.Token.Content);
                            }
                        }
                        else
                        {
                            throw new Exception(BuildExceptionMessage2(nameof(TokenKind.SystemVarPrefix), kindOfName, item));
                        }
                        break;

                    case TokenKind.ChannelVarPrefix:
                        if (kindOfName == KindOfName.CommonConcept)
                        {
                            if (wasWord)
                            {
                                throw new Exception(BuildExceptionMessage1(nameof(TokenKind.ChannelVarPrefix)));
                            }
                            else
                            {
                                kindOfName = KindOfName.Channel;
                                nameValueSb.Append(item.Token.Content);
                                normalizedNameValueSb.Append(item.Token.Content);
                            }
                        }
                        else
                        {
                            throw new Exception(BuildExceptionMessage2(nameof(TokenKind.ChannelVarPrefix), kindOfName, item));
                        }
                        break;

                    case TokenKind.PropertyPrefix:
#if DEBUG
                        //Info("CC947963-BF42-4674-A411-C917D3D17D16", $"kindOfName = {kindOfName}");
#endif

                        if (kindOfName == KindOfName.CommonConcept)
                        {
                            if (wasWord)
                            {
                                throw new Exception(BuildExceptionMessage1(nameof(TokenKind.PropertyPrefix)));
                            }
                            else
                            {
                                kindOfName = KindOfName.Property;
                                nameValueSb.Append(item.Token.Content);
                                normalizedNameValueSb.Append(item.Token.Content);
                            }
                        }
                        else
                        {
                            throw new Exception(BuildExceptionMessage2(nameof(TokenKind.PropertyPrefix), kindOfName, item));
                        }
                        break;

                    case TokenKind.Word:
                        {
                            if (item.Capacity.HasValue)
                            {
                                capacity = item.Capacity.Value;
                            }
                            else
                            {
                                if (item.HasInfiniteCapacity)
                                {
                                    hasInfiniteCapacity = true;
                                }
                            }

                            if (item.SubParts.Any())
                            {
#if DEBUG
                                //Info("FD9B7C67-2D59-4F83-8742-EB9F6AE30332", $"item.SubParts = {item.SubParts.WriteListToString()}");
#endif

                                var convertedSubItems = PostProcessParts(item.SubParts, false);

#if DEBUG
                                //Info("51133947-8891-4354-A0C8-6385F85D9573", $"convertedSubItems = {convertedSubItems.WriteListToString()}");
#endif

                                subItems.AddRange(convertedSubItems);
                            }

                            var content = item.Token.Content;

                            if (wasWord)
                            {
                                var lastChar = nameValueSb.Last();

#if DEBUG
                                //Info("86502C7E-188A-4E73-B02F-BEE4187C347A", $"lastChar = '{lastChar}'");
                                
#endif

                                if (!content.StartsWith("-") && lastChar != '-' && !content.StartsWith("_") && lastChar != '_')
                                {
                                    nameValueSb.Append(" ");
                                    normalizedNameValueSb.Append(" ");
                                }

                                nameValueSb.Append(content);
                                normalizedNameValueSb.Append(content);
                            }
                            else
                            {
                                nameValueSb.Append("`");
                                nameValueSb.Append(content);
                                normalizedNameValueSb.Append(content);
                                wasWord = true;
                            }
                        }
                        break;

                    case TokenKind.Number:
                    case TokenKind.Minus:
                        {
                            var content = item.Token.Content;

#if DEBUG
                            //Info("FC21F895-676B-4918-BD4A-05D392FD0DF5", $"content = '{content}'");
#endif

                            if (wasWord)
                            {
                                var lastChar = nameValueSb.Last();

#if DEBUG
                                //Info("45FDE69C-4954-4E37-A2CC-1BD6EAA984B9", $"lastChar = '{lastChar}'");

#endif

                                if (!content.StartsWith("-") && lastChar != '-')
                                {
                                    nameValueSb.Append(" ");
                                    normalizedNameValueSb.Append(" ");
                                }


                                nameValueSb.Append(content);
                                normalizedNameValueSb.Append(content);
                            }
                            else
                            {
                                nameValueSb.Append("`");
                                nameValueSb.Append(content);
                                normalizedNameValueSb.Append(content);
                                wasWord = true;
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(tokenKind), tokenKind, $"In `{Text}`.");
                }
            }

            if(wasWord)
            {
                nameValueSb.Append("`");
            }
            else
            {
                switch(kindOfName)
                {
                    case KindOfName.EntityCondition:
                        kindOfName = KindOfName.AnonymousEntityCondition;
                        break;

                    case KindOfName.OnceResolvedEntityCondition:
                        kindOfName = KindOfName.OnceResolvedAnonymousEntityCondition;
                        break;

                    case KindOfName.Unknown:
                    case KindOfName.CommonConcept:
                    case KindOfName.Entity:
                    case KindOfName.RuleOrFact:
                    case KindOfName.Concept:
                    case KindOfName.Channel:
                    case KindOfName.Var:
                    case KindOfName.SystemVar:
                    case KindOfName.LogicalVar:
                    case KindOfName.LinguisticVar:
                    case KindOfName.Property:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, $"Unexpected {nameof(KindOfName)} anonymous identifier in `{Text}`.");
                }
            }

            if (isRootItem && !capacity.HasValue && !hasInfiniteCapacity)
            {
                capacity = 1;
            }

#if DEBUG
            //Info("711C4952-C769-4A7F-B7DB-2287A09C9F1F", $"nameValueSb = {nameValueSb}");
            //Info("0A91B826-A842-4757-8015-8F10297172B2", $"normalizedNameValueSb = {normalizedNameValueSb}");
            //Info("51A42AD0-4B49-4CEF-AE6F-1256697D3766", $"kindOfName = {kindOfName}");
            //Info("53C85862-4A77-4A84-86A9-E4D4FFF10EB9", $"level = {level}");
            //Info("BEB5A1ED-AF93-4DEA-B974-3ACE0D4F18DC", $"capacity = {capacity}");
            //Info("39D5DFC1-7E0E-4E56-9255-2A3C137CD69A", $"hasInfiniteCapacity = {hasInfiniteCapacity}");
            //Info("16324165-46D6-401C-A9E2-35731FECA78A", $"subItems = {subItems.WriteListToString()}");
#endif

            var nameValue = nameValueSb.ToString().Trim();

            var result = new StrongIdentifierValue
            {
                IsEmpty = string.IsNullOrWhiteSpace(nameValue),
                NameValue = nameValue,
                NormalizedNameValue = normalizedNameValueSb.ToString().ToLower().Trim(),
                KindOfName = kindOfName,
                Level = level,
                Capacity = capacity,
                HasInfiniteCapacity = hasInfiniteCapacity,
                IsArray = (capacity.HasValue && capacity.Value > 1) || hasInfiniteCapacity
            };

            if(subItems.Any())
            {
                result.Namespaces.AddRange(subItems);
            }

            result.CheckDirty();

            return result;
        }

        private string BuildExceptionMessage1(string tokenKindName)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"An {tokenKindName} can not be placed in the middle of an identifier.");
            sb.Append($"in `{Text}`.");
            return sb.ToString();
        }

        private string BuildExceptionMessage2(string tokenKindName, KindOfName kindOfName, StrongIdentifierPart item)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"An {nameof(tokenKindName)} is already used. The identifier has already kind of name `{kindOfName}`. But prefix `{item.Token.Content}` is used.");
            sb.Append($"in `{Text}`.");
            return sb.ToString();
        }

        private List<List<StrongIdentifierPart>> GroupPartsByOr(List<StrongIdentifierPart> items)
        {
#if DEBUG
            //Info("F5AA0CA1-CE97-4AD0-BD7A-304049093881", $"items = {items.WriteListToString()}");
#endif

            var result = new List<List<StrongIdentifierPart>>();

            var setOfParts = new List<StrongIdentifierPart>();

            foreach (var item in items)
            {
#if DEBUG
                //Info("51E5C9D1-37A9-45D5-B2A3-DB3937A62B33", $"item = {item}");
#endif

                var token = item.Token;

                if(token == null)
                {
                    setOfParts.Add(item);
                }
                else
                {
                    var tokenKind = token.TokenKind;

                    switch (tokenKind)
                    {
                        case TokenKind.Or:
                            result.Add(setOfParts);
                            setOfParts = new List<StrongIdentifierPart>();
                            break;

                        case TokenKind.Gravis:
                            break;

                        default:
                            setOfParts.Add(item);
                            break;
                    }
                }
            }

            result.Add(setOfParts);

            return result;
        }

        private List<List<StrongIdentifierPart>> GroupPartsByDoubleColon(List<StrongIdentifierPart> items)
        {
#if DEBUG
            //Info("FCE78ECE-2AFF-4C8D-A1E5-F4E61D3D4233", $"items = {items.WriteListToString()}");
#endif

            var result = new List<List<StrongIdentifierPart>>();

            var setOfParts = new List<StrongIdentifierPart>();

            foreach (var item in items)
            {
#if DEBUG
                //Info("03D2A14F-E8B2-46F0-B9C8-6B12881E019C", $"item = {item}");
#endif

                var token = item.Token;

                if(token == null)
                {
                    setOfParts.Add(item);
                }
                else
                {
                    var tokenKind = token.TokenKind;

                    switch (tokenKind)
                    {
                        case TokenKind.DoubleColon:
                            result.Add(setOfParts);
                            setOfParts = new List<StrongIdentifierPart>();
                            break;

                        case TokenKind.Gravis:
                            break;

                        default:
                            setOfParts.Add(item);
                            break;
                    }
                }
            }

            result.Add(setOfParts);

            return result;
        }
    }
}
