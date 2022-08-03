using NLog;
using SymOntoClay.Core.Internal.DataResolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.DebugHelpers
{
    /// <summary>
    /// You can see it by http://magjac.com/graphviz-visual-editor/.
    /// </summary>
    public static class DebugHelperForLogicalSearchExplainNode
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static string ToDot(LogicalSearchExplainNode source)
        {
#if DEBUG
            //_gbcLogger.Info($"source = {source}");
#endif

            if(source.Kind != KindOfLogicalSearchExplainNode.Root)
            {
                throw new Exception($"`{source.Kind}` node can not be converted to dot. Node should be {nameof(KindOfLogicalSearchExplainNode.Root)}.");
            }

            var context = new DebugHelperForLogicalSearchExplainNodeContext();

            context.Output.AppendLine("digraph graphname {");
            context.Output.AppendLine("rankdir=\"BT\"");

            ProcessNodeContent(source, context);

            foreach(var path in context.PathsList)
            {
                context.Output.AppendLine(path);
            }

            context.Output.AppendLine("}");

            return context.Output.ToString();
        }

        private static void ProcessNodeContent(LogicalSearchExplainNode source, DebugHelperForLogicalSearchExplainNodeContext context)
        {
#if DEBUG
            _gbcLogger.Info($"source = {source}");
#endif

            var name = context.GetNodeName(source);

#if DEBUG
            _gbcLogger.Info($"name = {name}");
#endif

            //Dot supports html https://graphviz.org/doc/info/shapes.html but the html can not contain '&' and '<>'.
            //So you should create huamnized string generation in html style.

            var nameBuilder = new StringBuilder();
            nameBuilder.Append($"{name} [shape=box label=\"");
            nameBuilder.Append("{: $x = {: act(M16, shoot) :} & hear(I, $x)&#92;n & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}");
            nameBuilder.AppendLine("\"];");

            context.Output.AppendLine(nameBuilder.ToString());

            if (source.Source != null)
            {
                ProcessNodeContent(source.Source, context);

                context.PathsList.Add($"{context.GetNodeName(source.Source)} -> {name};");
            }

            if(source.Result != null)
            {
                ProcessNodeContent(source.Result, context);

                context.PathsList.Add($"{context.GetNodeName(source.Result)} -> {name};");
            }
        }
    }
}
