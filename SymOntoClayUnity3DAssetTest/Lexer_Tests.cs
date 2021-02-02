/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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

        private IEntityLogger _logger;

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
