using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.TasksExecution
{
    public class BuiltPlanItem : IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString
    {
        public BaseTask ProcessedTask { get; set; }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public BuiltPlanItem Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public BuiltPlanItem Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (BuiltPlanItem)context[this];
            }

            var result = new BuiltPlanItem();
            context[this] = result;

            result.ProcessedTask = ProcessedTask;

            return result;
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

            sb.PrintObjProp(n, nameof(ProcessedTask), ProcessedTask);

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

            sb.PrintShortObjProp(n, nameof(ProcessedTask), ProcessedTask);

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

            sb.PrintBriefObjProp(n, nameof(ProcessedTask), ProcessedTask);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToDbgString()
        {
            return ToDbgString(0u);
        }

        /// <inheritdoc/>
        public string ToDbgString(uint n)
        {
            return this.GetDefaultToDbgStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToDbgString.PropertiesToDbgString(uint n)
        {
            //var spaces = DisplayHelper.Spaces(n);
            //var sb = new StringBuilder();
            //return sb.ToString();

            throw new NotImplementedException("EE6E92A2-37BA-4329-9525-339C97CAB4E9");
        }
    }
}
