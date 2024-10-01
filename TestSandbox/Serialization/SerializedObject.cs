﻿using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Serialization;
using SymOntoClay.Serialization.Settings;
using SymOntoClay.Threading;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TestSandbox.Serialization
{
    [SocSerialization]
    public partial class SerializedObject : IObjectToString
    {
        public SerializedObject(bool value) 
        {
            _dict1[1] = "Hi!";
            _dict1[5] = "Hello!";

            //_cancellationTokenSource = new CancellationTokenSource();
            //_cancellationToken = _cancellationTokenSource.Token;

            ////_cancellationTokenSource.Cancel();

            //_cancellationTokenSource2 = new CancellationTokenSource();

            //_linkedCancellationTokenSourceSettings = new LinkedCancellationTokenSourceSerializationSettings()
            //{
            //    Token1 = _cancellationTokenSource.Token,
            //    Token2 = _cancellationTokenSource2.Token
            //};

            //_linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, _cancellationTokenSource2.Token);

            //_linkedCancellationTokenSourceSettings2 = new LinkedCancellationTokenSourceSerializationSettings()
            //{
            //    Token1 = _cancellationTokenSource.Token,
            //    Token2 = _cancellationTokenSource2.Token,
            //    Token3 = CancellationToken.None
            //};

            //_linkedCancellationTokenSource2 = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, _cancellationTokenSource2.Token, CancellationToken.None);

            //_noneCancelationToken = CancellationToken.None;

            //var minThreadsCount = 1;
            //var maxThreadsCount = 5;

            //_customThreadPoolSerializationSettings = new CustomThreadPoolSerializationSettings()
            //{
            //    MaxThreadsCount = maxThreadsCount,
            //    MinThreadsCount = minThreadsCount,
            //    CancellationToken = _noneCancelationToken
            //};

            //_threadPool = new CustomThreadPool(minThreadsCount, maxThreadsCount, _noneCancelationToken);
        }

        public int IntField;

        private Dictionary<int, string> _dict1 = new Dictionary<int, string>();

        //private object _lockObj = new object();

        //private CancellationTokenSource _cancellationTokenSource;
        //private CancellationToken _cancellationToken;
        //private CancellationTokenSource _cancellationTokenSource2;

        //[SocObjectSerializationSettings(nameof(_linkedCancellationTokenSourceSettings))]
        //private CancellationTokenSource _linkedCancellationTokenSource;

        //[SocObjectSerializationSettings(nameof(_linkedCancellationTokenSourceSettings2))]
        //private CancellationTokenSource _linkedCancellationTokenSource2;

        //private LinkedCancellationTokenSourceSerializationSettings _linkedCancellationTokenSourceSettings;
        //private LinkedCancellationTokenSourceSerializationSettings _linkedCancellationTokenSourceSettings2;

        //private CancellationToken _noneCancelationToken;

        //private CustomThreadPoolSerializationSettings _customThreadPoolSerializationSettings;

        //[SocObjectSerializationSettings(nameof(_customThreadPoolSerializationSettings))]
        //private ICustomThreadPool _threadPool;

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
            //sb.AppendLine($"{spaces}{nameof(_cancellationTokenSource)}.{nameof(_cancellationTokenSource.IsCancellationRequested)} = {_cancellationTokenSource?.IsCancellationRequested}");
            sb.PrintPODDictProp(n, nameof(_dict1), _dict1);
            return sb.ToString();
        }
    }
}
