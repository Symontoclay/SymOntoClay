using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class WaypointValue : Value
    {
        public WaypointValue(Vector2 relativeCoordinates, IEngineContext context)
        {
            _context = context;

            throw new NotImplementedException();
        }

        private WaypointValue(Vector3 abcoluteCoordinates, IEngineContext context)
        {
            AbcoluteCoordinates = abcoluteCoordinates;
            _context = context;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.WaypointValue;

        /// <inheritdoc/>
        public override bool IsWaypointValue => true;

        /// <inheritdoc/>
        public override WaypointValue AsWaypointValue => this;

        public Vector3 AbcoluteCoordinates { get; private set; }
        private IEngineContext _context;

        public IndexedWaypointValue Indexed { get; set; }

        public IndexedWaypointValue GetIndexed(IMainStorageContext mainStorageContext)
        {
            if (Indexed == null)
            {
                return ConvertorToIndexed.ConvertWaypointValue(this, mainStorageContext);
            }

            return Indexed;
        }

        /// <inheritdoc/>
        public override IndexedAnnotatedItem IndexedAnnotatedItem => Indexed;

        /// <inheritdoc/>
        public override IndexedValue GetIndexedValue(IMainStorageContext mainStorageContext)
        {
            return GetIndexed(mainStorageContext);
        }

        /// <inheritdoc/>
        public override IndexedAnnotatedItem GetIndexedAnnotatedItem(IMainStorageContext mainStorageContext)
        {
            return GetIndexed(mainStorageContext);
        }

        /// <inheritdoc/>
        public override IndexedAnnotatedItem GetIndexedAnnotatedItem(IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (Indexed == null)
            {
                return ConvertorToIndexed.ConvertWaypointValue(this, mainStorageContext, convertingContext);
            }

            return Indexed;
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return null;
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneValue(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> cloneContext)
        {
            if (cloneContext.ContainsKey(this))
            {
                return (Value)cloneContext[this];
            }

            var result = new WaypointValue(AbcoluteCoordinates, _context);
            cloneContext[this] = result;

            result.AppendAnnotations(this, cloneContext);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            
            sb.AppendLine($"{spaces}{nameof(AbcoluteCoordinates)} = {AbcoluteCoordinates}");

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(AbcoluteCoordinates)} = {AbcoluteCoordinates}");

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(AbcoluteCoordinates)} = {AbcoluteCoordinates}");

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
