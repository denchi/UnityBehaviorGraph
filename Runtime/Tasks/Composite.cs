using System.Collections.Generic;

namespace Behaviours.Graph
{
    [Task(Title = "Graph")]
    [Input(Title = "Sync")]
    [Output(Title = "Sync")]
    [Output(Title = "Result", Type = ValueType.Other, Order = 1)]
    public class Composite : Task
    {
        public List<Node> nodes;

        public override void OnEnable()
        {
            base.OnEnable();

            if (nodes == null)
            {
                nodes = new List<Node>();
            }
        }
    }
}