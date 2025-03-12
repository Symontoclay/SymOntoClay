/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class PackedInheritanceResolver: IPackedInheritanceResolver
    {
        public PackedInheritanceResolver(InheritanceResolver inheritanceResolver, ILocalCodeExecutionContext localCodeExecutionContext)
            : this(inheritanceResolver, localCodeExecutionContext, inheritanceResolver.DefaultOptions)
        {
        }

        public PackedInheritanceResolver(InheritanceResolver inheritanceResolver, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            _inheritanceResolver = inheritanceResolver;
            _localCodeExecutionContext = localCodeExecutionContext;
            _options = options;
        }

        private readonly InheritanceResolver _inheritanceResolver;
        private readonly ILocalCodeExecutionContext _localCodeExecutionContext;
        private readonly ResolverOptions _options;

        /// <inheritdoc/>
        public Value GetInheritanceRank(IMonitorLogger logger, TypeInfo subType, TypeInfo superType)
        {
            return _inheritanceResolver.GetInheritanceRank(logger, subType, superType, _localCodeExecutionContext, _options);
        }

        /// <inheritdoc/>
        public float GetRawInheritanceRank(IMonitorLogger logger, TypeInfo subType, TypeInfo superType)
        {
            return _inheritanceResolver.GetRawInheritanceRank(logger, subType, superType, _localCodeExecutionContext, _options);
        }

        /// <inheritdoc/>
        public IList<StrongIdentifierValue> GetSuperClassesKeysList(IMonitorLogger logger, StrongIdentifierValue subName)
        {
            return _inheritanceResolver.GetSuperClassesKeysList(logger, subName, _localCodeExecutionContext, _options);
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceItem> GetWeightedInheritanceItems(IMonitorLogger logger, TypeInfo subType)
        {
            return _inheritanceResolver.GetWeightedInheritanceItems(logger, subType, _localCodeExecutionContext, _options);
        }

        /// <inheritdoc/>
        public bool IsFit(IMonitorLogger logger, IList<TypeInfo> typeInfoList, Value value)
        {
            return _inheritanceResolver.IsFit(logger, typeInfoList, value, _localCodeExecutionContext, _options);
        }

        /// <inheritdoc/>
        public uint? GetDistance(IMonitorLogger logger, IList<TypeInfo> typeInfoList, Value value)
        {
            return _inheritanceResolver.GetDistance(logger, typeInfoList, value, _localCodeExecutionContext, _options);
        }

        /// <inheritdoc/>
        public uint? GetDistance(IMonitorLogger logger, TypeInfo subType, StrongIdentifierValue superName)
        {
            return _inheritanceResolver.GetDistance(logger, subType, superName, _localCodeExecutionContext, _options);
        }
    }
}
