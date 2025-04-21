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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Monitor.Common;
using System;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class NumberValueLinearResolver : BaseResolver
    {
        public NumberValueLinearResolver(IMainStorageContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            _fuzzyLogicResolver = _context.DataResolversFactory.GetFuzzyLogicResolver();
        }

        private FuzzyLogicResolver _fuzzyLogicResolver;

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();

        public bool CanBeResolved(IMonitorLogger logger, Value source)
        {
            if(source == null)
            {
                return true;
            }

            switch (source.KindOfValue)
            {
                case KindOfValue.NullValue:
                case KindOfValue.LogicalValue:
                case KindOfValue.NumberValue:
                    return true;

                default:
                    return false;
            }
        }

        public NumberValue Resolve(IMonitorLogger logger, Value source, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(logger, source, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(IMonitorLogger logger, Value source, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if(source == null)
            {
                return ValueConverter.ConvertNullValueToNumberValue(logger, NullValue.Instance, _context);
            }

            var sourceKind = source.KindOfValue;

            switch (sourceKind)
            {
                case KindOfValue.NullValue:
                    return ValueConverter.ConvertNullValueToNumberValue(logger, source.AsNullValue, _context);

                case KindOfValue.LogicalValue:
                    return ValueConverter.ConvertLogicalValueToNumberValue(logger, source.AsLogicalValue, _context);

                case KindOfValue.NumberValue:
                    return source.AsNumberValue;

                case KindOfValue.StrongIdentifierValue:
                    {
                        ReasonOfFuzzyLogicResolving reasonOfFuzzyLogicResolving = null;

                        return _fuzzyLogicResolver.Resolve(logger, source.AsStrongIdentifierValue, reasonOfFuzzyLogicResolving, localCodeExecutionContext);
                    }

                case KindOfValue.FuzzyLogicNonNumericSequenceValue:
                    {
                        ReasonOfFuzzyLogicResolving reasonOfFuzzyLogicResolving = null;

                        return _fuzzyLogicResolver.Resolve(logger, source.AsFuzzyLogicNonNumericSequenceValue, reasonOfFuzzyLogicResolving, localCodeExecutionContext);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(sourceKind), sourceKind, null);
            }
        }
    }
}
