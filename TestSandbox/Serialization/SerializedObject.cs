using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Serialization;
using SymOntoClay.Serialization.Settings;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TestSandbox.Serialization
{
    [SocSerialization]
    public partial class SerializedObject : IObjectToString, IObjectToBriefString
    {
        public SerializedObject(bool value) 
        {
            _dict1[1] = "Hi!";
            _dict1[5] = "Hello!";

            _dict2[3] = "Hi!";
            _dict2[5] = 12;
            _dict2[7] = new object();
            _dict2[8] = true;
            _dict2[9] = 1.23;
            _dict2[10] = DateTime.Now;

            //_dict3[12] = this;

            //_dict4[5] = 16;

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

        //private Dictionary<int, sbyte> _dict = new Dictionary<int, sbyte>();
        private Dictionary<int, string> _dict1 = new Dictionary<int, string>();
        private Dictionary<int, object> _dict2 = new Dictionary<int, object>();
        //private Dictionary<int, SerializedObject> _dict3 = new Dictionary<int, SerializedObject>();
        //private Dictionary<object, int> _dict4 = new Dictionary<object, int>();

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
            sb.PrintPODDictProp(n, nameof(_dict2), _dict2);
            //sb.PrintBriefObjDict_2_Prop(n, nameof(_dict3), _dict3);
            //sb.PrintPODDictProp(n, nameof(_dict4), _dict4);
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(IntField)} = {IntField}");
            return sb.ToString();
        }
    }
}
