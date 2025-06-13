using Behaviours.Graph.Runtime;
using Behaviours.Runtime;

namespace Behaviours.Graph
{
    [Task(Title = "VarFloat")]
    [Input(Title = "Set", Order = 0)]
    [Input(Title = "Value", Type = ValueType.Float, Order = 1)]
    [Output(Title = "Sync", Order = 0)]
    [Output(Title = "Value", Type = ValueType.Float, Order = 1)]
    public class VarFloat : Task
    {
        public string valueId;

        public override string Title
        {
            get
            {
                return "Var:Float [" + valueId + "]";
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

            RuntimeValueData value = runtimeData.getValue("Value");

            customRuntimeTaskData.runtimeValue.fValue = value.fValue;

            runtimeData.sync("Sync");
        }

        RuntimeValueData Value(Runtime.RuntimeTaskData runtimeData)
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