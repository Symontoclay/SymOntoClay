using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class PackedInheritanceResolver: IPackedInheritanceResolver
    {
        public PackedInheritanceResolver(InheritanceResolver inheritanceResolver, LocalCodeExecutionContext localCodeExecutionContext)
            : this(inheritanceResolver, localCodeExecutionContext, inheritanceResolver.DefaultOptions)
        {
        }

        public PackedInheritanceResolver(InheritanceResolver inheritanceResolver, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            _inheritanceResolver = inheritanceResolver;
            _localCodeExecutionContext = localCodeExecutionContext;
            _options = options;
        }

        private readonly InheritanceResolver _inheritanceResolver;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly ResolverOptions _options;

        /// <inheritdoc/>
        public Value GetInheritanceRank(StrongIdentifierValue subName, StrongIdentifierValue superName)
        {
            return _inheritanceResolver.GetInheritanceRank(subName, superName, _localCodeExecutionContext, _options);
        }

        /// <inheritdoc/>
        public float GetRawInheritanceRank(StrongIdentifierValue subName, StrongIdentifierValue superName)
        {
            return _inheritanceResolver.GetRawInheritanceRank(subName, superName, _localCodeExecutionContext, _options);
        }

        /// <inheritdoc/>
        public IList<StrongIdentifierValue> GetSuperClassesKeysList(StrongIdentifierValue subName)
        {
            return _inheritanceResolver.GetSuperClassesKeysList(subName, _localCodeExecutionContext, _options);
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(StrongIdentifierValue subName)
        {
            return _inheritanceResolver.GetWeightedInheritanceItems(subName, _localCodeExecutionContext, _options);
        }

        /// <inheritdoc/>
        public bool IsFit(IList<StrongIdentifierValue> typeNamesList, Value value)
        {
            return _inheritanceResolver.IsFit(typeNamesList, value, _localCodeExecutionContext, _options);
        }

        /// <inheritdoc/>
        public uint? GetDistance(IList<StrongIdentifierValue> typeNamesList, Value value)
        {
            return _inheritanceResolver.GetDistance(typeNamesList, value, _localCodeExecutionContext, _options);
        }

        /// <inheritdoc/>
        public uint? GetDistance(StrongIdentifierValue subName, StrongIdentifierValue superName)
        {
            return _inheritanceResolver.GetDistance(subName, superName, _localCodeExecutionContext, _options);
        }
    }
}
