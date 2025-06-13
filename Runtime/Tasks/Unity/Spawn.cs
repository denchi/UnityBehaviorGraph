using UnityEngine;
using Behaviours.Graph.Runtime;
using Behaviours.Runtime;

namespace Behaviours.Graph.Unity
{
    [Task(Title = "Spawn")]
    [Input(Title = "Sync", Order = 0)]    
    [Output(Title = "Sync", Order = 0)]
    [Output(Title = "OnFailed", Order = 1)]
    [Output(Title = "gameObject", Type = ValueType.Other, Order = 2)]
    public class Spawn : Task
    {
        public GameObject prefab;

        public override string Title
        {
            get
            {
                return base.Title;
            }
        }

        public class CustomRuntimeTaskData : RuntimeTaskData
        {
            public RuntimeValueData runtimeValueData;
        }

        public override RuntimeTaskData createTaskData() { return new CustomRuntimeTaskData(); }

        void Sync(Runtime.RuntimeTaskData runtimeData)
        {
            GameObject go = GameObject.Instantiate(prefab);

            CustomRuntimeTaskData customRuntimeTaskData = runtimeData as CustomRuntimeTaskData;
            {
                customRuntimeTaskData.runtimeValueData = new RuntimeValueData("gamObject");
                customRuntimeTaskData.runtimeValueData.pValue = go;
                customRuntimeTaskData.runtimeValueData.type = ValueType.Other;
            }

            customRuntimeTaskData.sync("Sync");
        }

        // RETURN A VALUE
        RuntimeValueData Value(Runtime.RuntimeTaskData runtimeData)
        {
            CustomRuntimeTaskData customRuntimeTaskData = runtimeData as CustomRuntimeTaskData;
            return customRuntimeTaskData.runtimeValueData;
        }
    }
}
