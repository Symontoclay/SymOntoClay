using NUnit.Framework;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Unity3DAsset.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test
{
    public class Lexer_Tests
    {
        [SetUp]
        public void Setup()
        {
            _logger = new EmptyLogger();
        }

        private ILogger _logger;

        [Test]
        public void Lexer_Tests_Case1()
        {
            var text = @"app Enemy
{
};";

            var lexer = new Lexer(text, _logger);

            Token token = null;

            var n = 0;

            while ((token = lexer.GetToken()) != null)
            {
                n++;

                switch(n)
                {
                    case 1:
                        Assert.AreEqual(token.TokenKind, TokenKind.Word);
                        Assert.AreEqual(token.KeyWordTokenKind, KeyWordTokenKind.App);
                        Assert.AreEqual(token.Content, "app");
                        Assert.AreEqual(token.Pos, 1);
                        Assert.AreEqual(token.Line, 1);
                        break;

                    case 2:
                        Assert.AreEqual(token.TokenKind, TokenKind.Word);
                        Assert.AreEqual(token.KeyWordTokenKind, KeyWordTokenKind.Unknown);
                        Assert.AreEqual(token.Content, "Enemy");
                        Assert.AreEqual(token.Pos, 5);
                        Assert.AreEqual(token.Line, 1);
                        break;

                    case 3:
                        Assert.AreEqual(token.TokenKind, TokenKind.OpenFigureBracket);
                        Assert.AreEqual(token.KeyWordTokenKind, KeyWordTokenKind.Unknown);
                        Assert.AreEqual(token.Content, string.Empty);
                        Assert.AreEqual(token.Pos, 1);
                        Assert.AreEqual(token.Line, 2);
                        break;

                    case 4:
                        Assert.AreEqual(token.TokenKind, TokenKind.CloseFigureBracket);
                        Assert.AreEqual(token.KeyWordTokenKind, KeyWordTokenKind.Unknown);
                        Assert.AreEqual(token.Content, string.Empty);
                        Assert.AreEqual(token.Pos, 1);
                        Assert.AreEqual(token.Line, 3);
                        break;

                    case 5:
                        Assert.AreEqual(token.TokenKind, TokenKind.Semicolon);
                        Assert.AreEqual(token.KeyWordTokenKind, KeyWordTokenKind.Unknown);
                        Assert.AreEqual(token.Content, string.Empty);
                        Assert.AreEqual(token.Pos, 2);
                        Assert.AreEqual(token.Line, 3);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            }
        }
    }
}
