/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public abstract class BaseGameComponentSettings : IObjectToString
    {
        /// <summary>
        /// Gets or sets unique Id.
        /// It allows us to identify each item of the game.
        /// </summary>
        public string Id { get; set; }
        public int InstanceId { get; set; }
        public Vector3? UseStaticPosition { get; set; }

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
            var nextN = n + 4;
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Id)} = {Id}");
            sb.AppendLine($"{spaces}{nameof(InstanceId)} = {InstanceId}");
            sb.AppendLine($"{spaces}{nameof(UseStaticPosition)} = {UseStaticPosition}");

            return sb.ToString();
        }
    }
}
