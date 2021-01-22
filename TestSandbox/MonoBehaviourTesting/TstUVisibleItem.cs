using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TestSandbox.MonoBehaviourTesting
{
    public struct TstUVisibleItem : IObjectToString
    {
        /// <summary>
        /// InstanceId in Unity Engine.
        /// </summary>
        public int InstanceId { get; set; }

        /// <summary>
        /// Position of this GameObject in Unity Engine.
        /// This position will be used for prevention chitering AI.
        /// </summary>
        public Vector3 Position { get; set; }
        public float Distance { get; set; }
        public bool IsInFocus { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(InstanceId)} = {InstanceId}");
            sb.AppendLine($"{spaces}{nameof(Position)} = {Position}");
            sb.AppendLine($"{spaces}{nameof(Distance)} = {Distance}");
            sb.AppendLine($"{spaces}{nameof(IsInFocus)} = {IsInFocus}");
            return sb.ToString();
        }
    }
}
