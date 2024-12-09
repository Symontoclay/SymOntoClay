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

using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    /// <summary>
    /// It is top level parser for source code file.
    /// </summary>
    public class SourceCodeParser: BaseInternalParser
    {
        public SourceCodeParser(InternalParserContext context)
            : base(context)
        {
        }

        public List<CodeItem> Result { get; set; } = new List<CodeItem>();

        /// <inheritdoc/>
        protected override void OnRun()
        {
            switch(_currToken.TokenKind)
            {
                case TokenKind.Word:
                    switch(_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.World:
                            {
                                _context.Recovery(_currToken);
                                var parser = new WorldPaser(_context);
                                parser.Run();
                                Result.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.App:
                            {
                                _context.Recovery(_currToken);
                                var parser = new AppPaser(_context);
                                parser.Run();
                                Result.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.Lib:
                            {
                                _context.Recovery(_currToken);
                                var parser = new LibPaser(_context);
                                parser.Run();
                                Result.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.Class:
                            {
                                _context.Recovery(_currToken);
                                var parser = new ClassPaser(_context);
                                parser.Run();
                                Result.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.Action:
                            {
                                _context.Recovery(_currToken);
                                var parser = new ActionPaser(_context);
                                parser.Run();
                                Result.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.State:
                            {
                                _context.Recovery(_currToken);
                                var parser = new StatePaser(_context);
                                parser.Run();
                                Result.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.LinguisticVariable:
                            {
                                _context.Recovery(_currToken);
                                var parser = new LinguisticVariableParser(_context);
                                parser.Run();
                                Result.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.Fun:
                            {
                                _context.Recovery(_currToken);
                                var parser = new FunctionDeclParser(_context);
                                parser.Run();
                                Result.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.States:
                            {
                                _context.Recovery(_currToken);
                                var parser = new MutuallyExclusiveStatesParser(_context);
                                parser.Run();
                                Result.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.Relation:
                            {
                                _context.Recovery(_currToken);
                                var parser = new RelationDescriptionParser(_context);
                                parser.Run();
                                Result.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.Synonym:
                            {
                                _context.Recovery(_currToken);
                                var parser = new SynonymParser(_context);
                                parser.Run();
                                Result.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.Compound:
                            {
                                _context.Recovery(_currToken);
                                var parser = new CompoundTaskParser(_context);
                                parser.Run();
                                Result.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.Primitive:
                            {
                                _context.Recovery(_currToken);
                                var parser = new PrimitiveTaskParser(_context);
                                parser.Run();

                                throw new NotImplementedException("3DE3C20B-02DD-47BF-83F8-900D06BD79E6");
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case TokenKind.OpenFactBracket:
                    {
                        _context.Recovery(_currToken);
                        var parser = new LogicalQueryAsCodeEntityParser(_context);
                        parser.Run();
                        Result.Add(parser.Result);
                    }
                    break;

                default:
                    throw new UnexpectedTokenException(_currToken);
            }
        }
    }
}
