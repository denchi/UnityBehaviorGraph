using UnityEngine;

namespace Behaviours.Graph
{
    [CreateAssetMenu(menuName = "Behaviours/Graph")]
    public class Graph : ILayer
    {
        public Node root;

        public override void OnEnable()
        {
            base.OnEnable();            
        }
    }
}
