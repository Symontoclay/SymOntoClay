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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using System;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class PrimitiveHtnTaskPreconditionsParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotPreconditionsMark,
            GotConditionExpr
        }

        public PrimitiveHtnTaskPreconditionsParser(InternalParserContext context)
            : base(context)
        {
        }
        
        public LogicalExecutableExpression Result { get; private set; }
        public AstExpression OriginalExpression { get; private set; }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Info("D99532FA-6FA7-444A-B6DD-678033D29E63", $"_state = {_state}");
            //Info("A4A5D717-394B-4067-8C78-4F96BC1DC9BC", $"_currToken = {_currToken}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Preconditions:
                                    _state = State.GotPreconditionsMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotPreconditionsMark:
                    {
                        _context.Recovery(_currToken);

                        var parser = new AstExpressionParser(_context, new TerminationToken(TokenKind.Semicolon, true));
                        parser.Run();

                        var conditionExpr = parser.Result;

#if DEBUG
                        //Info("CF50DF8B-E604-43FB-BB80-E21937EABBAE", $"conditionExpr = {conditionExpr}");
                        //Info("9076EAFE-F030-46FC-B828-BEB56DE339B0", $"conditionExpr = {conditionExpr.ToHumanizedString()}");
#endif

                        OriginalExpression = conditionExpr;

                        var compiledFunctionBody = _context.Compiler.CompileLambda(conditionExpr);

                        Result = new LogicalExecutableExpression(conditionExpr, compiledFunctionBody);

                        _state = State.GotConditionExpr;
                    }
                    break;

                case State.GotConditionExpr:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
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
