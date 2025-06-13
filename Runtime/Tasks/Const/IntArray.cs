using System.Collections.Generic;

using Behaviours.Runtime;
using Behaviours.Graph.Runtime;

namespace Behaviours.Graph.Const.Array
{
    [Task(Title = "IntArray")]
    [Output(Title = "Value", Type = ValueType.Array)]
    public class IntArray : Task
    {
        public List<int> values;

        public override string Title
        {
            get
            {
                return "Int:String";
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
                    (customRuntimeTaskData.runtimeValueData.pValue as List<RuntimeValueData>)[i].type = ValueType.Integer;
                    (customRuntimeTaskData.runtimeValueData.pValue as List<RuntimeValueData>)[i].iValue = values[i];
                }
            }

            return customRuntimeTaskData.runtimeValueData;
        }
    }
}