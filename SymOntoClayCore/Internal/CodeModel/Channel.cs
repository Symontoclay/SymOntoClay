using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class Channel : AnnotatedItem
    {
        public Name Name { get; set; }
        public IChannelHandler Handler { get; set; }

        public IndexedChannel Indexed { get; set; }

        public IndexedChannel GetIndexed(IEntityDictionary entityDictionary)
        {
            if (Indexed == null)
            {
                return ConvertorToIndexed.ConvertChannel(this, entityDictionary);
            }

            return Indexed;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Name), Name);
            sb.PrintExisting(n, nameof(Handler), Handler);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Name), Name);
            sb.PrintExisting(n, nameof(Handler), Handler);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Name), Name);
            sb.PrintExisting(n, nameof(Handler), Handler);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
