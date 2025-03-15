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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System;

namespace SymOntoClay.Core.Internal.Converters
{
    public static class ValueConverter
    {

        public static LogicalValue ConvertNullValueToLogicalValue(IMonitorLogger logger, NullValue source, IMainStorageContext mainStorageContext)
        {
            var result = new LogicalValue(null);

            FillAnnotationsModalitiesAndSections(logger, source, result, mainStorageContext);

            return result;
        }

        public static LogicalValue ConvertNumberValueToLogicalValue(IMonitorLogger logger, NumberValue source, IMainStorageContext mainStorageContext)
        {
            var result = new LogicalValue((float?)source.AsNumberValue.SystemValue);

            FillAnnotationsModalitiesAndSections(logger, source, result, mainStorageContext);

            return result;
        }

        public static LogicalValue ConvertStrongIdentifierValueToLogicalValue(IMonitorLogger logger, StrongIdentifierValue source, IMainStorageContext mainStorageContext)
        {
            var normalizedNameValue = source.NormalizedNameValue;

            switch (normalizedNameValue)
            {
                case "true":
                    return new LogicalValue(1);

                case "false":
                    return new LogicalValue(0);

                default:
                    throw new NotImplementedException("3A4265C3-6A2A-467A-B207-58BA3E048258");
            }
        }

        public static NumberValue ConvertNullValueToNumberValue(IMonitorLogger logger, NullValue source, IMainStorageContext mainStorageContext)
        {
            var result = new NumberValue(null);

            FillAnnotationsModalitiesAndSections(logger, source, result, mainStorageContext);

            return result;
        }

        public static NumberValue ConvertLogicalValueToNumberValue(IMonitorLogger logger, LogicalValue source, IMainStorageContext mainStorageContext)
        {
            var result = new NumberValue(source.AsLogicalValue.SystemValue);

            FillAnnotationsModalitiesAndSections(logger, source, result, mainStorageContext);

            return result;
        }

        private static void FillAnnotationsModalitiesAndSections(IMonitorLogger logger, AnnotatedItem source, AnnotatedItem dest, IMainStorageContext mainStorageContext)
        {
            dest.AppendAnnotations(source);
        }
    }
}
