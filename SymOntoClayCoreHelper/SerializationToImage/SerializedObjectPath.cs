using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class SerializedObjectPath: IObjectToString, IObjectToBriefString
    {
#if DEBUG
        //private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public string Name { get; private set; }
        public SerializedObjectPath Parent { get; private set; }
        public SerializedObjectPath Child { get; private set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            sb.PrintBriefObjProp(n, nameof(Parent), Parent);
            sb.PrintObjProp(n, nameof(Child), Child);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            return PropertiesToBriefString(n);
        }

        protected virtual string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            sb.PrintExisting(n, nameof(Parent), Parent);
            sb.PrintExisting(n, nameof(Child), Child);

            return sb.ToString();
        }

        public static SerializedObjectPath Parse(string path)
        {
            var pathChunksList = path.Split('/');

            var wasRoot = false;

            SerializedObjectPath root = null;
            SerializedObjectPath node = null;
            SerializedObjectPath prevNode = null;

            foreach (var chunk in pathChunksList)
            {
                node = new SerializedObjectPath() 
                { 
                    Name = chunk,
                    Parent = prevNode
                };

                if(prevNode != null)
                {
                    prevNode.Child = node;
                }

                if (!wasRoot)
                {
                    wasRoot = true;
                    root = node;
                }

                prevNode = node;
            }

            return root;
        }
    }
}
