using NLog;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;
using TestSandbox.Helpers;

namespace TestSandbox.CodeExecution
{
    public class RegOperatorsHandler: IBinaryOperatorHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly EngineContext _context;

        public RegOperatorsHandler()
        {
            _context = TstEngineContextHelper.CreateAndInitContext();
        }

        public void Run()
        {
            _logger.Info("Begin");

            var globalStorage = _context.Storage.GlobalStorage;

            _logger.Info($"globalStorage == null = {globalStorage == null}");

            var globalOperatorsStorage = globalStorage.OperatorsStorage;

            var op = new Operator();
            op.KindOfOperator = KindOfOperator.LeftRightStream;
            op.IsSystemDefined = true;
            op.SystemHandler = new  BinaryOperatorSystemHandler(this, _context.Dictionary);

            _logger.Info($"op = {op}");

            globalOperatorsStorage.Append(op);

            var operatorsResolver = _context.DataResolversFactory.GetOperatorsResolver();

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            localCodeExecutionContext.Storage = globalStorage;

            var targetOp = operatorsResolver.GetOperator(KindOfOperator.LeftRightStream, localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            _logger.Info($"targetOp = {targetOp}");

            _logger.Info("End");
        }

        public IndexedValue Call(IndexedValue leftOperand, IndexedValue rightOperand, IndexedValue annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
            _logger.Info($"leftOperand = {leftOperand}");
            _logger.Info($"rightOperand = {rightOperand}");

            var result = new NumberValue(5);

            return result.GetIndexed(_context);
        }
    }
}
