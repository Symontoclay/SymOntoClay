using SymOntoClay.Core.Internal.DataResolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.DebugHelpers
{
    public class DebugHelperForLogicalSearchExplainNodeContext
    {
        public StringBuilder Output { get; set; } = new StringBuilder();
        private Dictionary<LogicalSearchExplainNode, string> _nodeNamesDict = new Dictionary<LogicalSearchExplainNode, string>();
        public List<string> PathsList { get; set; } = new List<string>();
        private int _currIndex;

        public string GetNodeName(LogicalSearchExplainNode node)
        {
            if(_nodeNamesDict.ContainsKey(node))
            {
                return _nodeNamesDict[node];
            }

            _currIndex++;
            var name = $"n_{_currIndex}";
            _nodeNamesDict[node] = name;
            return name;
        }
    }
}
