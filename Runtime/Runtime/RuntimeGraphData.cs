using UnityEngine;
using System.Collections.Generic;
using Behaviours.Runtime;

namespace Behaviours.Graph.Runtime
{
    public class RuntimeGraphData
    {
        public List<RuntimeValueData> values;
        public RuntimeGroupData runtimeRootGroupData;
        public GameObject gameObject;
        public Graph associatedGraph;

        public RuntimeValueData findRuntimeValue(string name)
        {
            return values.Find(x => x.name == name);
        }

        public RuntimeValueData findRuntimeValue(int hashCode)
        {
            return values.Find(x => x.hash == hashCode);
        }

        public void start()
        {
            runtimeRootGroupData.defaultRuntimeTaskData.sync("Sync");
        }

        public void update(float dt)
        {
            
        }
    }
}