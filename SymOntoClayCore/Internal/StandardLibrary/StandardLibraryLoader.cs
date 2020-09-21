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
                KindOfOperator = KindOfOperator.SelectLogicalQuery,
                IsSystemDefined = true,
                SystemHandler = new UnaryOperatorSystemHandler(new SelectLogicalQueryOperatorHandler(_context), dictionary),
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
