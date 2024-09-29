﻿using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Serialization;
using SymOntoClay.Serialization.Settings;
using System.Text;
using System.Threading;

namespace TestSandbox.Serialization
{
    [SocSerialization]
    public partial class SerializedObject : IObjectToString
    {
        public SerializedObject(bool value) 
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            //_cancellationTokenSource.Cancel();

            _cancellationTokenSource2 = new CancellationTokenSource();

            _linkedCancellationTokenSourceSettings = new LinkedCancellationTokenSourceSettings()
            {
                Token1 = _cancellationTokenSource.Token,
                Token2 = _cancellationTokenSource2.Token
            };

            _linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, _cancellationTokenSource2.Token);

            _noneCancelationToken = CancellationToken.None;
        }

        public int IntField;

        private object _lockObj = new object();

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private CancellationTokenSource _cancellationTokenSource2;

        [SocObjectSerializationSettings(nameof(_linkedCancellationTokenSourceSettings))]
        private CancellationTokenSource _linkedCancellationTokenSource;

        private LinkedCancellationTokenSourceSettings _linkedCancellationTokenSourceSettings;

        private CancellationToken _noneCancelationToken;

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }
        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }
        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(IntField)} = {IntField}");
            sb.AppendLine($"{spaces}{nameof(_cancellationTokenSource)}.{nameof(_cancellationTokenSource.IsCancellationRequested)} = {_cancellationTokenSource?.IsCancellationRequested}");
            return sb.ToString();
        }
    }
}
