/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.BaseTestLib.Monitoring;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.StandardFacts;
using SymOntoClay.UnityAsset.Core.Internal.SoundPerception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib
{
    public class TestSoundReceiver : BaseSoundReceiverComponent
    {
        public TestSoundReceiver(int instanceId, Vector3 position, Action<double, double, Vector3, string, string> callBack)
            : base(new EmptyLogger(), instanceId, new StandardFactsBuilder())
        {
            _position = position;
            _callBack = callBack;
        }

        public TestSoundReceiver(int instanceId, Vector3 position, Action<double, double, Vector3, string, RuleInstance> callBack)
             : base(new EmptyLogger(), instanceId, new StandardFactsBuilder())
        {
            _position = position;
            _factCallBack = callBack;
        }

        private Vector3 _position;

        /// <inheritdoc/>
        public override Vector3 Position => _position;

        /// <inheritdoc/>
        public override double Threshold => 0;

        private readonly Action<double, double, Vector3, string, string> _callBack;
        private readonly Action<double, double, Vector3, string, RuleInstance> _factCallBack;

        /// <inheritdoc/>
        public override void CallBack(double power, double distance, Vector3 position, string query)
        {
            var convertedQuery = ConvertQuery(power, distance, position, query);

            _callBack(power, distance, position, query, convertedQuery);
        }

        /// <inheritdoc/>
        public override void CallBack(double power, double distance, Vector3 position, RuleInstance fact)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override float GetDirectionToPosition(Vector3 position)
        {
            return 12;
        }
    }
}
