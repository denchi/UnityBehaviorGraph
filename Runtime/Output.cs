using UnityEngine;
using System.Collections.Generic;

namespace Behaviours.Graph
{
    public class Output : ScriptableObject
    {
        public Task task;        
        public string title;
        public ValueType type = ValueType.None;
        public int order;

        public Input input;
        public List<Input> inputs;

        void OnEnable()
        {
            hideFlags = HideFlags.HideInHierarchy;

            if (inputs == null)
            {
                inputs = new List<Input>();                
            }

            if (input != null)
                if (inputs.Contains(input) == false)
                    inputs.Add(input);
        }
    }
}
