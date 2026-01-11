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
using SymOntoClay.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters.Helpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SymOntoClay.Core;
using SymOntoClay.Monitor.Common;

namespace SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters
{
    [PlatformTypesConverter]
    public class EntityAndStrongIdentifierValueConverter : BasePlatformTypesConverter
    {
        /// <inheritdoc/>
        public override Type PlatformType => typeof(IEntity);

        /// <inheritdoc/>
        public override Type CoreType => typeof(StrongIdentifierValue);

        /// <inheritdoc/>
        public override bool CanConvertToPlatformType => true;

        /// <inheritdoc/>
        public override bool CanConvertToCoreType => false;

        /// <inheritdoc/>
        public override object ConvertToCoreType(IMonitorLogger logger, object platformObject, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
            throw new NotImplementedException("3C2B0500-B3D5-462C-8E55-43F3A58A6D66");
        }

        /// <inheritdoc/>
        public override object ConvertToPlatformType(IMonitorLogger logger, object coreObject, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
            var identifier = (StrongIdentifierValue)coreObject;

            var kindOfName = identifier.KindOfName;

            switch (kindOfName)
            {
                case KindOfName.Entity:
                    return ConvertEntityToPlatformType(logger, identifier, context, localContext);

                case KindOfName.CommonConcept:
                    return ConvertConceptToPlatformType(logger, identifier, context, localContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
            }
        }

        private object ConvertEntityToPlatformType(IMonitorLogger logger, StrongIdentifierValue identifier, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
            var entityValue = PlatformTypesConverterHelper.GetResolvedEntityValue(logger, identifier, context, localContext);

            return entityValue;
        }

        private object ConvertConceptToPlatformType(IMonitorLogger logger, StrongIdentifierValue concept, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
            var conditionalEntityValue = PlatformTypesConverterHelper.GetResolvedConditionalEntityValue(logger, concept, context, localContext);

            return conditionalEntityValue;
        }
    }
}
