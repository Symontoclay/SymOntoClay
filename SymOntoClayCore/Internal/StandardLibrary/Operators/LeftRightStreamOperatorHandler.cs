using NLog.Fluent;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
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
        public Value Call(Value leftOperand, Value rightOperand, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"leftOperand = {leftOperand}");
            //Log($"rightOperand = {rightOperand}");
#endif

            Value valueFromSource = null;

            if(leftOperand.KindOfValue == KindOfValue.IdentifierValue)
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

            if(rightOperand.KindOfValue == KindOfValue.IdentifierValue)
            {
                var identifier = rightOperand.AsIdentifierValue;

#if DEBUG
                //Log($"identifier = {identifier}");
#endif

                if(identifier.KindOfName == KindOfName.Channel)
                {
                    var channel = _channelsResolver.GetChannel(identifier, localCodeExecutionContext, ResolverOptions.GetDefaultFluentOptions());

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
