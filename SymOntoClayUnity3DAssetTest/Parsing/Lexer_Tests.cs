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

using NUnit.Framework;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Tests.Parsing
{
    public class Lexer_Tests
    {
        [SetUp]
        public void Setup()
        {
            _logger = new EmptyLogger();
        }

        private IEntityLogger _logger;

        [Test]
        [Parallelizable]
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
