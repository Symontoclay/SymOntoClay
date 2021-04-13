/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using NLog.Fluent;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalValueLinearResolver: BaseResolver
    {
        public LogicalValueLinearResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public LogicalValue Resolve(Value source, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            return Resolve(source, localCodeExecutionContext, options, false);
        }

        public LogicalValue Resolve(Value source, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options, bool forInheritance)
        {
#if DEBUG
            //Log($"source = {source}");
#endif

            var sourceKind = source.KindOfValue;

            switch(sourceKind)
            {
                case KindOfValue.NullValue:
                    return ValueConvertor.ConvertNullValueToLogicalValue(source.AsNullValue, _context);

                case KindOfValue.LogicalValue:
                    return source.AsLogicalValue;

                case KindOfValue.NumberValue:
                    return ValueConvertor.ConvertNumberValueToLogicalValue(source.AsNumberValue, _context);

                case KindOfValue.StrongIdentifierValue:
                    ReasonOfFuzzyLogicResolving reasonOfFuzzyLogicResolving = null;
                    
                    if(forInheritance)
                    {
                        reasonOfFuzzyLogicResolving = new ReasonOfFuzzyLogicResolving() { Kind = KindOfReasonOfFuzzyLogicResolving.Inheritance };
                    }
                    
                    return ValueConvertor.ConvertNumberValueToLogicalValue(_context.DataResolversFactory.GetFuzzyLogicResolver().Resolve(source.AsStrongIdentifierValue, reasonOfFuzzyLogicResolving, localCodeExecutionContext, options), _context);

                default:
                    throw new ArgumentOutOfRangeException(nameof(sourceKind), sourceKind, null);
            }
        }
    }
}
