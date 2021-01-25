using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core.Internal.HostSupport;
using SymOntoClay.UnityAsset.Core.Internal.Vision;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC
{
    public class HumanoidNPCGameComponentContext
    {
        public string IdForFacts { get; set; }
        public int SelfInstanceId { get; set; }
        public VisionComponent VisionComponent { get; set; }
        public HostSupportComponent HostSupportComponent { get; set; }
        public Engine CoreEngine { get; set; }
    }
}
