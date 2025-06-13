using UnityEngine;
using Behaviours.Graph.Runtime;

namespace Behaviours.Graph
{
    public class Flowgraph : MonoBehaviour
    {
        public Graph graph;

        public RuntimeGraphData runtimeGraph;

        void OnEnable()
        {
            runtimeGraph = Utils.createWithGraph(graph, gameObject);
            runtimeGraph.start();
        }
    }
}