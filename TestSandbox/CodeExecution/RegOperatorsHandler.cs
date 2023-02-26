/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
        private readonly IEngineContext _context;

        public RegOperatorsHandler()
        {
            _context = TstEngineContextHelper.CreateAndInitContext().EngineContext;
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
            op.SystemHandler = new  BinaryOperatorSystemHandler(this);

            _logger.Info($"op = {op}");

            globalOperatorsStorage.Append(op);

            var operatorsResolver = _context.DataResolversFactory.GetOperatorsResolver();

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            localCodeExecutionContext.Storage = globalStorage;

            var targetOp = operatorsResolver.GetOperator(KindOfOperator.LeftRightStream, localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            _logger.Info($"targetOp = {targetOp}");

            _logger.Info("End");
        }

        public Value Call(Value leftOperand, Value rightOperand, Value annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
            _logger.Info($"leftOperand = {leftOperand}");
            _logger.Info($"rightOperand = {rightOperand}");

            var result = new NumberValue(5);

            return result;
        }
    }
}
