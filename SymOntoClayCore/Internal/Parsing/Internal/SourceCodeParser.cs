/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

        public List<CodeEntity> Result { get; set; } = new List<CodeEntity>();

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
#endif

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

                        case KeyWordTokenKind.Class:
                            {
                                _context.Recovery(_currToken);
                                var parser = new ClassPaser(_context);
                                parser.Run();
                                Result.Add(parser.Result);
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
