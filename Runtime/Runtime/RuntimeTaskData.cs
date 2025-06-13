using UnityEngine;
using System.Collections.Generic;

using Behaviours.Runtime;

namespace Behaviours.Graph.Runtime
{
    public class RuntimeGroupData : RuntimeTaskData
    {
        public Dictionary<Task, RuntimeTaskData> runtimeTasks;
        public RuntimeTaskData defaultRuntimeTaskData;

        public override void setTask(Task task)
        {
            base.setTask(task);

            runtimeTasks = new Dictionary<Task, RuntimeTaskData>();

            Composite comp = task as Composite;

            // CREATE RUNTIME TASKS
            foreach (Node node in comp.nodes)
            {
                if (node.task)
                {
                    runtimeTasks[node.task] = node.task.createTaskData();

                    runtimeTasks[node.task].runtimeParent = this;

                    runtimeTasks[node.task].runtimeGraph = runtimeGraph;

                    if (node.task is Start)
                    {
                        defaultRuntimeTaskData = runtimeTasks[node.task];
                    }
                }
            }

            // SET RUNTIME TASKS LINKS
            foreach (Node node in comp.nodes)
            {
                if (node.task)
                {                    
                    runtimeTasks[node.task].setTask(node.task);
                }
            }
        }
    }

    public class RuntimeTaskAnchorInfo
    {
        public Task task;
        public System.Reflection.MethodInfo method;
        public RuntimeTaskData runtimeTaskData;
    }

    public class RuntimeTaskData
    {
        public RuntimeGroupData runtimeParent;
        public RuntimeGraphData runtimeGraph;

        public Task task;

        public Dictionary<string, RuntimeTaskAnchorInfo> runtimeOutputs;
        public Dictionary<string, RuntimeTaskAnchorInfo> runtimeInputs;

        public virtual void setTask(Task task)
        {
            this.task = task;

            runtimeOutputs = new Dictionary<string, RuntimeTaskAnchorInfo>();
            foreach (Output temp in task.outputs)
            {
                if (temp)
                {
                    string outputName = temp.title;

                    if (temp.input && temp.input.type == ValueType.None)
                    {
                        if (temp.input.task)
                        {
                            RuntimeTaskAnchorInfo outputInfo = new RuntimeTaskAnchorInfo();

                            RuntimeTaskData rtd = runtimeParent.runtimeTasks[temp.input.task];

                            System.Reflection.MethodInfo mi = temp.input.task.GetType().GetMethod(temp.input.title, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            if (mi != null)
                            {
                                outputInfo.task = temp.input.task;
                                outputInfo.method = mi;
                                outputInfo.runtimeTaskData = rtd;

                                runtimeOutputs[outputName] = outputInfo;
                            }
                            else
                            {
                                Debug.LogErrorFormat("Task {0} has no proc {1} [{2}]", temp.input.task.title, temp.input.title, task.title);
                            }
                        }
                        else
                        {
                            Debug.LogErrorFormat("Task {0} output named {1} has no task [{2}]", temp.task.title, outputName, task.title);
                        }
                    }
                    //else
                    //{
                    //    Debug.LogErrorFormat("Task {0} output named {1} has a null input", task.title, outputName);
                    //}
                }
                else
                {
                    Debug.LogErrorFormat("Task {0} has a NULL output", task.title);
                }
            }

            runtimeInputs = new Dictionary<string, RuntimeTaskAnchorInfo>();
            foreach (Input temp in task.inputs)
            {
                if (temp)
                {
                    string inputName = temp.title;

                    if (temp.output )
                    {
                        if (temp.type != ValueType.None)
                        {
                            if (temp.output.task)
                            {
                                RuntimeTaskAnchorInfo inputInfo = new RuntimeTaskAnchorInfo();

                                RuntimeTaskData rtd = runtimeParent.runtimeTasks[temp.output.task];

                                System.Reflection.MethodInfo mi = temp.output.task.GetType().GetMethod(temp.output.title, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                                if (mi != null)
                                {
                                    inputInfo.task = temp.output.task;
                                    inputInfo.method = mi;
                                    inputInfo.runtimeTaskData = rtd;

                                    runtimeInputs[inputName] = inputInfo;

                                    Debug.LogFormat("{0}:: Task {1} has func {2} for input {0}::{3}", task.title, temp.output.task.title, mi.Name, temp.title);
                                }
                                else
                                {
                                    Debug.LogErrorFormat("{0}:: Task {1} has no func {2}", task.title, temp.output.task.title, temp.output.title);
                                }
                            }
                            else
                            {
                                Debug.LogErrorFormat("{0}:: Output named {1} has no task", task.title, temp.output.title);
                            }
                        }
                        else
                        {
                            Debug.LogFormat("{0}:: Skipping Input named {1} as it is of Type {2}", task.title, temp.title, temp.type);
                        }
                    }
                    else
                    {
                        Debug.LogFormat("{0}:: Input named {1} has a NULL output", task.title, temp.title);
                    }
                }
                else
                {
                    Debug.LogErrorFormat("Task {0} has a NULL input", task.title);
                }
            }
        }

        public void sync(string outputName)
        {
            if (runtimeOutputs.ContainsKey(outputName))
            {
                RuntimeTaskAnchorInfo outputInfo = runtimeOutputs[outputName];

                sync(outputInfo.task, outputInfo.method, outputInfo.runtimeTaskData);
            }
            else
            {
                Debug.LogWarningFormat("{0}:: There is no output named {1}", task.title, outputName);
            }
        }

        public static void sync(Task task, System.Reflection.MethodInfo method, RuntimeTaskData runtimeTask)
        {
            method.Invoke(task, new object[] { runtimeTask });
        }

        public T find<T>() where T : Task, new()
        {
            Composite comp = task as Composite;
            foreach(Node n in comp.nodes)
            {
                if (n.task is T)
                {
                    return n.task as T;
                }
            }
            return null;
        }

        public RuntimeValueData getValue(string inputName)
        {
            if (runtimeInputs.ContainsKey(inputName))
            {
                RuntimeTaskAnchorInfo inputInfo = runtimeInputs[inputName];

                return inputInfo.method.Invoke(inputInfo.task, new object[] { inputInfo.runtimeTaskData }) as RuntimeValueData;
            }
            else
            {
                Debug.LogWarningFormat("{0}:: There is no input named {1}", task.title, inputName);
            }

            return null;
        }
    }
}