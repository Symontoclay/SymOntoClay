/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
