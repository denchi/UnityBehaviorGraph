using UnityEngine;

namespace Behaviours.Graph
{
    public class Input : ScriptableObject
    {
        public string title;
        public ValueType type = ValueType.None;
        public Task task;
        public int order;

        public Output output;

        void OnEnable()
        {
            hideFlags = HideFlags.HideInHierarchy;            
        }
    }
}