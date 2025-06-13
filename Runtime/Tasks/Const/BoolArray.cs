using System.Collections.Generic;

using Behaviours.Runtime;
using Behaviours.Graph.Runtime;

namespace Behaviours.Graph.Const.Array
{
    [Task(Title = "BoolArray")]
    [Output(Title = "Value", Type = ValueType.Array)]
    public class BoolArray : Task
    {
        public List<bool> values;

        public override string Title
        {
            get
            {
                return "Bool:String";
            }
        }

        public class CustomRuntimeTaskData : RuntimeTaskData
        {
            public RuntimeValueData runtimeValueData;
        }

        public override RuntimeTaskData createTaskData() { return new CustomRuntimeTaskData(); }

        RuntimeValueData Value(Runtime.RuntimeTaskData runtimeData)
        {
            CustomRuntimeTaskData customRuntimeTaskData = runtimeData as CustomRuntimeTaskData;
            if (customRuntimeTaskData.runtimeValueData == null)
            {
                customRuntimeTaskData.runtimeValueData = new RuntimeValueData("Value");
                customRuntimeTaskData.runtimeValueData.type = ValueType.Array;
                customRuntimeTaskData.runtimeValueData.pValue = new List<RuntimeValueData>();
                for (int i = 0; i < values.Count; ++i)
                {
                    (customRuntimeTaskData.runtimeValueData.pValue as List<RuntimeValueData>).Add(new RuntimeValueData(i.ToString()));
                    (customRuntimeTaskData.runtimeValueData.pValue as List<RuntimeValueData>)[i].type = ValueType.Bool;
                    (customRuntimeTaskData.runtimeValueData.pValue as List<RuntimeValueData>)[i].bValue = values[i];
                }
            }

            return customRuntimeTaskData.runtimeValueData;
        }
    }
}