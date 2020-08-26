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
