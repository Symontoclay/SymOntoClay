using SymOntoClay.Common.DebugHelpers;
using System.IO;
using System.Text;

namespace SymOntoClay.Serialization.SmartValues.Functors
{
    public class PathCombineSmartValue : SmartValue<string>
    {
        public PathCombineSmartValue()
        {
        }

        public PathCombineSmartValue(SmartValue<string> path1, SmartValue<string> path2)
        {
            _path1 = path1;
            _path2 = path2;
        }

        private readonly SmartValue<string> _path1;
        private readonly SmartValue<string> _path2;

        /// <inheritdoc/>
        public override string Value => Path.Combine(_path1.Value, _path2.Value);

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintObjProp(n, nameof(_path1), _path1);
            sb.PrintObjProp(n, nameof(_path2), _path2);
            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintShortObjProp(n, nameof(_path1), _path1);
            sb.PrintShortObjProp(n, nameof(_path2), _path2);
            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjProp(n, nameof(_path1), _path1);
            sb.PrintBriefObjProp(n, nameof(_path2), _path2);
            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }
    }
}
