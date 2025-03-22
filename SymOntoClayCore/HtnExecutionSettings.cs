using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.Core
{
    public class HtnExecutionSettings : IObjectToString
    {
        /// <summary>
        /// Sets Max count of plan execution iterations.
        /// <list type="bullet">
        /// <item>
        /// <term>
        /// null
        /// </term>
        /// <description>
        /// Infinite execution.
        /// It is for production mode.
        /// Default value.
        /// </description>
        /// </item>
        /// <item>
        /// 0
        /// <term>
        /// </term>
        /// <description>
        /// Turns off execution.
        /// </description>
        /// </item>
        /// <item>
        /// Any positive value.
        /// <term>
        /// </term>
        /// <description>
        /// Builds and executes htn plan the required number of times.
        /// </description>
        /// </item>
        /// </list>
        /// </summary>
        public int? PlanExecutionIterationsMaxCount { get; set; }

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
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(PlanExecutionIterationsMaxCount)} = {PlanExecutionIterationsMaxCount}");

            return sb.ToString();
        }
    }
}
