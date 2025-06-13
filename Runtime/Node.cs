using UnityEngine;

namespace Behaviours.Graph
{
    public class Node : ScriptableObject
    {
        public string title;
        public Graph graph;
        public Node parent;
        public Task task;
        public Rect rect;

        //public List<Input> inputs;
        //public List<Output> outputs;

        void OnEnable()
        {
            
        }

        //public Input getInput(string Title)
        //{
        //    return inputs.Find(x => x.title == Title);
        //}

        //public Output getOutput(string Title)
        //{
        //    return outputs.Find(x => x.title == Title);
        //}
    }
}
