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
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class LeftRightStreamOperatorHandler: BaseLoggedComponent, IBinaryOperatorHandler
    {
        public LeftRightStreamOperatorHandler(IEngineContext engineContext)
            : base(engineContext.Logger)
        {
            _engineContext = engineContext;
            _channelsResolver = engineContext.DataResolversFactory.GetChannelsResolver();
        }

        private readonly IEngineContext _engineContext;
        private readonly ChannelsResolver _channelsResolver;

        /// <inheritdoc/>
        public IndexedValue Call(IndexedValue leftOperand, IndexedValue rightOperand, IndexedValue annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"leftOperand = {leftOperand}");
            //Log($"rightOperand = {rightOperand}");
            //Log($"annotation = {annotation}");
#endif

            IndexedValue valueFromSource = null;

            if(leftOperand.KindOfValue == KindOfValue.StrongIdentifierValue)
            {
                throw new NotImplementedException();
            }
            else
            {
                valueFromSource = leftOperand;
            }

#if DEBUG
            //Log($"valueFromSource = {valueFromSource}");
#endif

            if(rightOperand.KindOfValue == KindOfValue.StrongIdentifierValue)
            {
                var identifier = rightOperand.AsStrongIdentifierValue;

#if DEBUG
                //Log($"identifier = {identifier}");
#endif

                if(identifier.KindOfName == KindOfName.Channel)
                {
                    var channel = _channelsResolver.GetChannel(identifier, localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

#if DEBUG
                    //Log($"channel = {channel}");
#endif

                    return channel.Handler.Write(valueFromSource);
                }
                else
                {
                    throw new Exception("Right operand should be a channel.");
                }
            }
            else
            {
                throw new Exception("Right operand should be a channel.");
            }          
        }
    }
}
