/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
