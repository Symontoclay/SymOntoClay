/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.Common.SerializationToImage.Attributes;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.SerializationToImage;
using SymOntoClay.CoreHelper.SerializationToImage.Attributes;
using SymOntoClay.CoreHelper.SerializationToImage.ComponentsInterfaces;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.InternalImplementations.GameObject;
using SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC;
using SymOntoClay.UnityAsset.Core.InternalImplementations.Place;
using SymOntoClay.UnityAsset.Core.InternalImplementations.Player;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace SymOntoClay.UnityAsset.Core.World
{
    [WorldRootAttribute]
    [SerializeOnlyExplicitlySerializableMembersAttribute]
    public class WorldCore: IWorld, ISerializedWorldRoot
    {
        #region constructors
        public WorldCore(WorldSettings settings)
        {
            _settings = settings;

            _context = new WorldContext(settings);
        }
        #endregion

        #region public members

        /// <inheritdoc/>
        [WorldComponentIdMember]
        public string Id => string.Empty;

        /// <inheritdoc/>
        public string IdForFacts => string.Empty;

        /// <inheritdoc/>
        public int InstanceId => 0;

        /// <inheritdoc/>
        public WorldContext WorldContext => _context;

        /// <inheritdoc/>
        public IMonitor Monitor => _context.Monitor;

        /// <inheritdoc/>
        public void AddConvertor(IPlatformTypesConverter convertor)
        {
            lock (_lockObj)
            {
                if(_platformTypesConverters.Contains(convertor))
                {
                    return;
                }

                _platformTypesConverters.Add(convertor);
                _context.AddConvertor(convertor);
            }
        }

        /// <inheritdoc/>
        public bool EnableLogging { get => _context.EnableLogging; set => _context.EnableLogging = value; }

        /// <inheritdoc/>
        public bool EnableRemoteConnection { get => _context.EnableRemoteConnection; set => _context.EnableRemoteConnection = value; }

        /// <inheritdoc/>
        public IMonitorLogger Logger => _context.Logger;

        /// <inheritdoc/>
        public void RunInMainThread(Action function)
        {
            _context.RunInMainThread(function);
        }

        /// <inheritdoc/>
        public TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            return _context.RunInMainThread(function);
        }

        /// <inheritdoc/>
        public IHumanoidNPC GetHumanoidNPC(HumanoidNPCSettings settings)
        {
            lock (_lockObj)
            {
                var worldComponent = new HumanoidNPCImplementation(settings, _context);

                _serializedWorldComponents.Add(worldComponent);

                return worldComponent;
            }
        }

        /// <inheritdoc/>
        public IPlayer GetPlayer(PlayerSettings settings)
        {
            lock (_lockObj)
            {
                var worldComponent = new PlayerImlementation(settings, _context);

                _serializedWorldComponents.Add(worldComponent);

                return worldComponent;
            }
        }

        /// <inheritdoc/>
        public IGameObject GetGameObject(GameObjectSettings settings)
        {
            lock (_lockObj)
            {
                var worldComponent = new GameObjectImplementation(settings, _context);

                _serializedWorldComponents.Add(worldComponent);

                return worldComponent;
            }
        }

        /// <inheritdoc/>
        public IPlace GetPlace(PlaceSettings settings)
        {
            lock (_lockObj)
            {
                var worldComponent = new PlaceImplementation(settings, _context);

                _serializedWorldComponents.Add(worldComponent);

                return worldComponent;
            }
        }

        /// <inheritdoc/>
        public string InsertPublicFact(IMonitorLogger logger, string text)
        {
            throw new NotImplementedException("E33E5B22-DED1-48D1-858F-B5DAC5718854");
        }

        /// <inheritdoc/>
        public string InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            throw new NotImplementedException("6DAA88AB-D6E7-49BF-826C-01483F650BAD");
        }

        /// <inheritdoc/>
        public void RemovePublicFact(IMonitorLogger logger, string id)
        {
            throw new NotImplementedException("5E04617D-9C71-4A38-BD82-47480F32C660");
        }

        /// <inheritdoc/>
        public void PushSoundFact(float power, string text)
        {
            throw new NotImplementedException("C3641CE4-73D6-459B-904D-AB9623E6D96A");
        }

        /// <inheritdoc/>
        public void PushSoundFact(float power, RuleInstance fact)
        {
            throw new NotImplementedException("169604B9-2661-43FF-9321-572EBADE7E6C");
        }

        /// <inheritdoc/>
        public IStandardFactsBuilder StandardFactsBuilder => throw new NotImplementedException("AA896824-F1FE-4012-9F25-A702C837D52B");

        /// <inheritdoc/>
        public void AddCategory(IMonitorLogger logger, string category)
        {
            throw new NotImplementedException("0A8D1B89-5E02-447C-8A95-9F805B871A6A");
        }

        /// <inheritdoc/>
        public void AddCategories(IMonitorLogger logger, List<string> categories)
        {
            throw new NotImplementedException("3388265A-2A24-4790-8BFD-DAD36A3A4C07");
        }

        /// <inheritdoc/>
        public void RemoveCategory(IMonitorLogger logger, string category)
        {
            throw new NotImplementedException("B43DD791-F8CE-4ECD-9D11-C256A0C7D3A0");
        }

        /// <inheritdoc/>
        public void RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            throw new NotImplementedException("897967D3-58EE-4E27-9C19-1A0216083A8D");
        }

        /// <inheritdoc/>
        public bool EnableCategories { get => throw new NotImplementedException("5B0E49C5-919F-40AE-A951-BCB364258B17"); set => throw new NotImplementedException("339F2D6D-0225-4439-9C9D-462CC4A356B2"); }

        /// <inheritdoc/>
        public List<ISerializedWorldComponent> SerializedWorldComponents => _serializedWorldComponents;

        /// <inheritdoc/>
        public void LoadFromSourceCode()
        {
            _context.LoadFromSourceCode();
        }

        /// <inheritdoc/>
        public void LoadFromImage(SerializationToImageSettings settings)
        {
            _context.LoadFromImage(settings);
        }

        /// <inheritdoc/>
        public void SaveToImage(SerializationToImageSettings settings)
        {

#if DEBUG
            //Info(, $"settings = {settings}");
#endif

            _context.Stop();

            var structuralContext = new WorldStructuralContext();

            var serializer = new SerializerToImage(settings, structuralContext);
            serializer.Serialize(this);

            throw new NotImplementedException("C0A8D36D-50A8-4015-87C2-861455D6D421");

            //_context.SaveToImage(settings);
        }

        /// <inheritdoc/>
        public void Start()
        {
            _context.Start();
        }

        /// <inheritdoc/>
        public void Stop()
        {
            _context.Stop();
        }

        /// <inheritdoc/>
        public bool IsActive { get => _context.IsActive; }

        /// <inheritdoc/>
        public void Dispose()
        {
            _context.Dispose();
        }

        /// <inheritdoc/>
        public bool IsDisposed { get => _context.IsDisposed; }
        #endregion

        #region private members
        [SettingsMember]
        private readonly WorldSettings _settings;

        private readonly object _lockObj = new object();
        private ComponentState _state = ComponentState.Created;

        [SerializedMemberWithChildren]
        private List<ISerializedWorldComponent> _serializedWorldComponents = new List<ISerializedWorldComponent>();


        private List<IPlatformTypesConverter> _platformTypesConverters = new List<IPlatformTypesConverter>();

        private readonly WorldContext _context;
        #endregion
    }
}
