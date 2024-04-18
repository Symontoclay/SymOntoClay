/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints;
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters;
using SymOntoClay.UnityAsset.Core.Internal.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SymOntoClay.Monitor.Common;
using SymOntoClay.CoreHelper.DebugHelpers;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations
{
    public abstract class BaseManualControllingGameComponent: BaseGameComponent, IHostListener
    {
        protected BaseManualControllingGameComponent(BaseManualControllingGameComponentSettings settings, IWorldCoreGameComponentContext worldContext)
            : base(settings, worldContext)
        {
            BaseManualControllingGameComponentSettingsValidator.Validate(settings);

            var platformTypesConvertorsRegistry = worldContext.PlatformTypesConvertors;

            _endpointsRegistries = new List<IEndpointsRegistry>();

            _hostEndpointsRegistry = new EndpointsRegistry(Logger);
            _endpointsRegistries.Add(_hostEndpointsRegistry);

            var platformEndpointsList = EndpointDescriber.GetEndpointsInfoList(settings.HostListener);

            _hostEndpointsRegistry.AddEndpointsRange(platformEndpointsList);

            _endPointsResolver = new EndPointsResolver(Logger, platformTypesConvertorsRegistry);

            _endPointActivator = new EndPointActivator(Logger, platformTypesConvertorsRegistry, worldContext.InvokerInMainThread);

            _internalManualControlledObjectsList = new List<IInternalManualControlledObject>();
            _internalManualControlledObjectsDict = new Dictionary<IGameObject, InternalManualControlledObject>();
            _endpointsRegistryForManualControlledObjectsDict = new Dictionary<IGameObject, EndpointsProxyRegistryForDevices>();
        }

        private readonly List<IEndpointsRegistry> _endpointsRegistries;
        private readonly EndpointsRegistry _hostEndpointsRegistry;
        private readonly EndPointsResolver _endPointsResolver;
        private readonly EndPointActivator _endPointActivator;

        private readonly List<IInternalManualControlledObject> _internalManualControlledObjectsList;
        private readonly Dictionary<IGameObject, InternalManualControlledObject> _internalManualControlledObjectsDict;
        private readonly Dictionary<IGameObject, EndpointsProxyRegistryForDevices> _endpointsRegistryForManualControlledObjectsDict;

        private readonly object _manualControlLockObj = new object();

        public void AddToManualControl(IGameObject obj, int device)
        {
            AddToManualControl(obj, new List<int>() { device});
        }

        public void AddToManualControl(IGameObject obj, IList<int> devices)
        {
            lock (_manualControlLockObj)
            {
                if(_internalManualControlledObjectsDict.ContainsKey(obj))
                {
                    _internalManualControlledObjectsDict[obj].Devices = devices;
                    _endpointsRegistryForManualControlledObjectsDict[obj].Devices = devices;
                    return;
                }

                var internalManualControlledObject = new InternalManualControlledObject(obj);
                internalManualControlledObject.Devices = devices;

                _internalManualControlledObjectsList.Add(internalManualControlledObject);

                _internalManualControlledObjectsDict[obj] = internalManualControlledObject;

                var endpointsProxyRegistryForDevices = new EndpointsProxyRegistryForDevices(Logger, obj.EndpointsRegistry, devices);
                _endpointsRegistries.Add(endpointsProxyRegistryForDevices);
                _endpointsRegistryForManualControlledObjectsDict[obj] = endpointsProxyRegistryForDevices;
            }
        }

        public void RemoveFromManualControl(IGameObject obj)
        {
            lock (_manualControlLockObj)
            {
                if(!_internalManualControlledObjectsDict.ContainsKey(obj))
                {
                    return;
                }

                var internalManualControlledObject = _internalManualControlledObjectsDict[obj];
                _internalManualControlledObjectsDict.Remove(obj);

                _internalManualControlledObjectsList.Remove(internalManualControlledObject);

                var endpointsProxyRegistryForDevices = _endpointsRegistryForManualControlledObjectsDict[obj];
                _endpointsRegistryForManualControlledObjectsDict.Remove(obj);

                _endpointsRegistries.Remove(endpointsProxyRegistryForDevices);
            }
        }

        public IList<IInternalManualControlledObject> GetManualControlledObjects()
        {
            lock (_manualControlLockObj)
            {
                return _internalManualControlledObjectsList.ToList();
            }
        }

        /// <inheritdoc/>
        public IProcessCreatingResult CreateProcess(IMonitorLogger logger, string callMethodId, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
            try
            {
                var packedSynonymsResolver = new PackedSynonymsResolver(context.DataResolversFactory.GetSynonymsResolver(), localContext);

                var endPointInfo = _endPointsResolver.GetEndpointInfo(logger, callMethodId, command, _endpointsRegistries, packedSynonymsResolver);

                logger.SystemExpr("9C43C9A5-6D3A-420F-A97E-1399F66AA196", callMethodId, "endPointInfo != null", endPointInfo != null);

                if (endPointInfo == null)
                {
                    return new ProcessCreatingResult();
                }

                logger.SystemExpr("23122C03-0F23-4CF8-8BC6-C46871CD5B3A", callMethodId, "endPointInfo.Name", endPointInfo.Name);
                logger.SystemExpr("DB3AE13D-FE1E-467C-B407-2A4548C826CF", callMethodId, "endPointInfo.Devices", endPointInfo.Devices.WritePODListToString());

                var processInfo = _endPointActivator.Activate(logger, callMethodId, endPointInfo, command, context, localContext);

                return new ProcessCreatingResult(processInfo);
            }
            catch (Exception e)
            {
                logger.Error("D61FB02A-C84A-4959-9F72-67F9479600CB", e);

                return new ProcessCreatingResult(e);
            }
        }
    }
}
