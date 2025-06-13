using UnityEngine;
using System.Collections.Generic;

using Behaviours.Runtime;

namespace Behaviours.Graph.Runtime
{
    public class Utils
    {
        public static RuntimeGraphData createWithGraph(Graph graph, GameObject gameObject)
        {
            RuntimeGraphData runtimeGraph = new RuntimeGraphData();
            {
                runtimeGraph.associatedGraph = graph;
                runtimeGraph.gameObject = gameObject;

                runtimeGraph.values = new List<RuntimeValueData>();

                for (int i = 0; i < graph.values.Count; ++i)
                {
                    RuntimeValueData v = new RuntimeValueData(graph.values[i].name);
                    {
                        v.type = graph.values[i].type;

                        switch (graph.values[i].type)
                        {
                            case ValueType.Bool: v.bValue = graph.values[i].bValue; break;
                            case ValueType.Integer: v.iValue = graph.values[i].iValue; break;
                            case ValueType.Float: v.fValue = graph.values[i].fValue; break;
                            case ValueType.String: v.sValue = graph.values[i].sValue; break;
                            case ValueType.Other: v.sValue = graph.values[i].sValue; break;
                        }
                    }

                    runtimeGraph.values.Add(v);
                }

                runtimeGraph.runtimeRootGroupData = new RuntimeGroupData();
                {
                    runtimeGraph.runtimeRootGroupData.runtimeGraph = runtimeGraph;
                    runtimeGraph.runtimeRootGroupData.setTask(graph.root.task);
                }
            }
            
            return runtimeGraph;
        }        
    }
}