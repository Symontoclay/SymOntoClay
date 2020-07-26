﻿using SymOntoClay.Core.Internal.CodeExecution;
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

            var op = new Operator
            {
                KindOfOperator = KindOfOperator.LeftRightStream,
                IsSystemDefined = true,
                SystemHandler = new BinaryOperatorSystemHandler(new LeftRightStreamOperatorHandler(_context), _context.Dictionary),
                Holder = _context.CommonNamesStorage.DefaultHolder
            };
            globalOperatorsStorage.Append(op);

            op = new Operator
            {
                KindOfOperator = KindOfOperator.Is,
                IsSystemDefined = true,
                SystemHandler = new BinaryOperatorSystemHandler(new IsOperatorHandler(_context), _context.Dictionary),
                Holder = _context.CommonNamesStorage.DefaultHolder
            };
            globalOperatorsStorage.Append(op);
        }

        private void RegChannels()
        {
            var globalStorage = _context.Storage.GlobalStorage;
            var globalChannelsStorage = globalStorage.ChannelsStorage;

            var name = NameHelper.CreateName("@>log", _context.Dictionary);

            var channel = new Channel();
            channel.Name = name;
            channel.Handler = new LogChannelHandler(_context);
            channel.Holder = _context.CommonNamesStorage.DefaultHolder;

            globalChannelsStorage.Append(channel);
        }
    }
}
