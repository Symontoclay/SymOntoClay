using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace SymOntoClay.Core.Internal.Converters
{
    public class TypeConverter: BaseContextComponent, ITypeConverter
    {
        public TypeConverter(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IMainStorageContext _context;

        private InheritanceResolver _inheritanceResolver;
        private LogicalSearchResolver _logicalSearchResolver;
        private FuzzyLogicResolver _fuzzyLogicResolver;

        private StrongIdentifierValue _anyTypeName;
        private StrongIdentifierValue _booleanTypeName;
        private StrongIdentifierValue _fuzzyTypeName;
        private StrongIdentifierValue _numberTypeName;

        private IList<StrongIdentifierValue> _emptyTypesList = new List<StrongIdentifierValue>();

        private TypeFitCheckingResult _needConversionToBooleanTypeFitCheckingResult;        

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            var dataResolversFactory = _context.DataResolversFactory;

            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();
            _logicalSearchResolver = dataResolversFactory.GetLogicalSearchResolver();
            _fuzzyLogicResolver = dataResolversFactory.GetFuzzyLogicResolver();

            var commonNamesStorage = _context.CommonNamesStorage;

            _anyTypeName = commonNamesStorage.AnyTypeName;
            _booleanTypeName = commonNamesStorage.BooleanTypeName;
            _fuzzyTypeName = commonNamesStorage.FuzzyTypeName;
            _numberTypeName = commonNamesStorage.NumberTypeName;

            _needConversionToBooleanTypeFitCheckingResult = new TypeFitCheckingResult(KindOfTypeFitCheckingResult.NeedConversion, _booleanTypeName);
        }

        /// <inheritdoc/>
        public ResolverOptions DefaultOptions => _defaultOptions;

        /// <inheritdoc/>
        public int GetCapacity(IMonitorLogger logger, IList<StrongIdentifierValue> typesList)
        {
            if(typesList.IsNullOrEmpty())
            {
                return 1;
            }

            return typesList.Min(p => p.Capacity ?? 1);
        }

        /// <inheritdoc/>
        public ValueCallResult CheckAndTryConvert(IMonitorLogger logger, Value value, IList<StrongIdentifierValue> typesList, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return CheckAndTryConvert(logger, value, typesList, localCodeExecutionContext, DefaultOptions);
        }

        /// <inheritdoc/>
        public ValueCallResult CheckAndTryConvert(IMonitorLogger logger, Value value, IList<StrongIdentifierValue> typesList, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Info("50526CB3-643C-4602-8B17-58CB31D522CA", $"value = {value?.ToHumanizedString()}");
            //Info("95E681A8-1612-4252-B477-F70B51097823", $"typesList = {typesList.WriteListToString()}");
#endif

            var checkingResult = CheckFitValue(logger, value, typesList, localCodeExecutionContext, options);

#if DEBUG
            //Info("AB5669E1-4B92-45A8-A562-5162241198CD", $"checkingResult = {checkingResult}");
#endif

            return TryConvertToCallResult(logger, value, typesList, checkingResult, localCodeExecutionContext, options);
        }

        private string BuildIsNotFitErrorMessage(Value value, IList<StrongIdentifierValue> typesList)
        {
            return $"The value '{value.ToHumanizedString()}' does not fit to type {typesList.TypesListToHumanizedString()}";
        }

        /// <inheritdoc/>
        public Value TryConvertToValue(IMonitorLogger logger, Value value, TypeFitCheckingResult checkingResult, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return TryConvertToValue(logger, value, _emptyTypesList, checkingResult, localCodeExecutionContext, DefaultOptions);
        }

        /// <inheritdoc/>
        public Value TryConvertToValue(IMonitorLogger logger, Value value, IList<StrongIdentifierValue> typesList, TypeFitCheckingResult checkingResult, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return TryConvertToValue(logger, value, typesList, checkingResult, localCodeExecutionContext, DefaultOptions);
        }

        /// <inheritdoc/>
        public Value TryConvertToValue(IMonitorLogger logger, Value value, IList<StrongIdentifierValue> typesList, TypeFitCheckingResult checkingResult, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var kindOfResult = checkingResult.KindOfResult;

            switch (kindOfResult)
            {
                case KindOfTypeFitCheckingResult.IsFit:
                    return value;

                case KindOfTypeFitCheckingResult.NeedConversion:
                    {
                        var conversionResult = Convert(logger, value, checkingResult.SuggestedType, localCodeExecutionContext, options);

                        if (conversionResult == null)
                        {
                            throw new Exception($"7B409E1B-0273-40AE-A8F4-4720C93DEBC3: {BuildIsNotFitErrorMessage(value, typesList)}");
                        }

                        return conversionResult;
                    }

                case KindOfTypeFitCheckingResult.IsNotFit:
                    throw new Exception($"435AEB0F-4EB7-4219-89D3-D63871296755: {BuildIsNotFitErrorMessage(value, typesList)}");

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfResult), kindOfResult, null);
            }
        }

        /// <inheritdoc/>
        public ValueCallResult TryConvertToCallResult(IMonitorLogger logger, Value value, IList<StrongIdentifierValue> typesList, TypeFitCheckingResult checkingResult, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return TryConvertToCallResult(logger, value, typesList, checkingResult, localCodeExecutionContext, DefaultOptions);
        }

        /// <inheritdoc/>
        public ValueCallResult TryConvertToCallResult(IMonitorLogger logger, Value value, IList<StrongIdentifierValue> typesList, TypeFitCheckingResult checkingResult, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            return new ValueCallResult(TryConvertToValue(logger, value, typesList, checkingResult, localCodeExecutionContext, options));
        }

        /// <inheritdoc/>
        public TypeFitCheckingResult CheckFitValue(IMonitorLogger logger, Value value, IList<StrongIdentifierValue> typesList, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return CheckFitValue(logger, value, typesList, localCodeExecutionContext, DefaultOptions);
        }

        /// <inheritdoc/>
        public TypeFitCheckingResult CheckFitValue(IMonitorLogger logger, Value value, IList<StrongIdentifierValue> typesList, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Info("F003F8DC-8153-42A6-86D5-9F544A627690", $"value = {value.ToHumanizedString()}");
            //Info("569F4890-E159-4A87-BDCE-03F5D9F039FB", $"typesList = {typesList.WriteListToString()}");
#endif

            if (value.IsNullValue)
            {
                return TypeFitCheckingResult.Fit;
            }

            if (typesList.IsNullOrEmpty())
            {
                return TypeFitCheckingResult.Fit;
            }

            if (typesList.Any(p => p == _anyTypeName))
            {
                return TypeFitCheckingResult.Fit;
            }

            if (typesList.Any(p => p == _booleanTypeName))
            {
                if(value.IsRuleInstance && value.AsRuleInstance.KindOfRuleInstance == KindOfRuleInstance.Fact)
                {
                    return _needConversionToBooleanTypeFitCheckingResult;
                }
            }

            if (typesList.Any(p => p == _numberTypeName))
            {
                if (value.IsStrongIdentifierValue)
                {
                    return new TypeFitCheckingResult(KindOfTypeFitCheckingResult.NeedConversion, _numberTypeName);
                }
            }

            if (typesList.Any(p => p == _fuzzyTypeName))
            {
                if (value.IsStrongIdentifierValue)
                {
                    return new TypeFitCheckingResult(KindOfTypeFitCheckingResult.NeedConversion, _fuzzyTypeName);
                }
            }

            var isFit = _inheritanceResolver.IsFit(logger, typesList, value, localCodeExecutionContext, options);

            if (isFit)
            {
                return TypeFitCheckingResult.Fit;
            }

            //throw new Exception($"The value '{value.ToHumanizedString()}' does not fit to variable {CodeItem.ToHumanizedString()}");

            //throw new NotImplementedException("11924B32-F3D5-4CC4-93FA-D14C239F27C5");

            return TypeFitCheckingResult.IsNotFit;
        }

        /// <inheritdoc/>
        public Value Convert(IMonitorLogger logger, Value value, StrongIdentifierValue targetType, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Convert(logger, value, targetType, localCodeExecutionContext, DefaultOptions);
        }

        /// <inheritdoc/>
        public Value Convert(IMonitorLogger logger, Value value, StrongIdentifierValue targetType, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Info("772AFCFF-0704-4AC2-A3C4-8BEF21681070", $"value = {value}");
            //Info("FB99DB07-9E1E-4904-8DAB-0E419C84C716", $"targetType = {targetType}");
#endif

            var normalizedTypeName = targetType.NormalizedNameValue;

            switch (normalizedTypeName)
            {
                case StandardNamesConstants.BooleanTypeName:
                    if(value.IsRuleInstance)
                    {
                        var ruleInstance = value.AsRuleInstance;

                        if(ruleInstance.KindOfRuleInstance == KindOfRuleInstance.Fact)
                        {
                            var searchOptions = new LogicalSearchOptions();
                            searchOptions.QueryExpression = ruleInstance;
                            searchOptions.LocalCodeExecutionContext = localCodeExecutionContext;

                            var isTruth = _logicalSearchResolver.IsTruth(logger, searchOptions);

#if DEBUG
                            //Info("EEE1B871-1108-423A-8E78-5A71E9C2A481", $"isTruth = {isTruth}");
#endif

                            return new LogicalValue(isTruth);
                        }
                        else
                        {
                            throw new NotImplementedException("CBFE55DB-8776-4375-9B84-A46C38AB06E5");
                        }                            
                    }
                    else
                    {
                        throw new NotImplementedException("30CA5B09-9171-4873-8893-DB3746C05036");
                    }

                case StandardNamesConstants.NumberTypeName:
                    if(value.IsStrongIdentifierValue)
                    {
                        return Defuzzificate(logger, value.AsStrongIdentifierValue.ForResolving, localCodeExecutionContext, options);
                    }
                    else
                    {
                        throw new NotImplementedException("34E25373-EA53-4471-9015-8A824505931D");
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(normalizedTypeName), normalizedTypeName, null);
            }
        }

        private NumberValue Defuzzificate(IMonitorLogger logger, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var targetFuzzyLogicItem = _fuzzyLogicResolver.GetTargetFuzzyLogicNonNumericValue(logger, name, null, null, localCodeExecutionContext, options);

            if (targetFuzzyLogicItem == null)
            {
                return null;
            }

            return _fuzzyLogicResolver.DefuzzificateTargetFuzzyLogicNonNumericValue(logger, targetFuzzyLogicItem);
        }
    }
}
