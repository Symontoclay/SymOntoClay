/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Convertors
{
    public static class ValueConvertor
    {
        public static LogicalValue ConvertNullValueToLogicalValue(NullValue source, IMainStorageContext mainStorageContext)
        {
            var result = new LogicalValue(null);

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext);

            return result;
        }

        public static LogicalValue ConvertNumberValueToLogicalValue(NumberValue source, IMainStorageContext mainStorageContext)
        {
            var result = new LogicalValue((float?)source.AsNumberValue.SystemValue);

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext);

            return result;
        }

        public static NumberValue ConvertNullValueToNumberValue(NullValue source, IMainStorageContext mainStorageContext)
        {
            var result = new NumberValue(null);

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext);

            return result;
        }

        public static NumberValue ConvertLogicalValueToNumberValue(LogicalValue source, IMainStorageContext mainStorageContext)
        {
            var result = new NumberValue(source.AsLogicalValue.SystemValue);

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext);

            return result;
        }

        private static void FillAnnotationsModalitiesAndSections(AnnotatedItem source, AnnotatedItem dest, IMainStorageContext mainStorageContext)
        {
            throw new NotImplementedException();
        }
    }
}
