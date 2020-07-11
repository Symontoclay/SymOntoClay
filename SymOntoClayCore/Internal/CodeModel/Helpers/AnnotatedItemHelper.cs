using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Helpers
{
    public static class AnnotatedItemHelper
    {
        public static void CheckAndFillHolder(AnnotatedItem item, ICommonNamesStorage commonNamesStorage)
        {
            if (item.Holder == null)
            {
                item.Holder = commonNamesStorage.DefaultHolder;
            }
        }

        public static void CheckAndFillHolder(IndexedAnnotatedItem item, ICommonNamesStorage commonNamesStorage)
        {
            if (item.Holder == null)
            {
                item.Holder = commonNamesStorage.DefaultHolder;
            }
        }
    }
}
