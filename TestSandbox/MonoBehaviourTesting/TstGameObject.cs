using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Text;
using TestSandbox.CoreHostListener;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.MonoBehaviourTesting
{
    public class TstGameObject : TstMonoBehaviour
    {
        private readonly IEntityLogger _logger = new LoggerImpementation();

        private IGameObject _gameObject;
        private string _id;

        public override void Awake()
        {
            _logger.Log("Begin");

            var platformListener = new TstPlatformHostListener();

            var settings = new GameObjectSettings();

            _id = "#`Gun 1`";
            settings.Id = _id;
            settings.InstanceId = 2;
            settings.HostListener = platformListener;

            _gameObject = WorldFactory.WorldInstance.GetGameObject(settings);

            _logger.Log("End");
        }

        public override void Stop()
        {
            _logger.Log("Begin");

            _gameObject.Dispose();

            _logger.Log("End");
        }
    }
}
