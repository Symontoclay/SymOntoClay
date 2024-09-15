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
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.Threads;

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

            _endPointActivator = new EndPointActivator(Logger, platformTypesConvertorsRegistry, worldContext.InvokerInMainThread, AsyncEventsThreadPool);

            _internalManualControlledObjectsList = new List<IInternalManualControlledObject>();
            _internalManualControlledObjectsDict = new Dictionary<IGameObject, InternalManualControlledObject>();
            _endpointsRegistryForManualControlledObjectsDict = new Dictionary<IGameObject, EndpointsProxyRegistryForDevices>();

            _activeObjectContext = ActiveObjectContext;
            _serializationAnchor = new SerializationAnchor();
        }

        private List<IEndpointsRegistry> _endpointsRegistries;
        private EndpointsRegistry _hostEndpointsRegistry;
        private EndPointsResolver _endPointsResolver;
        private EndPointActivator _endPointActivator;

        private List<IInternalManualControlledObject> _internalManualControlledObjectsList;
        private Dictionary<IGameObject, InternalManualControlledObject> _internalManualControlledObjectsDict;
        private Dictionary<IGameObject, EndpointsProxyRegistryForDevices> _endpointsRegistryForManualControlledObjectsDict;

        private object _manualControlLockObj = new object();

        protected IActiveObjectContext _activeObjectContext;
        protected SerializationAnchor _serializationAnchor;

        public ISyncMethodResponse AddToManualControl(IGameObject obj, int device)
        {
            return AddToManualControl(obj, new List<int>() { device });
        }

        public ISyncMethodResponse AddToManualControl(IGameObject obj, IList<int> devices)
        {
            return LoggedSyncFunctorWithoutResult<BaseManualControllingGameComponent, IGameObject, IList<int>>.Run(Logger, "4B621E74-7606-4F98-97F0-4C4DE29704E8", this, obj, devices,
                (IMonitorLogger loggerValue, BaseManualControllingGameComponent instanceValue, IGameObject objValue, IList<int> devicesValue) => {
                    instanceValue.NAddToManualControl(objValue, devicesValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void NAddToManualControl(IGameObject obj, IList<int> devices)
        {
            lock (_manualControlLockObj)
            {
                if (_internalManualControlledObjectsDict.ContainsKey(obj))
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

        public ISyncMethodResponse RemoveFromManualControl(IGameObject obj)
        {
            return LoggedSyncFunctorWithoutResult<BaseManualControllingGameComponent, IGameObject>.Run(Logger, "0BC7B04D-81C2-47ED-A36C-1E9D3A381DAE", this, obj,
                (IMonitorLogger loggerValue, BaseManualControllingGameComponent instanceValue, IGameObject objValue) => {
                    instanceValue.NRemoveFromManualControl(objValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void NRemoveFromManualControl(IGameObject obj)
        {
            lock (_manualControlLockObj)
            {
                if (!_internalManualControlledObjectsDict.ContainsKey(obj))
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
