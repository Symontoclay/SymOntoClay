using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CommonNames;
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

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();

            var commonNamesStorage = _context.CommonNamesStorage;

            _trueValueLiteral = commonNamesStorage.TrueValueLiteral;
            _falseValueLiteral = commonNamesStorage.FalseValueLiteral;
        }

        private PropertiesResolver _propertiesResolver;
        private FuzzyLogicResolver _fuzzyLogicResolver;

        private StrongIdentifierValue _trueValueLiteral;
        private StrongIdentifierValue _falseValueLiteral;

        public ResolverOptions DefaultOptions { get; private set; } = ResolverOptions.GetDefaultOptions();

        public CallResult GetValue(IMonitorLogger logger, StrongIdentifierValue name, IInstance instance, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetValue(logger, name, instance, localCodeExecutionContext, DefaultOptions);
        }

        public CallResult GetValue(IMonitorLogger logger, StrongIdentifierValue name, IInstance instance, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Info("C83986EF-4A7C-4ED8-8CA1-C9F39C04815A", $"name = {name}");
#endif

            if(name.IsNullValue)
            {
                return new CallResult(NullValue.Instance);
            }

            var targetFuzzyLogicItem = _fuzzyLogicResolver.GetTargetFuzzyLogicNonNumericValue(logger, name, null, null, localCodeExecutionContext, options);

            if (targetFuzzyLogicItem != null)
            {
                return new CallResult(name);
            }

            var property = _propertiesResolver.Resolve(logger, name, localCodeExecutionContext, options);

#if DEBUG
            //Info("E16D50F3-4AC5-4E80-BCF9-AB72FE95A6CE", $"property?.KindOfProperty = {property?.KindOfProperty}");
#endif

            if(property != null)
            {
                return _propertiesResolver.ConvertPropertyInstanceToCallResult(property);
            }

            if(name == _trueValueLiteral)
            {
                return new CallResult(LogicalValue.TrueValue);
            }

            if(name == _falseValueLiteral)
            {
                return new CallResult(LogicalValue.FalseValue);
            }

            var value = _propertiesResolver.ResolveImplicitProperty(logger, name, instance, localCodeExecutionContext, options);

#if DEBUG
            Info("38CEA52A-DE78-42DA-B03D-B8901AFB6A97", $"value = {value}");
#endif

            if (value == null)
            {
                return new CallResult(name);
            }
            else
            {
                return new CallResult(value);
            }
        }
    }
}
