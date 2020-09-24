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

                        case KeyWordTokenKind.Host:
                            {
                                _context.Recovery(_currToken);
                                var parser = new HostPaser(_context);
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
