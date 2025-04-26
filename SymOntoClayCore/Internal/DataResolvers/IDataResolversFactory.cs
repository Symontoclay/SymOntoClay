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

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public interface IDataResolversFactory
    {
        BaseResolver GetBaseResolver();

        /// <summary>
        /// Gets instance of Channels resolver.
        /// </summary>
        /// <returns>Instance of Channels resolver.</returns>
        ChannelsResolver GetChannelsResolver();

        /// <summary>
        /// Gets instance of Inheritance resolver.
        /// </summary>
        /// <returns>Instance of Inheritance resolver.</returns>
        InheritanceResolver GetInheritanceResolver();

        /// <summary>
        /// Gets instance of LogicalValue resolver.
        /// </summary>
        /// <returns>Instance of LogicalValue resolver.</returns>
        LogicalValueLinearResolver GetLogicalValueLinearResolver();

        /// <summary>
        /// Gets instance of Operators resolver.
        /// </summary>
        /// <returns>Instance of Operators resolver.</returns>
        OperatorsResolver GetOperatorsResolver();

        /// <summary>
        /// Gets instance of NumberValue linear resolver.
        /// </summary>
        /// <returns>Instance of NumberValue linear resolver.</returns>
        NumberValueLinearResolver GetNumberValueLinearResolver();

        /// <summary>
        /// Gets instance of StrongIdentifier linear resolver.
        /// </summary>
        /// <returns>Instance of StrongIdentifier linear resolver.</returns>
        StrongIdentifierLinearResolver GetStrongIdentifierLinearResolver();

        /// <summary>
        /// Gets instance of Triggers resolver.
        /// </summary>
        /// <returns>Instance of Triggers resolver.</returns>
        TriggersResolver GetTriggersResolver();

        /// <summary>
        /// Gets instance of Vars resolver.
        /// </summary>
        /// <returns>Instance of Vars resolver.</returns>
        VarsResolver GetVarsResolver();

        PropertiesResolver GetPropertiesResolver();

        /// <summary>
        /// Gets instance of LogicalSearch resolver.
        /// </summary>
        /// <returns>Instance of LogicalSearch resolver.</returns>
        LogicalSearchResolver GetLogicalSearchResolver();

        /// <summary>
        /// Gets instance of FuzzyLogic resolver.
        /// </summary>
        /// <returns>Instance of FuzzyLogic resolver.</returns>
        FuzzyLogicResolver GetFuzzyLogicResolver();

        /// <summary>
        /// Gets instance of Methods resolver.
        /// </summary>
        /// <returns>Instance of Methods resolver.</returns>
        MethodsResolver GetMethodsResolver();

        ConstructorsResolver GetConstructorsResolver();

        /// <summary>
        /// Gets instance of CodeItemDirectives resolver.
        /// </summary>
        /// <returns>Instance of CodeItemDirectives resolver.</returns>
        CodeItemDirectivesResolver GetCodeItemDirectivesResolver();

        /// <summary>
        /// Gets instance of States resolver.
        /// </summary>
        /// <returns>Instance of States resolver.</returns>
        StatesResolver GetStatesResolver();

        ToSystemBoolResolver GetToSystemBoolResolver();

        RelationsResolver GetRelationsResolver();

        LogicalValueModalityResolver GetLogicalValueModalityResolver();

        SynonymsResolver GetSynonymsResolver();

        IdleActionsResolver GetIdleActionsResolver();

        AnnotationsResolver GetAnnotationsResolver();

        ValueResolvingHelper GetValueResolvingHelper();

        MetadataResolver GetMetadataResolver();

        LogicalSearchVarResultsItemInvertor GetLogicalSearchVarResultsItemInvertor();

        DateTimeResolver GetDateTimeResolver();

        StrongIdentifierExprValueResolver GetStrongIdentifierExprValueResolver();
    }
}
