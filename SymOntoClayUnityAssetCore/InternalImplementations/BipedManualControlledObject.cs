/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations
{
    public class BipedManualControlledObject : IBipedManualControlledObject
    {
        public BipedManualControlledObject(IGameObject gameObject, IList<int> devices)
        {
            _gameObject = gameObject;
            _devices = devices?.Select(p => (DeviceOfBiped)p).ToList();
        }

        private readonly IGameObject _gameObject;
        private readonly IList<DeviceOfBiped> _devices;

        /// <inheritdoc/>
        public IGameObject GameObject => _gameObject;

        /// <inheritdoc/>
        public IList<DeviceOfBiped> Devices => _devices;
    }
}
