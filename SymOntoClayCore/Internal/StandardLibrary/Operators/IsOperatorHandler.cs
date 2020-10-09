/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class IsOperatorHandler: BaseLoggedComponent, IBinaryOperatorHandler
    {
        public IsOperatorHandler(IEngineContext engineContext)
            : base(engineContext.Logger)
        {
            _engineContext = engineContext;

            var dataResolversFactory = engineContext.DataResolversFactory;

            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();
            _strongIdentifierLinearResolver = dataResolversFactory.GetStrongIdentifierLinearResolver();
        }

        private readonly IEngineContext _engineContext;
        private readonly InheritanceResolver _inheritanceResolver;
        private readonly StrongIdentifierLinearResolver _strongIdentifierLinearResolver;

        /// <inheritdoc/>
        public IndexedValue Call(IndexedValue leftOperand, IndexedValue rightOperand, IndexedValue annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"leftOperand = {leftOperand}");
            //Log($"rightOperand = {rightOperand}");
            //Log($"annotation = {annotation}");
#endif

            if ((leftOperand.IsStrongIdentifierValue || leftOperand.IsInstanceValue) && (leftOperand.IsStrongIdentifierValue || leftOperand.IsInstanceValue))
            {
                return GetInheritanceRank(leftOperand, rightOperand, localCodeExecutionContext);
            }

            throw new NotImplementedException();
        }

        private IndexedValue GetInheritanceRank(IndexedValue leftOperand, IndexedValue rightOperand, LocalCodeExecutionContext localCodeExecutionContext)
        {
            var subName = _strongIdentifierLinearResolver.Resolve(leftOperand, localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            var superName = _strongIdentifierLinearResolver.Resolve(rightOperand, localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            return _inheritanceResolver.GetInheritanceRank(subName, superName, localCodeExecutionContext, ResolverOptions.GetDefaultOptions());
        }
    }
}
