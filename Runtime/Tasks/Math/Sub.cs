using Behaviours.Graph.Runtime;
using Behaviours.Runtime;

namespace Behaviours.Graph.Math
{
    [Task(Title = "Sub")]
    [Input(Title = "A", Type = ValueType.Float, Order = 0)]
    [Input(Title = "B", Type = ValueType.Float, Order = 1)]
    [Output(Title = "Result", Type = ValueType.Float, Order = 0)]
    public class Sub : Task
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
            RuntimeValueData b = customRuntimeTaskData.getValue("B");

            if (customRuntimeTaskData.runtimeValueData == null)
            {
                customRuntimeTaskData.runtimeValueData = new RuntimeValueData("Result");
                customRuntimeTaskData.runtimeValueData.type = ValueType.Float;
            }

            customRuntimeTaskData.runtimeValueData.fValue = (a != null ? a.fValue : 0) - (b != null ? b.fValue : 0);

            return customRuntimeTaskData.runtimeValueData;
        }
    }
}
