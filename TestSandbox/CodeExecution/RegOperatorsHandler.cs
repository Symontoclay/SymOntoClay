using NLog;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Text;
using TestSandbox.Helpers;

namespace TestSandbox.CodeExecution
{
    public class RegOperatorsHandler: IBinaryOperatorHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            var context = TstEngineContextHelper.CreateAndInitContext();

            var globalStorage = context.Storage.GlobalStorage;

            _logger.Info($"globalStorage == null = {globalStorage == null}");

            var globalOperatorsStorage = globalStorage.OperatorsStorage;

            var op = new Operator();
            op.KindOfOperator = KindOfOperator.LeftRightStream;
            op.IsSystemDefined = true;
            op.SystemHandler = new  BinaryOperatorSystemHandler(this, context.Dictionary);

            _logger.Info($"op = {op}");

            globalOperatorsStorage.Append(op);

            var targetOp = globalOperatorsStorage.GetOperator(KindOfOperator.LeftRightStream);

            _logger.Info($"targetOp = {targetOp}");

            _logger.Info("End");
        }

        public Value Call(Value leftOperand, Value rightOperand, IStorage localContext)
        {
            _logger.Info($"leftOperand = {leftOperand}");
            _logger.Info($"rightOperand = {rightOperand}");

            return new NumberValue(5);
        }
    }
}
