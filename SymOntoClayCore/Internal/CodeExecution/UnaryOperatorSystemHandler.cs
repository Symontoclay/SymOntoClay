/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class UnaryOperatorSystemHandler : ISystemHandler
    {
        public UnaryOperatorSystemHandler(IUnaryOperatorHandler operatorHandler, IEntityDictionary entityDictionary)
        {
            _operandKey = entityDictionary.GetKey("operand");
            _operatorHandler = operatorHandler;
        }

        private readonly ulong _operandKey;
        private readonly IUnaryOperatorHandler _operatorHandler;

        /// <inheritdoc/>
        public IndexedValue Call(IList<IndexedValue> paramsList, LocalCodeExecutionContext localCodeExecutionContext)
        {
            var operand = paramsList[0];
            var anotation = paramsList[1];

            return _operatorHandler.Call(operand, anotation, localCodeExecutionContext);
        }

        /// <inheritdoc/>
        public IndexedValue Call(IDictionary<ulong, IndexedValue> paramsDict, IndexedValue anotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
            var operand = paramsDict[_operandKey];

            return _operatorHandler.Call(operand, anotation, localCodeExecutionContext);
        }
    }
}
