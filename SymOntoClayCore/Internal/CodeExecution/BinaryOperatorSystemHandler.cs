/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class BinaryOperatorSystemHandler: ISystemHandler
    {
        public BinaryOperatorSystemHandler(IBinaryOperatorHandler operatorHandler, IEntityDictionary entityDictionary)
        {
            _leftOperandKey = entityDictionary.GetKey("leftOperand");
            _rightOperandKey = entityDictionary.GetKey("rightOperand");
            _operatorHandler = operatorHandler;
        }

        private readonly ulong _leftOperandKey;
        private readonly ulong _rightOperandKey;
        private readonly IBinaryOperatorHandler _operatorHandler;

        /// <inheritdoc/>
        public IndexedValue Call(IList<IndexedValue> paramsList, LocalCodeExecutionContext localCodeExecutionContext)
        {
            var leftOperand = paramsList[0];
            var rightOperand = paramsList[1];
            var anotation = paramsList[2];

            return _operatorHandler.Call(leftOperand, rightOperand, anotation, localCodeExecutionContext);
        }
        
        /// <inheritdoc/>
        public IndexedValue Call(IDictionary<ulong, IndexedValue> paramsDict, IndexedValue anotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
            var leftOperand = paramsDict[_leftOperandKey];
            var rightOperand = paramsDict[_rightOperandKey];

            return _operatorHandler.Call(leftOperand, rightOperand, anotation, localCodeExecutionContext);
        }
    }
}
