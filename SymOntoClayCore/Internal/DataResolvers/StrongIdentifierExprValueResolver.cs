using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class StrongIdentifierExprValueResolver : BaseResolver
    {
        public StrongIdentifierExprValueResolver(IMainStorageContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            var dataResolversFactory = _context.DataResolversFactory;
            _propertiesResolver = dataResolversFactory.GetPropertiesResolver();
            _fuzzyLogicResolver = dataResolversFactory.GetFuzzyLogicResolver();
        }

        private PropertiesResolver _propertiesResolver;
        private FuzzyLogicResolver _fuzzyLogicResolver;

        public ResolverOptions DefaultOptions { get; private set; } = ResolverOptions.GetDefaultOptions();

        public CallResult GetValue(IMonitorLogger logger, StrongIdentifierValue name, IInstance instance, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetValue(logger, name, instance, localCodeExecutionContext, DefaultOptions);
        }

        public CallResult GetValue(IMonitorLogger logger, StrongIdentifierValue name, IInstance instance, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Info("C83986EF-4A7C-4ED8-8CA1-C9F39C04815A", $"name = {name}");
#endif

            var property = _propertiesResolver.Resolve(logger, name, localCodeExecutionContext, options);

#if DEBUG
            Info("E16D50F3-4AC5-4E80-BCF9-AB72FE95A6CE", $"property?.KindOfProperty = {property?.KindOfProperty}");
#endif

            if(property != null)
            {
                return _propertiesResolver.ConvertPropertyInstanceToCallResult(property);
            }

            var targetFuzzyLogicItem = _fuzzyLogicResolver.GetTargetFuzzyLogicNonNumericValue(logger, name, null, null, localCodeExecutionContext, options);

            if (targetFuzzyLogicItem != null)
            {
                return new CallResult(_fuzzyLogicResolver.DefuzzificateTargetFuzzyLogicNonNumericValue(logger, targetFuzzyLogicItem));
            }



            throw new NotImplementedException();
        }
    }
}
