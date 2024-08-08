using System.Text;
using System;
using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Serialization;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.Functors.PlainObjects
{
    public partial class CodeChunkPo : IObjectToString
    {
        public string _id;
        public bool _isFinished;
        public bool _actionIsFinished;
        public ObjectPtr _codeChunksFactory;
        //public ObjectPtr _action;
        public ObjectPtr _children;
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
            sb.AppendLine($"{spaces}{nameof(_id)} = {_id}");
            sb.AppendLine($"{spaces}{nameof(_isFinished)} = {_isFinished}");
            sb.AppendLine($"{spaces}{nameof(_actionIsFinished)} = {_actionIsFinished}");
            sb.AppendLine($"{spaces}{nameof(_codeChunksFactory)} = {_codeChunksFactory}");
            //sb.AppendLine($"{spaces}{nameof(_action)} = {_action}");
            sb.AppendLine($"{spaces}{nameof(_children)} = {_children}");
            return sb.ToString();
        }
    }
}

namespace SymOntoClay.ActiveObject.Functors
{
    public partial class CodeChunk : ISerializable
    {
        Type ISerializable.GetPlainObjectType() => typeof(PlainObjects.CodeChunkPo);

        void ISerializable.OnWritePlainObject(object plainObject, ISerializer serializer)
        {
            OnWritePlainObject((PlainObjects.CodeChunkPo)plainObject, serializer);
        }

        private void OnWritePlainObject(PlainObjects.CodeChunkPo plainObject, ISerializer serializer)
        {
            plainObject._id = _id;
            plainObject._isFinished = _isFinished;
            plainObject._actionIsFinished = _actionIsFinished;
            plainObject._codeChunksFactory = serializer.GetSerializedObjectPtr(_codeChunksFactory);
            //plainObject._action = serializer.GetSerializedObjectPtr(_action);
            plainObject._children = serializer.GetSerializedObjectPtr(_children);
        }

        void ISerializable.OnReadPlainObject(object plainObject, IDeserializer deserializer)
        {
            OnReadPlainObject((PlainObjects.CodeChunkPo)plainObject, deserializer);
        }

        private void OnReadPlainObject(PlainObjects.CodeChunkPo plainObject, IDeserializer deserializer)
        {
            _id = plainObject._id;
            _isFinished = plainObject._isFinished;
            _actionIsFinished = plainObject._actionIsFinished;
            _codeChunksFactory = deserializer.GetDeserializedObject<ICodeChunksContext>(plainObject._codeChunksFactory);
            //_action = deserializer.GetDeserializedObject<Action>(plainObject._action);
            _children = deserializer.GetDeserializedObject<List<ICodeChunk>>(plainObject._children);
        }

    }
}
