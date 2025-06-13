using UnityEngine;
using Behaviours.Graph.Runtime;
using Behaviours.Runtime;

namespace Behaviours.Graph.Math
{
    [Task(Title = "Random")]
    [Input(Title = "a", Type = ValueType.Other, Order = 0)]
    [Input(Title = "b", Type = ValueType.Other, Order = 1)]
    [Output(Title = "result", Type = ValueType.Other, Order = 0)]
    public class Random : Task
    {
        public class CustomRuntimeTaskData : RuntimeTaskData
        {
            public RuntimeValueData runtimeValueData;
        }

        public override RuntimeTaskData createTaskData() => new CustomRuntimeTaskData { runtimeValueData = new RuntimeValueData("result", ValueType.Other) };

        RuntimeValueData Result(Runtime.RuntimeTaskData runtimeData)
        {
            CustomRuntimeTaskData customRuntimeTaskData = runtimeData as CustomRuntimeTaskData;

            RuntimeValueData a = customRuntimeTaskData.getValue("a");
            RuntimeValueData b = customRuntimeTaskData.getValue("b");

            if (a.type == b.type)
            {
                switch (a.type)
                {
                    case ValueType.Float: customRuntimeTaskData.runtimeValueData.type = ValueType.Float; customRuntimeTaskData.runtimeValueData.fValue = UnityEngine.Random.Range(a.fValue, b.fValue); break;
                    case ValueType.Integer: customRuntimeTaskData.runtimeValueData.type = ValueType.Integer; customRuntimeTaskData.runtimeValueData.iValue = UnityEngine.Random.Range(a.iValue, b.iValue); break;
                    case ValueType.Vector2:
                        {
                            var v1 = (Vector2)a.pValue;
                            var v2 = (Vector2)b.pValue;
                            customRuntimeTaskData.runtimeValueData.type = ValueType.Vector2; customRuntimeTaskData.runtimeValueData.pValue = new Vector2(UnityEngine.Random.Range(v1.x, v2.x), UnityEngine.Random.Range(v1.y, v2.y));
                        }
                        break;

                    case ValueType.Vector3:
                        {
                            var v1 = (Vector3)a.pValue;
                            var v2 = (Vector3)b.pValue;
                            customRuntimeTaskData.runtimeValueData.type = ValueType.Vector3; customRuntimeTaskData.runtimeValueData.pValue = new Vector3(UnityEngine.Random.Range(v1.x, v2.x), UnityEngine.Random.Range(v1.y, v2.y), UnityEngine.Random.Range(v1.z, v2.z));
                        }
                        break;
                }
            }

            return customRuntimeTaskData.runtimeValueData;
        }
    }
}
