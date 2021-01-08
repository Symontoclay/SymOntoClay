using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.MonoBehaviourTesting
{
    public class TstWord: TstMonoBehaviour
    {
        private readonly IEntityLogger _logger = new LoggerImpementation();

        public override void Awake()
        {
            _logger.Log("Begin");



            _logger.Log("End");
        }

        public override void Start()
        {
            _logger.Log("Begin");



            _logger.Log("End");
        }
    }
}
