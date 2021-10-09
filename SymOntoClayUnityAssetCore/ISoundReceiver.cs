using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public interface ISoundReceiver
    {
        int InstanceId { get; }
        Vector3 Position { get; }
        IEntityLogger Logger { get; }

        /// <summary>
        /// Gets threshold of audibility in decibels.
        /// </summary>
        double Threshold { get; }
        void CallBack(double power, double distance, Vector3 position, string query);
    }
}
