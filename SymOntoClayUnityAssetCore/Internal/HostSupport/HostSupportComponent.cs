/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.HostSupport
{
    public class HostSupportComponent : BaseComponent, IHostSupport
    {
        public HostSupportComponent(IEntityLogger logger, IPlatformSupport platformSupport, IWorldCoreGameComponentContext worldContext)
            : base(logger)
        {
            _worldContext = worldContext;
            _invokerInMainThread = worldContext.InvokerInMainThread;
            _platformSupport = platformSupport;

        }

        private readonly IWorldCoreGameComponentContext _worldContext;
        private readonly IInvokerInMainThread _invokerInMainThread;
        private readonly IPlatformSupport _platformSupport;

        /// <inheritdoc/>
        public Vector3 ConvertFromRelativeToAbsolute(RelativeCoordinate relativeCoordinate)
        {
#if DEBUG
            Log($"relativeCoordinate = {relativeCoordinate}");
#endif

            var invocableInMainThreadObj = new InvocableInMainThreadObj<Vector3>(() => {
                return _platformSupport.ConvertFromRelativeToAbsolute(relativeCoordinate);
            }, _invokerInMainThread);

            return invocableInMainThreadObj.Run();
        }
    }
}
