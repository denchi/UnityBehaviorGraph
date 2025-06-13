using Behaviours.Runtime;
using Behaviours.Graph.Runtime;

namespace Behaviours.Graph.Const.Primitive
{
    [Task(Title = "String")]
    [Output(Title = "Value", Type = ValueType.String)]
    public class String : Task
    {
        public string value;

        public override string Title
        {
            get
            {
                return base.Title + ":String";
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
                customRuntimeTaskData.runtimeValueData.type = ValueType.String;
                customRuntimeTaskData.runtimeValueData.sValue = value;
            }

            return customRuntimeTaskData.runtimeValueData;
        }
    }
}