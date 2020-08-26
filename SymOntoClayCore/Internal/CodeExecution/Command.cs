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

            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(ParamsCount)} = {ParamsCount}");
            sb.AppendLine($"{spaces}{nameof(KindOfCommandParameters)} = {KindOfCommandParameters}");

            sb.PrintObjListProp(n, nameof(ParamsList), ParamsList);

            sb.PrintObjDict_1_Prop(n, nameof(ParamsDict), ParamsDict);

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

            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(ParamsCount)} = {ParamsCount}");
            sb.AppendLine($"{spaces}{nameof(KindOfCommandParameters)} = {KindOfCommandParameters}");

            sb.PrintShortObjListProp(n, nameof(ParamsList), ParamsList);

            sb.PrintShortObjDict_1_Prop(n, nameof(ParamsDict), ParamsDict);

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
                     
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(ParamsCount)} = {ParamsCount}");
            sb.AppendLine($"{spaces}{nameof(KindOfCommandParameters)} = {KindOfCommandParameters}");

            sb.PrintBriefObjListProp(n, nameof(ParamsList), ParamsList);

            sb.PrintBriefObjDict_1_Prop(n, nameof(ParamsDict), ParamsDict);

            return sb.ToString();
        }
    }
}
