using Behaviours.Graph.Runtime;
using Behaviours.Runtime;

namespace Behaviours.Graph
{
    [Task(Title = "Set Var")]
    [Input(Title = "Sync", Order = 0)]
    [Input(Title = "value", Type = ValueType.Other, Order = 1)]
    [Output(Title = "Sync", Order = 0)]
    public class SetVar : Task
    {
        public string valueId;

        public override string Title
        {
            get
            {
                return "Set Var: [" + valueId + "]";
            }
        }

        public class CustomRuntimeTaskData : RuntimeTaskData
        {
            public RuntimeValueData runtimeValue;
        }

        public override RuntimeTaskData createTaskData() { return new CustomRuntimeTaskData(); }

        void Set(Runtime.RuntimeTaskData runtimeData)
        {
            CustomRuntimeTaskData customRuntimeTaskData = runtimeData as CustomRuntimeTaskData;
            if (customRuntimeTaskData.runtimeValue == null)
            {
                customRuntimeTaskData.runtimeValue = runtimeData.runtimeGraph.findRuntimeValue(valueId);
            }

            RuntimeValueData value = runtimeData.getValue("value");

            customRuntimeTaskData.runtimeValue = value;

            runtimeData.sync("Sync");
        }
    }
}