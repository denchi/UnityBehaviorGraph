using Behaviours.Runtime;
using Behaviours.Graph.Runtime;

namespace Behaviours.Graph.Const.Primitive
{
    [Task(Title = "Bool")]
    [Output(Title = "Value", Type = ValueType.Bool)]
    public class Bool : Task
    {
        public bool value;

        public override string Title
        {
            get
            {
                return base.Title + ":Bool";
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
                customRuntimeTaskData.runtimeValueData = new RuntimeValueData("Result");
                customRuntimeTaskData.runtimeValueData.type = ValueType.Bool;
                customRuntimeTaskData.runtimeValueData.bValue = value;
            }

            return customRuntimeTaskData.runtimeValueData;
        }
    }
}