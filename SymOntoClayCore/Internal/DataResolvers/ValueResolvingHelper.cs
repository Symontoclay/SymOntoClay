/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class ValueResolvingHelper : BaseContextComponent
    {
        public ValueResolvingHelper(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            var dataResolversFactory = _context.DataResolversFactory;

            _varsResolver = dataResolversFactory.GetVarsResolver();
            _strongIdentifierExprValueResolver = dataResolversFactory.GetStrongIdentifierExprValueResolver();
        }

        private readonly IMainStorageContext _context;
        private VarsResolver _varsResolver;
        private StrongIdentifierExprValueResolver _strongIdentifierExprValueResolver;

        public ValueCallResult TryResolveFromVarOrExpr(IMonitorLogger logger, Value operand, KindOfValueConversion kindOfValueConversion, ValueResolvingMode valueResolvingMode, ILocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Info("2833B222-F7A1-4588-A63A-612D54D1AB28", $"operand = {operand}");
            //Info("FABDB406-3D13-4A57-961E-DF6B8745409F", $"kindOfValueConversion = {kindOfValueConversion}");
            //Info("3E7BD299-8F4F-4059-A8B4-69511DD5B0D7", $"valueResolvingMode = {valueResolvingMode}");
#endif

            var kindOfValue = operand.KindOfValue;

#if DEBUG
            //Info("387ABF8B-71F9-44F3-B4D5-504FCDB930EF", $"kindOfValue = {kindOfValue}");
#endif

            switch(kindOfValue)
            {
                case KindOfValue.StrongIdentifierValue:
                    {
                        var identifier = operand.AsStrongIdentifierValue;

                        var kindOfName = identifier.KindOfName;

#if DEBUG
                        //Info("9347D620-94CC-48AA-B07A-3B49252A9236", $"kindOfName = {kindOfName}");
                        //Info("8FAE3943-8A07-47AE-AD56-EFBEE868C00B", $"localCodeExecutionContext.Instance?.Name = {localCodeExecutionContext.Instance?.Name}");
                        //if(localCodeExecutionContext.Instance == null)
                        //{
                        //    localCodeExecutionContext.DbgPrintContextChain(logger, "9CF526E2-9860-49E2-9067-CD554BD8CEB6");
                        //}
#endif
                        
                        switch (kindOfName)
                        {
                            case KindOfName.Var:
                            case KindOfName.SystemVar:
                                if(kindOfValueConversion.HasFlag(KindOfValueConversion.All) ||
                                    kindOfValueConversion.HasFlag(KindOfValueConversion.Var))
                                {
                                    return new ValueCallResult(_varsResolver.GetVarValue(logger, identifier, localCodeExecutionContext));
                                }
                                else
                                {
                                    return new ValueCallResult(operand);
                                }
                                
                            case KindOfName.CommonConcept:
                            case KindOfName.LinguisticVar:
                            case KindOfName.Property:
                                return _strongIdentifierExprValueResolver.GetValue(logger, identifier, kindOfValueConversion, localCodeExecutionContext.Instance, localCodeExecutionContext);
                                
                            case KindOfName.Entity:
                                return new ValueCallResult(operand);

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
                        }
                    }

                case KindOfValue.MemberValue:
                    {
                        switch(valueResolvingMode)
                        {
                            case ValueResolvingMode.Usual:
                            case ValueResolvingMode.Parameter:
                                {
                                    var memberValue = operand.AsMemberValue;

                                    return memberValue.GetValue(logger);
                                }

                            case ValueResolvingMode.Caller:
                                {
                                    var memberValue = operand.AsMemberValue;

                                    var kindOfMember = memberValue.KindOfMember;

#if DEBUG
                                    //Info("48A11334-C950-4AA7-81B1-E8EDC44A2A5B", $"kindOfMember = {kindOfMember}");
#endif

                                    switch(kindOfMember)
                                    {
                                        case Instances.KindOfMember.HostMethod:
                                            return new ValueCallResult(operand);

                                        default:
                                            return memberValue.GetValue(logger);
                                    }
                                }

                            default:
                                throw new ArgumentOutOfRangeException(nameof(valueResolvingMode), valueResolvingMode, null);
                        }
                    }

                default:
                    return new ValueCallResult(operand);
            }
        }
    }
}
