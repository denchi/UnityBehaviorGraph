using UnityEngine;
using Behaviours.Graph.Runtime;
using Behaviours.Runtime;

namespace Behaviours.Graph.Math
{
    [Task(Title = "Mul")]
    [Input(Title = "A", Type = ValueType.Float, Order = 1)]
    [Input(Title = "Power", Type = ValueType.Float, Order = 2)]
    [Output(Title = "Result", Type = ValueType.Float, Order = 1)]
    public class Pow : Task
    {
        public class CustomRuntimeTaskData : RuntimeTaskData
        {
            public RuntimeValueData runtimeValueData;
        }

        public override RuntimeTaskData createTaskData() { return new CustomRuntimeTaskData(); }

        RuntimeValueData Result(Runtime.RuntimeTaskData runtimeData)
        {
            CustomRuntimeTaskData customRuntimeTaskData = runtimeData as CustomRuntimeTaskData;

            RuntimeValueData a = customRuntimeTaskData.getValue("A");
            RuntimeValueData b = customRuntimeTaskData.getValue("Power");

            if (customRuntimeTaskData.runtimeValueData == null)
            {
                customRuntimeTaskData.runtimeValueData = new RuntimeValueData("Result");
                customRuntimeTaskData.runtimeValueData.type = ValueType.Float;
            }

            customRuntimeTaskData.runtimeValueData.fValue = Mathf.Pow((a != null ? a.fValue : 1), (b != null ? b.fValue : 1));

            return customRuntimeTaskData.runtimeValueData;
        }
    }
}
