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

using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.StandardFacts;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Internal.SoundPerception;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.SoundBusHandler
{
    public class TstSoundReceiver: BaseSoundReceiverComponent
    {
        public TstSoundReceiver(int instanceId, Vector3 position)
            : base(new MonitorLoggerNLogImpementation(), instanceId, new StandardFactsBuilder())
        {
            _position = position;
        }

        private Vector3 _position;

        public override Vector3 Position => _position;

        public override double Threshold => 0;

        public override void CallBack(double power, double distance, Vector3 position, string query)
        {
            Info("5EDCCF78-ED84-4429-B44C-96C91054237B", $"power = {power}");
            Info("BB493075-BC4F-4CB1-8B27-EA58F6682035", $"distance = {distance}");
            Info("67DAB0BB-07B3-4C79-973A-52CA3702DF24", $"position = {position}");
            Info("0FA33490-AFB9-426F-99DF-DF06CEC7DDE3", $"query = {query}");

            var convertedQuery = ConvertQuery(power, distance, position, query);

            Info("0C4EB551-779E-496E-83F2-10D1707EAB24", $"convertedQuery = {convertedQuery}");




        }

        /// <inheritdoc/>
        public override void CallBack(double power, double distance, Vector3 position, RuleInstance fact)
        {
#if DEBUG
            Info("4D34231F-3E4A-4554-A25A-189A66F29A8D", $"power = {power}");
            Info("D87B4138-2CA2-4AA8-8B46-A50694B6A515", $"distance = {distance}");
            Info("B5DB24BA-9B1B-48A0-9F63-5DCA8B8290DD", $"position = {position}");
            Info("4B69FD9A-7109-4B08-9BD9-4B523A9C8A54", $"fact = {fact.ToHumanizedString()}");
#endif

            var convertedQuery = ConvertQuery(power, distance, position, fact);

            Info("7DA0BE69-3A7F-40F4-A60C-88FC811A9FD9", $"convertedQuery = {convertedQuery.ToHumanizedString()}");
        }

        protected override float GetDirectionToPosition(Vector3 position)
        {
            return 12;
        }
    }
}
