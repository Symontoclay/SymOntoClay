/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
