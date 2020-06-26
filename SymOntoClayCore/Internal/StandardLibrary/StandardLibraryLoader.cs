using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
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
        }

        private void RegOperators()
        {
            var globalStorage = _context.Storage.GlobalStorage;
            var globalOperatorsStorage = globalStorage.OperatorsStorage;

            var op = new Operator
            {
                KindOfOperator = KindOfOperator.LeftRightStream,
                IsSystemDefined = true,
                SystemHandler = new BinaryOperatorSystemHandler(new LeftRightStreamOperatorHandler(_context), _context.Dictionary)
            };
            globalOperatorsStorage.Append(op);
        }
    }
}
