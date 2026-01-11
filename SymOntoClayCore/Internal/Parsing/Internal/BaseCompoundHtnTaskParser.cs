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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Linq;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public abstract class BaseCompoundHtnTaskParser : BaseInternalParser
    {
        protected BaseCompoundHtnTaskParser(InternalParserContext context)
            : base(context)
        {
        }

        public BaseCompoundHtnTask Result { get; protected set; }

        protected void ProcessGeneralContent()
        {
#if DEBUG
            //Info("100904DC-698C-45AA-A704-B268A9712F01", $"_currToken = {_currToken}");
#endif

            switch (_currToken.TokenKind)
            {
                case TokenKind.CloseFigureBracket:
                    Exit();
                    break;

                case TokenKind.Word:
                    switch (_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.Preconditions:
                            {
                                _context.Recovery(_currToken);

                                var parser = new PrimitiveHtnTaskPreconditionsParser(_context);
                                parser.Run();

#if DEBUG
                                //Info("44381852-131C-4F29-B506-F254DE8B95E1", $"parser.Result = {parser.Result}");
#endif

                                Result.Precondition = parser.Result;
                                Result.PreconditionExpression = parser.OriginalExpression;
                            }
                            break;

                        case KeyWordTokenKind.Case:
                            {
                                _context.Recovery(_currToken);

                                var parser = new CompoundHtnTaskCaseParser(_context);
                                parser.Run();

                                Result.Cases.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.Prop:
                            {
                                _context.Recovery(_currToken);

                                var parser = new PropertyParser(_context);
                                parser.Run();

                                Result.SubItems.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.Before:
                            {
                                _context.Recovery(_currToken);

                                var parser = new CompoundHtnTaskBeforeParser(_context);
                                parser.Run();

                                Result.Before = parser.Result;
                            }
                            break;

                        case KeyWordTokenKind.After:
                            {
                                _context.Recovery(_currToken);

                                var parser = new CompoundHtnTaskAfterParser(_context);
                                parser.Run();

                                Result.After = parser.Result;
                            }
                            break;

                        case KeyWordTokenKind.Background:
                            {
                                _context.Recovery(_currToken);

                                var parser = new CompoundHtnTaskBackgroundParser(_context);
                                parser.Run();

                                var parsingResult = parser.Result;

#if DEBUG
                                //Info("62D56D5A-937D-4241-9B02-4AF70948401F", $"parsingResult = {parsingResult}");
                                //throw new NotImplementedException("7FD4F4FA-ADCF-4611-B9CE-0153096B325D");
#endif

                                parsingResult.Holder = Result.Name;

                                Result.Backgrounds.Add(parsingResult);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                default:
                    throw new UnexpectedTokenException(Text, _currToken);
            }
        }

        protected virtual void Validate()
        {
            ValidateCases();
        }

        private void ValidateCases()
        {
            var cases = Result.Cases;

            if(cases.IsNullOrEmpty())
            {
                return;
            }

            var defaultCases = cases.Where(p => p.Condition == null);

            var defaultCasesCount = defaultCases.Count();

#if DEBUG
            //Info("26DB4C8B-9BE4-415D-BE1F-5C6CE0252DB0", $"defaultCasesCount = {defaultCasesCount}");
#endif

            if(defaultCasesCount > 1)
            {
                throw new DuplicatedCompoundHtnTaskCaseException(Result.Name.ToHumanizedLabel(), defaultCases.First().ToHumanizedLabel(), defaultCasesCount);
            }

            Result.Cases = cases.OrderBy(p => p.Condition == null).ToList();
        }
    }
}
