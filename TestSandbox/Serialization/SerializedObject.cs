using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.Serialization;
using SymOntoClay.Serialization.Settings;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TestSandbox.Serialization
{
    public class SerializedObject : IObjectToString, IObjectToBriefString
    {
        public SerializedObject(bool value) 
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            //_cancellationTokenSource.Cancel();

            _cancellationTokenSource2 = new CancellationTokenSource();

            _linkedCancellationTokenSourceSettings = new LinkedCancellationTokenSourceSerializationSettings()
            {
                Token1 = _cancellationTokenSource.Token,
                Token2 = _cancellationTokenSource2.Token
            };

            _linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, _cancellationTokenSource2.Token);

            _linkedCancellationTokenSourceSettings2 = new LinkedCancellationTokenSourceSerializationSettings()
            {
                Token1 = _cancellationTokenSource.Token,
                Token2 = _cancellationTokenSource2.Token,
                Token3 = CancellationToken.None
            };

            _linkedCancellationTokenSource2 = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, _cancellationTokenSource2.Token, CancellationToken.None);

            _noneCancelationToken = CancellationToken.None;

            var minThreadsCount = 1;
            var maxThreadsCount = 5;

            _customThreadPoolSerializationSettings = new CustomThreadPoolSerializationSettings()
            {
                MaxThreadsCount = maxThreadsCount,
                MinThreadsCount = minThreadsCount,
                CancellationToken = _noneCancelationToken
            };

            _threadPool = new FakeCustomThreadPool(minThreadsCount, maxThreadsCount, _noneCancelationToken);

            _serializedSubObject = new SerializedSubObject()
            {
                SomeField = 222,
                Token = _linkedCancellationTokenSource2.Token
            };

            _dict1[1] = "Hi!";
            _dict1[5] = "Hello!";

            _dict2[3] = "Hi!";
            _dict2[5] = 12;
            _dict2[7] = new object();
            _dict2[8] = true;
            _dict2[9] = 1.23;
            _dict2[10] = DateTime.Now;
            _dict2[11] = _serializedSubObject;

            _dict3[12] = _serializedSubObject;

            _dict4[5] = 16;
            _dict4["Hi"] = 2000;
            _dict4[new object()] = 12;
            _dict4[_serializedSubObject] = 56;

            _dict5[5] = "Some value";

            _dict6[7] = _serializedSubObject;

            _dict7[_serializedSubObject] = 7;

            _dict8[_serializedSubObject] = "Some value 16";

            _dict9[_serializedSubObject] = _serializedSubObject;

            _list1.Add(16);
            _list2.Add("SomeValue");
            _list3.Add(_serializedSubObject);
        }

        private SerializedSubObject _serializedSubObject;

        public int IntField;

        //private Dictionary<int, sbyte> _dict = new Dictionary<int, sbyte>();
        private Dictionary<int, string> _dict1 = new Dictionary<int, string>();
        private Dictionary<int, object> _dict2 = new Dictionary<int, object>();
        private Dictionary<int, SerializedSubObject> _dict3 = new Dictionary<int, SerializedSubObject>();
        private Dictionary<object, int> _dict4 = new Dictionary<object, int>();
        private Dictionary<object, object> _dict5 = new Dictionary<object, object>();
        private Dictionary<object, SerializedSubObject> _dict6 = new Dictionary<object, SerializedSubObject>();
        private Dictionary<SerializedSubObject, int> _dict7 = new Dictionary<SerializedSubObject, int>();
        private Dictionary<SerializedSubObject, object> _dict8 = new Dictionary<SerializedSubObject, object>();
        private Dictionary<SerializedSubObject, SerializedSubObject> _dict9 = new Dictionary<SerializedSubObject, SerializedSubObject>();

        private List<int> _list1 = new List<int>();
        private List<object> _list2 = new List<object>();
        private List<SerializedSubObject> _list3 = new List<SerializedSubObject>();

        private object _lockObj = new object();

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private CancellationTokenSource _cancellationTokenSource2;

        [SocObjectSerializationSettings(nameof(_linkedCancellationTokenSourceSettings))]
        private CancellationTokenSource _linkedCancellationTokenSource;

        [SocObjectSerializationSettings(nameof(_linkedCancellationTokenSourceSettings2))]
        private CancellationTokenSource _linkedCancellationTokenSource2;

        private LinkedCancellationTokenSourceSerializationSettings _linkedCancellationTokenSourceSettings;
        private LinkedCancellationTokenSourceSerializationSettings _linkedCancellationTokenSourceSettings2;

        private CancellationToken _noneCancelationToken;

        private CustomThreadPoolSerializationSettings _customThreadPoolSerializationSettings;

        [SocObjectSerializationSettings(nameof(_customThreadPoolSerializationSettings))]
        private IFakeCustomThreadPool _threadPool;

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
            sb.PrintObjProp(n, nameof(_serializedSubObject), _serializedSubObject);
            sb.AppendLine($"{spaces}{nameof(IntField)} = {IntField}");
            //sb.AppendLine($"{spaces}{nameof(_cancellationTokenSource)}.{nameof(_cancellationTokenSource.IsCancellationRequested)} = {_cancellationTokenSource?.IsCancellationRequested}");
            sb.PrintPODDictProp(n, nameof(_dict1), _dict1);
            sb.PrintPODDictProp(n, nameof(_dict2), _dict2);
            sb.PrintObjDict_2_Prop(n, nameof(_dict3), _dict3);
            sb.PrintPODDictProp(n, nameof(_dict4), _dict4);
            sb.PrintPODDictProp(n, nameof(_dict5), _dict5);
            sb.PrintPODDictProp(n, nameof(_dict6), _dict6);
            sb.PrintPODDictProp(n, nameof(_dict7), _dict7);
            sb.PrintPODDictProp(n, nameof(_dict8), _dict8);
            sb.PrintPODDictProp(n, nameof(_dict9), _dict9);
            sb.PrintPODList(n, nameof(_list1), _list1);
            sb.PrintPODList(n, nameof(_list2), _list2);
            sb.PrintObjListProp(n, nameof(_list3), _list3);
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
