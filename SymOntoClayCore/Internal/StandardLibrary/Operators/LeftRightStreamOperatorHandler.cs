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
