using Behaviours.Runtime;
using Behaviours.Graph.Runtime;

namespace Behaviours.Graph
{
    [Task(Title = "ForEach")]
    [Input(Title = "Sync", Order = 0)]
    [Input(Title = "Array", Type = ValueType.Array, Order = 1)]
    [Output(Title = "Tick", Order = 0)]
    [Output(Title = "OnFinished", Order = 1)]
    [Output(Title = "Value", Type = ValueType.Other, Order = 2)]
    public class ForEach : Task
    {
        public class CustomRuntimeTaskData : RuntimeTaskData
        {
            public int index;
            public RuntimeValueData runtimeValueData;
            public RuntimeValueData runtimeArray;
        }

        public override RuntimeTaskData createTaskData() { return new CustomRuntimeTaskData(); }

        void Sync(Runtime.RuntimeTaskData runtimeData)
        {
            CustomRuntimeTaskData customRuntimeTaskData = runtimeData as CustomRuntimeTaskData;
            {
                customRuntimeTaskData.runtimeArray = runtimeData.getValue("Array");

                if (customRuntimeTaskData.runtimeArray.AsList != null && customRuntimeTaskData.runtimeArray.AsList.Count > 0)
                {
                    customRuntimeTaskData.runtimeValueData = customRuntimeTaskData.runtimeArray.AsList[0];
                }
            }
            runtimeData.sync("Tick");
        }

        RuntimeValueData Value(Runtime.RuntimeTaskData runtimeData)
        {
            CustomRuntimeTaskData customRuntimeTaskData = runtimeData as CustomRuntimeTaskData;
            return customRuntimeTaskData.runtimeValueData;
        }
    }
}