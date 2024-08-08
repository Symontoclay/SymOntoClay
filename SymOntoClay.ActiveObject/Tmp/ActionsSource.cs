using SymOntoClay.ActiveObject.Functors;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ActiveObject.Tmp
{
    public static class ActionsSource
    {
        static ActionsSource()
        {
            _actions = new Dictionary<string, Action>();
            _actions_ICodeChunk = new Dictionary<string, Action<ICodeChunk>>();

            _actions_ICodeChunk["852D8948-7DA6-41C9-B6EE-E038D58F0248"] = (currentCodeChunk) =>
            {
                //_logger.Info("64C869BD-91D0-4CC5-B675-1BF13A9062F3", "Chunk1");
            };

            _actions_ICodeChunk["5A33E959-BC01-40B0-82DE-3874E0E31AD7"] = (currentCodeChunk) =>
            {
                //_logger.Info("3E5FE705-0F2E-45EE-A579-B080F2FBF566", "Chunk2");
            };
        }

        private static Dictionary<string, Action> _actions;
        private static Dictionary<string, Action<ICodeChunk>> _actions_ICodeChunk;

        public static Action<ICodeChunk> GetActions_ICodeChunk(string id) => _actions_ICodeChunk[id];
    }
}
