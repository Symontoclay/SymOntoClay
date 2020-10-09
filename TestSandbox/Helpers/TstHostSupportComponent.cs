/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TestSandbox.Helpers
{
    public class TstHostSupportComponent: IHostSupport
    {
        public TstHostSupportComponent(IPlatformSupport platformSupport)
        {
            _platformSupport = platformSupport;
        }

        private readonly IPlatformSupport _platformSupport;

        /// <inheritdoc/>
        public Vector3 ConvertFromRelativeToAbsolute(Vector2 relativeCoordinates)
        {
            return _platformSupport.ConvertFromRelativeToAbsolute(relativeCoordinates);
        }
    }
}
