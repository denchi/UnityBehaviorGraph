using Behaviours.Runtime;
using Behaviours.Graph.Runtime;

namespace Behaviours.Graph.Const.Primitive
{
    [Task(Title = "Float")]
    [Output(Title = "Value", Type = ValueType.Float)]
    public class Float : Task
    {
        public float value;

        public override string Title
        {
            get
            {
                return base.Title + ":Float [" + value.ToString() + "]";
            }
        }

        public class CustomRuntimeTaskData : RuntimeTaskData
        {
            public RuntimeValueData runtimeValueData;
        }

        public override RuntimeTaskData createTaskData() { return new CustomRuntimeTaskData(); }

        RuntimeValueData Value(RuntimeTaskData runtimeData)
        {
            CustomRuntimeTaskData customRuntimeTaskData = runtimeData as CustomRuntimeTaskData;

            if (customRuntimeTaskData.runtimeValueData == null)
            {
                customRuntimeTaskData.runtimeValueData = new RuntimeValueData("Value");
                customRuntimeTaskData.runtimeValueData.type = ValueType.Float;
                customRuntimeTaskData.runtimeValueData.fValue = value;
            }

            return customRuntimeTaskData.runtimeValueData;
        }
    }
}