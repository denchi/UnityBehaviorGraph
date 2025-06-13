using UnityEngine;
using Behaviours.Runtime;
using Behaviours.Graph.Runtime;

namespace Behaviours.Graph.Const.Primitive
{
    [Task(Title = "Vector2")]
    [Output(Title = "Value", Type = ValueType.Vector2)]
    public class Vector2Const : Task
    {
        public Vector2 value;

        public class CustomRuntimeTaskData : RuntimeTaskData
        {
            public RuntimeValueData runtimeValueData;
        }

        public override RuntimeTaskData createTaskData() => new CustomRuntimeTaskData { runtimeValueData = new RuntimeValueData("Value", ValueType.Vector2) };

        RuntimeValueData Value(Runtime.RuntimeTaskData runtimeData)
        {
            CustomRuntimeTaskData customRuntimeTaskData = runtimeData as CustomRuntimeTaskData;

            customRuntimeTaskData.runtimeValueData.pValue = value;

            return customRuntimeTaskData.runtimeValueData;
        }
    }
}