using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestSandbox.SoundBusHandler
{
    public interface ISoundReceiver
    {
        Vector3 Position { get; }
        double Threshold { get; }//Decibel
        void CallBack(double power, double distance, Vector3 position, string query);
    }
}
