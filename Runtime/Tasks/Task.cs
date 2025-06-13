using UnityEngine;
using System.Collections.Generic;

using Behaviours.Graph.Runtime;

namespace Behaviours.Graph
{
    public class Task : ScriptableObject
    {
        public virtual string Title
        {
            get
            {
                return title;
            }
        }

        public string title;

        public Node node;

        public List<Input> inputs;
        public List<Output> outputs;

        public virtual void OnEnable()
        {
            hideFlags = HideFlags.HideInHierarchy;

            if (inputs == null)
            {
                inputs = new List<Input>();
            }

            if (outputs == null)
            {
                outputs = new List<Output>();
            }
        }

        public virtual RuntimeTaskData createTaskData()
        {
            return new RuntimeTaskData();
        }        
    }
}