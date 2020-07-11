using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public interface IDataResolversFactory
    {
        ChannelsResolver GetChannelsResolver();
        InheritanceResolver GetInheritanceResolver();
        LogicalValueLinearResolver GetLogicalValueLinearResolver();
        OperatorsResolver GetOperatorsResolver();
        NumberValueLinearResolver GetNumberValueLinearResolver();
        TriggersResolver GetTriggersResolver();
    }
}
