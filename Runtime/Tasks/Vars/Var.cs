using Behaviours.Graph.Runtime;
using Behaviours.Runtime;

namespace Behaviours.Graph
{
    [Task(Title = "Var")]
    [Output(Title = "value", Type = ValueType.Other, Order = 1)]
    public class Var : Task
    {
        public string valueId;

        public override string Title
        {
            get
            {
                return "Var [" + valueId + "]";
            }
        }

        public class CustomRuntimeTaskData : RuntimeTaskData
        {
            public RuntimeValueData runtimeValue;
        }

        public override RuntimeTaskData createTaskData() => new CustomRuntimeTaskData();

        RuntimeValueData value(Runtime.RuntimeTaskData runtimeData)
        {
            CustomRuntimeTaskData customRuntimeTaskData = runtimeData as CustomRuntimeTaskData;
            if (customRuntimeTaskData.runtimeValue == null)
            {
                customRuntimeTaskData.runtimeValue = runtimeData.runtimeGraph.findRuntimeValue(valueId);
            }

            return customRuntimeTaskData.runtimeValue;
        }
    }
}