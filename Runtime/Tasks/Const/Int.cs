using Behaviours.Runtime;
using Behaviours.Graph.Runtime;

namespace Behaviours.Graph.Const.Primitive
{
    [Task(Title = "Int")]
    [Output(Title = "Value", Type = ValueType.Integer)]
    public class Int : Task
    {
        public int value;

        public override string Title
        {
            get
            {
                return base.Title + ":Int";
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
                customRuntimeTaskData.runtimeValueData.type = ValueType.Integer;
                customRuntimeTaskData.runtimeValueData.iValue = value;
            }

            return customRuntimeTaskData.runtimeValueData;
        }
    }
}