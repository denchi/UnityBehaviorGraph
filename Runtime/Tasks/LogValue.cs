using UnityEngine;
using Behaviours.Graph.Runtime;
using Behaviours.Runtime;

namespace Behaviours.Graph
{
    [Task(Title = "LogValue")]
    [Input(Title = "Sync", Order = 0)]
    [Input(Title = "Value", Type = ValueType.Other, Order = 1)]
    [Output(Title = "Sync", Order = 0)]
    public class LogValue : Task
    {
        public class CustomRuntimeTaskData : RuntimeTaskData { }

        void Sync(Runtime.RuntimeTaskData runtimeData)
        {
            RuntimeValueData value = runtimeData.getValue("Value");

            if (value != null)
            {
                Debug.LogFormat("Value : {0} = {1}", value.type, value.ToObject());
            }
            else
            {
                Debug.Log("Value is NULL");
            }

            runtimeData.sync("Sync");
        }

        // RETURN A VALUE
        //RuntimeValueData Value(Runtime.RuntimeTaskData runtimeData)
        //{
        //    CustomRuntimeTaskData customRuntimeTaskData = runtimeData as CustomRuntimeTaskData;
        //    return customRuntimeTaskData.runtimeValueData;
        //}
    }
}
