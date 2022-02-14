/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints;
using SymOntoClay.UnityAsset.Core.Internal.TypesConvertors;
using SymOntoClay.UnityAsset.Core.Internal.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public IProcessCreatingResult CreateProcess(ICommand command)
        {
#if DEBUG
            //Log($"command = {command}");
#endif

            try
            {
                var endPointInfo = _endPointsResolver.GetEndpointInfo(command, _endpointsRegistries);

#if DEBUG
                //Log($"endPointInfo = {endPointInfo}");
#endif

                if (endPointInfo == null)
                {
                    return new ProcessCreatingResult();
                }

                var processInfo = _endPointActivator.Activate(endPointInfo, command);

                return new ProcessCreatingResult(processInfo);
            }
            catch (Exception e)
            {
#if DEBUG
                Log($"e = {e}");
#endif

                return new ProcessCreatingResult(e);
            }
        }
    }
}
