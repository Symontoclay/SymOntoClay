using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class Command: ICommand
    {
        /// <inheritdoc/>
        public StrongIdentifierValue Name { get; set; }

        /// <inheritdoc/>
        public IList<Value> ParamsList { get; set; }

        /// <inheritdoc/>
        public IDictionary<StrongIdentifierValue, Value> ParamsDict { get; set; }

        /// <inheritdoc/>
        public int ParamsCount
        {
            get
            {
                if(ParamsList != null)
                {
                    return ParamsList.Count;
                }

                if(ParamsDict != null)
                {
                    return ParamsDict.Count;
                }

                return 0;
            }
        }

        /// <inheritdoc/>
        public KindOfCommandParameters KindOfCommandParameters
        {
            get
            {
                if (ParamsList != null)
                {
                    return KindOfCommandParameters.ParametersByList;
                }

                if (ParamsDict != null)
                {
                    return KindOfCommandParameters.ParametersByDict;
                }

                return KindOfCommandParameters.NoParameters;
            }
        }

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
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = DisplayHelper.Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(ParamsCount)} = {ParamsCount}");
            sb.AppendLine($"{spaces}{nameof(KindOfCommandParameters)} = {KindOfCommandParameters}");

            sb.PrintObjListProp(n, nameof(ParamsList), ParamsList);

            if (ParamsDict == null)
            {
                sb.AppendLine($"{spaces}{nameof(ParamsDict)} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(ParamsDict)}");
                foreach (var item in ParamsDict)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Beign Key");
                    sb.Append(item.Key.ToString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Key");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
                sb.AppendLine($"{spaces}End {nameof(ParamsDict)}");
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = DisplayHelper.Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(ParamsCount)} = {ParamsCount}");
            sb.AppendLine($"{spaces}{nameof(KindOfCommandParameters)} = {KindOfCommandParameters}");

            sb.PrintShortObjListProp(n, nameof(ParamsList), ParamsList);

            if (ParamsDict == null)
            {
                sb.AppendLine($"{spaces}{nameof(ParamsDict)} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(ParamsDict)}");
                foreach (var item in ParamsDict)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Beign Key");
                    sb.Append(item.Key.ToShortString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Key");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToShortString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
                sb.AppendLine($"{spaces}End {nameof(ParamsDict)}");
            }

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
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = DisplayHelper.Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(ParamsCount)} = {ParamsCount}");
            sb.AppendLine($"{spaces}{nameof(KindOfCommandParameters)} = {KindOfCommandParameters}");

            sb.PrintBriefObjListProp(n, nameof(ParamsList), ParamsList);

            if (ParamsDict == null)
            {
                sb.AppendLine($"{spaces}{nameof(ParamsDict)} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(ParamsDict)}");
                foreach (var item in ParamsDict)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Beign Key");
                    sb.Append(item.Key.ToBriefString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Key");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToBriefString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
                sb.AppendLine($"{spaces}End {nameof(ParamsDict)}");
            }

            return sb.ToString();
        }
    }
}
