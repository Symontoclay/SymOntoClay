using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers.Exceptions;
using SymOntoClay.Core.Internal.Instances;
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
            _varsResolver = dataResolversFactory.GetVarsResolver();
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
        private VarsResolver _varsResolver;

        private StrongIdentifierValue _trueValueLiteral;
        private StrongIdentifierValue _falseValueLiteral;

        public ResolverOptions DefaultOptions { get; private set; } = ResolverOptions.GetDefaultOptions();

        public IMember GetMember(IMonitorLogger logger, StrongIdentifierValue name, IInstance instance, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetMember(logger, name, instance, localCodeExecutionContext, DefaultOptions);
        }

        public IMember GetMember(IMonitorLogger logger, StrongIdentifierValue name, IInstance instance, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Info("4E645F88-DA47-41E1-8448-CC9B460FF617", $"name = {name}");
#endif

            var kindOfName = name.KindOfName;

            switch (kindOfName)
            {
                case KindOfName.Var:
                    return _varsResolver.Resolve(logger, name, localCodeExecutionContext, options);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
            }
        }

        public ValueCallResult GetValue(IMonitorLogger logger, StrongIdentifierValue name, IInstance instance, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetValue(logger, name, instance, localCodeExecutionContext, DefaultOptions);
        }

        public ValueCallResult GetValue(IMonitorLogger logger, StrongIdentifierValue name, IInstance instance, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Info("C83986EF-4A7C-4ED8-8CA1-C9F39C04815A", $"name = {name}");
#endif

            if(name.IsNullValue)
            {
                return new ValueCallResult(NullValue.Instance);
            }

            var kindOfName = name.KindOfName;

            switch(kindOfName)
            {
                case KindOfName.CommonConcept:
                    return GetValueFromCommonConcept(logger, name, instance, localCodeExecutionContext, options);

                case KindOfName.LinguisticVar:
                    return GetValueFromLinguisticVar(logger, name, instance, localCodeExecutionContext, options);

                case KindOfName.Property:
                    return GetValueFromProperty(logger, name, instance, localCodeExecutionContext, options);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
            }
        }

        private ValueCallResult GetValueFromProperty(IMonitorLogger logger, StrongIdentifierValue name, IInstance instance, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var property = _propertiesResolver.Resolve(logger, name.ForResolving, localCodeExecutionContext, options);

#if DEBUG
            //Info("1CD4D730-2013-4CBF-B172-915D0F36AA8A", $"property?.KindOfProperty = {property?.KindOfProperty}");
#endif

            if(property == null)
            {
                var value = _propertiesResolver.ResolveImplicitProperty(logger, name.ForResolving, instance, localCodeExecutionContext, options);

#if DEBUG
                //Info("0E381D35-017E-4C24-A0B1-368416D1422C", $"value = {value}");
#endif

                if (value == null)
                {
                    return new ValueCallResult(name);
                }
                else
                {
                    return new ValueCallResult(value);
                }
            }

            return _propertiesResolver.ConvertPropertyInstanceToCallResult(property);
        }

        private ValueCallResult GetValueFromLinguisticVar(IMonitorLogger logger, StrongIdentifierValue name, IInstance instance, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var targetFuzzyLogicItem = _fuzzyLogicResolver.GetTargetFuzzyLogicNonNumericValue(logger, name.ForResolving, null, null, localCodeExecutionContext, options);

#if DEBUG
            //Info("5CE2BC84-0769-4A58-A9CB-D657D170F682", $"targetFuzzyLogicItem != null = {targetFuzzyLogicItem != null}");
#endif

            if(targetFuzzyLogicItem == null)
            {
                throw new UnresolvedLinguisticVariableException(name);
            }

            return new ValueCallResult(name);
        }

        private ValueCallResult GetValueFromCommonConcept(IMonitorLogger logger, StrongIdentifierValue name, IInstance instance, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var targetFuzzyLogicItem = _fuzzyLogicResolver.GetTargetFuzzyLogicNonNumericValue(logger, name, null, null, localCodeExecutionContext, options);

#if DEBUG
            //Info("0D08A130-16C5-4906-8E76-F0AFC9AB94EA", $"targetFuzzyLogicItem != null = {targetFuzzyLogicItem != null}");
#endif

            var property = _propertiesResolver.Resolve(logger, name, localCodeExecutionContext, options);

#if DEBUG
            //Info("E16D50F3-4AC5-4E80-BCF9-AB72FE95A6CE", $"property?.KindOfProperty = {property?.KindOfProperty}");
#endif

            if (targetFuzzyLogicItem != null && property != null)
            {
                throw new AmbiguousDataResolvingException(name);
            }

            if (targetFuzzyLogicItem != null && (name == _trueValueLiteral || name == _falseValueLiteral))
            {
                throw new AmbiguousDataResolvingException(name);
            }

            if (property != null && (name == _trueValueLiteral || name == _falseValueLiteral))
            {
                throw new AmbiguousDataResolvingException(name);
            }

            if (targetFuzzyLogicItem != null)
            {
                return new ValueCallResult(name);
            }

            if (property != null)
            {
                return _propertiesResolver.ConvertPropertyInstanceToCallResult(property);
            }

            if (name == _trueValueLiteral)
            {
                return new ValueCallResult(LogicalValue.TrueValue);
            }

            if (name == _falseValueLiteral)
            {
                return new ValueCallResult(LogicalValue.FalseValue);
            }

            var value = _propertiesResolver.ResolveImplicitProperty(logger, name, instance, localCodeExecutionContext, options);

#if DEBUG
            //Info("38CEA52A-DE78-42DA-B03D-B8901AFB6A97", $"value = {value}");
#endif

            if (value == null)
            {
                return new ValueCallResult(name);
            }
            else
            {
                return new ValueCallResult(value);
            }
        }
    }
}
