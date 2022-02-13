/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.StandardLibrary.FuzzyLogic;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LinguisticVariableParser: BaseInternalParser
    {
        private enum State
        {
            Init,
            GotLinguisticVariableMark,
            GotName,
            GotFor,
            GotRange,
            InContent,
            WaitTerm,
            GotTermName,
            WaitPredefinedMembershipFunction,
            GotNameOfPredefinedMembershipFunction,
            WaitForParameterOfMembershipFunction,
            GotParameterOfMembershipFunction,
            GotPredefinedMembershipFunction,
            WaitConstraint,
            GotConstraintFor,
            GotConstraintRelationMark,
            GotConstraintRelationName,
            GotConstraintInheritanceMark
        }

        public LinguisticVariableParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public CodeEntity Result { get; private set; }
        private LinguisticVariable _linguisticVariable;
        private FuzzyLogicNonNumericValue _currentFuzzyLogicNonNumericValue;
        private string _nameOfPredefinedMembershipFunction = string.Empty;
        private List<NumberValue> _parametersOfPredefinedMembershipFunction;
        private LinguisticVariableConstraintItem _currentConstraintItem;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = CreateCodeEntityAndSetAsCurrent(KindOfCodeEntity.LinguisticVariable);

            _linguisticVariable = CreateLinguisticVariable();
            _linguisticVariable.CodeEntity = Result;

            Result.LinguisticVariable = _linguisticVariable;
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
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");           
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.LinguisticVariable:
                            _state = State.GotLinguisticVariableMark;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotLinguisticVariableMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            var name = ParseName(_currToken.Content);
                            Result.Name = name;
                            _linguisticVariable.Name = name;
                            _state = State.GotName;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.For:
                                    _state = State.GotFor;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        case TokenKind.OpenFigureBracket:
                            _state = State.InContent;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotFor:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Range:
                                    {
                                        _context.Recovery(_currToken);

                                        var parser = new InlineRangeParser(_context);
                                        parser.Run();

#if DEBUG
                                        //Log($"parser.Result = {parser.Result}");
#endif

                                        _linguisticVariable.Range = parser.Result;
                                        _state = State.GotRange;                                        
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

                case State.GotRange:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _state = State.InContent;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.InContent:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Identifier:
                            _context.Recovery(_currToken);
                            _state = State.WaitTerm;
                            break;

                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Terms:
                                    {
                                        var nextToken = _context.GetToken();

#if DEBUG
                                        //Log($"nextToken = {nextToken}");
#endif

                                        if(nextToken.TokenKind == TokenKind.Colon)
                                        {
                                            _state = State.WaitTerm;
                                            break;
                                        }

                                        _context.Recovery(_currToken);
                                        _context.Recovery(nextToken);
                                        _state = State.WaitTerm;
                                    }
                                    break;

                                case KeyWordTokenKind.Constraints:
                                    {
                                        var nextToken = _context.GetToken();

#if DEBUG
                                        //Log($"nextToken = {nextToken}");
#endif

                                        if (nextToken.TokenKind == TokenKind.Colon)
                                        {
                                            _state = State.WaitConstraint;
                                            break;
                                        }

                                        _context.Recovery(_currToken);
                                        _context.Recovery(nextToken);
                                        _state = State.WaitTerm;
                                    }
                                    break;
                                    

                                default:
                                    _context.Recovery(_currToken);
                                    _state = State.WaitTerm;
                                    break;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitTerm:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Constraints:
                                    {
                                        var nextToken = _context.GetToken();

#if DEBUG
                                        //Log($"nextToken = {nextToken}");
#endif

                                        if (nextToken.TokenKind == TokenKind.Colon)
                                        {
                                            _state = State.WaitConstraint;
                                            break;
                                        }

                                        _context.Recovery(_currToken);
                                        _context.Recovery(nextToken);
                                        _state = State.WaitTerm;
                                    }
                                    break;

                                default:
                                    GetTermName();
                                    break;
                            }
                            break;

                        case TokenKind.Identifier:
                            GetTermName();
                            break;

                        case TokenKind.CloseFigureBracket:
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotTermName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Assign:
                            _state = State.WaitPredefinedMembershipFunction;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitPredefinedMembershipFunction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            _nameOfPredefinedMembershipFunction = _currToken.Content;
                            _state = State.GotNameOfPredefinedMembershipFunction;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotNameOfPredefinedMembershipFunction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            _parametersOfPredefinedMembershipFunction = new List<NumberValue>();
                            _state = State.WaitForParameterOfMembershipFunction;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForParameterOfMembershipFunction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            {
                                _context.Recovery(_currToken);
                                var parser = new NumberParser(_context);
                                parser.Run();

#if DEBUG
                                //Log($"parser.Result = {parser.Result}");
#endif

                                _parametersOfPredefinedMembershipFunction.Add(parser.Result.AsNumberValue);

                                _state = State.GotParameterOfMembershipFunction;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotParameterOfMembershipFunction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Comma:
                            _state = State.WaitForParameterOfMembershipFunction;
                            break;

                        case TokenKind.CloseRoundBracket:
                            _currentFuzzyLogicNonNumericValue.Handler = CreateMembershipPredefinedHandler();

#if DEBUG
                            //Log($"_currentFuzzyLogicNonNumericValue = {_currentFuzzyLogicNonNumericValue}");
#endif

                            _state = State.GotPredefinedMembershipFunction;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotPredefinedMembershipFunction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
                            _nameOfPredefinedMembershipFunction = string.Empty;
                            _parametersOfPredefinedMembershipFunction = null;
                            _currentFuzzyLogicNonNumericValue = null;
                            _state = State.WaitTerm;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitConstraint:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.For:
                                    _currentConstraintItem = new LinguisticVariableConstraintItem();
                                    _linguisticVariable.Constraint.AddItem(_currentConstraintItem);
                                    _state = State.GotConstraintFor;
                                    break;

                                case KeyWordTokenKind.Terms:
                                    {
                                        var nextToken = _context.GetToken();

#if DEBUG
                                        //Log($"nextToken = {nextToken}");
#endif

                                        if (nextToken.TokenKind == TokenKind.Colon)
                                        {
                                            _state = State.WaitTerm;
                                            break;
                                        }

                                        throw new UnexpectedTokenException(_currToken);
                                    }

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        case TokenKind.CloseFigureBracket:
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotConstraintFor:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Relation:
                                    _currentConstraintItem.Kind = KindOfLinguisticVariableConstraintItem.Relation;
                                    _state = State.GotConstraintRelationMark;
                                    break;

                                case KeyWordTokenKind.Inheritance:
                                    _currentConstraintItem.Kind = KindOfLinguisticVariableConstraintItem.Inheritance;
                                    _state = State.GotConstraintInheritanceMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotConstraintRelationMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            _currentConstraintItem.RelationName = ParseName(_currToken.Content);
                            _state = State.GotConstraintRelationName;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotConstraintRelationName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
                            _state = State.WaitConstraint;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotConstraintInheritanceMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
                            _state = State.WaitConstraint;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private IFuzzyLogicMemberFunctionHandler CreateMembershipPredefinedHandler()
        {
#if DEBUG
            //Log($"_nameOfPredefinedMembershipFunction = {_nameOfPredefinedMembershipFunction}");
            //Log($"_parametersOfPredefinedMembershipFunction.Result = {_parametersOfPredefinedMembershipFunction.WriteListToString()}");
#endif

            var linguisticVariableName = _linguisticVariable.Name.NameValue;
            var termName = _currentFuzzyLogicNonNumericValue.Name.NameValue;

            switch (_nameOfPredefinedMembershipFunction)
            {
                case "Trapezoid":
                    return CreateTrapezoidMembershipPredefinedHandler(linguisticVariableName, termName);

                case "L":
                    return CreateLMembershipPredefinedHandler(linguisticVariableName, termName);

                case "S":
                    return CreateSMembershipPredefinedHandler(linguisticVariableName, termName);

                default:
                    throw new Exception($"Unknown membership function `{_nameOfPredefinedMembershipFunction}` of term `{termName}` of linguistic variable `{linguisticVariableName}`!");
            }
        }

        private void GetTermName()
        {
            _nameOfPredefinedMembershipFunction = string.Empty;
            _parametersOfPredefinedMembershipFunction = null;
            _currentFuzzyLogicNonNumericValue = new FuzzyLogicNonNumericValue();
            _linguisticVariable.Values.Add(_currentFuzzyLogicNonNumericValue);
            _currentFuzzyLogicNonNumericValue.Parent = _linguisticVariable;
            _currentFuzzyLogicNonNumericValue.Name = ParseName(_currToken.Content);
            _state = State.GotTermName;
        }

        private IFuzzyLogicMemberFunctionHandler CreateTrapezoidMembershipPredefinedHandler(string linguisticVariableName, string termName)
        {
            if(_parametersOfPredefinedMembershipFunction.Count != 4)
            {
                throw new Exception($"Wrong params count in membership function `{_nameOfPredefinedMembershipFunction}` of term `{termName}` of linguistic variable `{linguisticVariableName}`! It must be 4, but got {_parametersOfPredefinedMembershipFunction.Count}!");
            }

            return new TrapezoidFuzzyLogicMemberFunctionHandler(_parametersOfPredefinedMembershipFunction[0], _parametersOfPredefinedMembershipFunction[1], _parametersOfPredefinedMembershipFunction[2], _parametersOfPredefinedMembershipFunction[3]);
        }

        private IFuzzyLogicMemberFunctionHandler CreateLMembershipPredefinedHandler(string linguisticVariableName, string termName)
        {
            if (_parametersOfPredefinedMembershipFunction.Count != 2)
            {
                throw new Exception($"Wrong params count in membership function `{_nameOfPredefinedMembershipFunction}` of term `{termName}` of linguistic variable `{linguisticVariableName}`! It must be 2, but got {_parametersOfPredefinedMembershipFunction.Count}!");
            }

            return new LFunctionFuzzyLogicMemberFunctionHandler(_parametersOfPredefinedMembershipFunction[0], _parametersOfPredefinedMembershipFunction[1]);
        }

        private IFuzzyLogicMemberFunctionHandler CreateSMembershipPredefinedHandler(string linguisticVariableName, string termName)
        {
            if (_parametersOfPredefinedMembershipFunction.Count != 2 && _parametersOfPredefinedMembershipFunction.Count != 3)
            {
                throw new Exception($"Wrong params count in membership function `{_nameOfPredefinedMembershipFunction}` of term `{termName}` of linguistic variable `{linguisticVariableName}`! It must be 2 or 3, but got {_parametersOfPredefinedMembershipFunction.Count}!");
            }

            if(_parametersOfPredefinedMembershipFunction.Count == 2)
            {
                return new SFunctionFuzzyLogicMemberFunctionHandler(_parametersOfPredefinedMembershipFunction[0], _parametersOfPredefinedMembershipFunction[1]);
            }

            return new SFunctionFuzzyLogicMemberFunctionHandler(_parametersOfPredefinedMembershipFunction[0], _parametersOfPredefinedMembershipFunction[1], _parametersOfPredefinedMembershipFunction[2]);
        }
    }
}
