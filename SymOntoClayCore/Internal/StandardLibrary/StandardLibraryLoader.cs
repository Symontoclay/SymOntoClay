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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.StandardLibrary.Channels;
using SymOntoClay.Core.Internal.StandardLibrary.Operators;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary
{
    public class StandardLibraryLoader : BaseComponent
    {
        public StandardLibraryLoader(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IEngineContext _context;

        public void LoadFromSourceCode()
        {
            RegOperators();
            RegChannels();
        }

        private void RegOperators()
        {
            var globalStorage = _context.Storage.GlobalStorage;
            var globalOperatorsStorage = globalStorage.OperatorsStorage;
            var dictionary = _context.Dictionary;

            var op = new Operator
            {
                KindOfOperator = KindOfOperator.LeftRightStream,
                IsSystemDefined = true,
                SystemHandler = new BinaryOperatorSystemHandler(new LeftRightStreamOperatorHandler(_context), dictionary),
                Holder = _context.CommonNamesStorage.DefaultHolder
            };

            globalOperatorsStorage.Append(op);

            op = new Operator
            {
                KindOfOperator = KindOfOperator.Is,
                IsSystemDefined = true,
                SystemHandler = new BinaryOperatorSystemHandler(new IsOperatorHandler(_context), dictionary),
                Holder = _context.CommonNamesStorage.DefaultHolder
            };

            globalOperatorsStorage.Append(op);

            op = new Operator
            {
                KindOfOperator = KindOfOperator.Point,
                IsSystemDefined = true,
                SystemHandler = new BinaryOperatorSystemHandler(new PointOperatorHandler(_context), dictionary),
                Holder = _context.CommonNamesStorage.DefaultHolder
            };

            globalOperatorsStorage.Append(op);

            op = new Operator
            {
                KindOfOperator = KindOfOperator.CallLogicalQuery,
                IsSystemDefined = true,
                SystemHandler = new UnaryOperatorSystemHandler(new CallLogicalQueryOperatorHandler(_context), dictionary),
                Holder = _context.CommonNamesStorage.DefaultHolder
            };

            globalOperatorsStorage.Append(op);
        }

        private void RegChannels()
        {
            var globalStorage = _context.Storage.GlobalStorage;
            var globalChannelsStorage = globalStorage.ChannelsStorage;

            var name = NameHelper.CreateName("@>log", _context.Dictionary);

            var channel = new Channel
            {
                Name = name,
                Handler = new LogChannelHandler(_context),
                Holder = _context.CommonNamesStorage.DefaultHolder
            };

            globalChannelsStorage.Append(channel);
        }
    }
}
