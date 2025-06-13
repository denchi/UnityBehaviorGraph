using UnityEngine;
using System.Collections.Generic;

using UnityEditor;

namespace Behaviours.Graph
{
    namespace Editor
    {
        public class GraphUtils
        {
            #region MENU

            [MenuItem("Behaviour/Graph/Create Graph Asset...")]
            static void BG_NEW_GRAPH()
            {
                string filePath = EditorUtility.SaveFilePanelInProject("Select a Graph path...", "NewGraph", "asset", "Creating a new graph...");
                if (string.IsNullOrEmpty(filePath) == false)
                {
                    createGraph(filePath);

                    AssetDatabase.ImportAsset(filePath);
                }
            }

            [MenuItem("Behaviour/Graph/New Task Script...")]
            static void BG_NEW_TASK()
            {
                string filePath = EditorUtility.SaveFilePanelInProject("Select task file path...", "NewTask", "cs", "Creating a new Task...");
                if (string.IsNullOrEmpty(filePath) == false)
                {
                    string[] template = new string[]
                    {
                "using UnityEngine;",
                "using System.Collections;",
                "using System.Collections.Generic;",
                "",
                "using Behaviours.Graph;",
                "using Behaviours.Graph.Runtime;",
                "using Behaviours.HFSM.Runtime;",
                "",
                "namespace Behaviours.Graph",
                "{",
                "    [Task(Title = \"[FILENAME]\")]",
                "    [Input(Title = \"Sync\", Order = 0)]",
                "    //[Input(Title = \"Value\", Type = ValueType.Array, Order = 1)]",
                "    [Output(Title = \"Sync\", Order = 0)]",
                "    //[Output(Title = \"Value\", Type = ValueType.Other, Order = 2)]",
                "    public class [FILENAME] : Task",
                "    {",
                "        public class CustomRuntimeTaskData : RuntimeTaskData",
                "        {",
                "            // public RuntimeValueData runtimeValueData;",
                "        }",
                "",
                "        public override RuntimeTaskData createTaskData() { return new CustomRuntimeTaskData(); }",
                "",
                "        void Sync(Runtime.RuntimeTaskData runtimeData)",
                "        {",
                "            CustomRuntimeTaskData customRuntimeTaskData = runtimeData as CustomRuntimeTaskData;",
                "            //runtimeData.sync(\"Sync\");",
                "        }",
                "",
                "        // RETURN A VALUE",
                "        //RuntimeValueData Value(Runtime.RuntimeTaskData runtimeData)",
                "        //{",
                "        //    CustomRuntimeTaskData customRuntimeTaskData = runtimeData as CustomRuntimeTaskData;",
                "        //    return customRuntimeTaskData.runtimeValueData;",
                "        //}",
                "    }",
                "}"
                    };

                    string fileData = "";
                    foreach (string line in template)
                    {
                        fileData += line.Replace("[FILENAME]", System.IO.Path.GetFileNameWithoutExtension(filePath)) + System.Environment.NewLine;
                    }

                    System.IO.File.WriteAllText(filePath, fileData);

                    AssetDatabase.ImportAsset(filePath);
                }
            }

            #endregion

            #region CREATE

            public static Graph createGraph(string assetPath)
            {
                Graph graph = CreateAsset<Graph>(assetPath);
                {
                    graph.name = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                    graph.root = createNode();
                    {
                        graph.root.name = "Root";
                        graph.root.graph = graph;
                    }

                    AssetDatabase.AddObjectToAsset(graph.root, graph);

                    graph.root.task = createTask(typeof(Composite), graph.root);
                }
                EditorUtility.SetDirty(graph);

                EditorUtility.SetDirty(graph.root);

                return graph;
            }

            public static Node createNode(Composite parent = null)
            {
                Node node = ScriptableObject.CreateInstance<Node>();
                {
                    node.title = "Node";
                }

                node.rect = new Rect(0, 0, 200, 50);

                if (parent)
                {
                    node.graph = parent.node.graph;
                    node.parent = parent.node;

                    parent.nodes.Add(node);

                    AssetDatabase.AddObjectToAsset(node, node.graph);

                    EditorUtility.SetDirty(parent);
                }

                EditorUtility.SetDirty(node);

                return node;
            }

            public static Task createTask(System.Type stateType, Node node)
            {
                if (node.task)
                {
                    removeTask(node.task);
                }

                Task task = ScriptableObject.CreateInstance(stateType) as Task;
                {
                    TaskAttribute ta = getAttribute<TaskAttribute>(stateType);
                    if (ta != null)
                    {
                        task.title = ta.Title;
                    }
                    else
                    {
                        task.title = stateType.Name;
                    }

                    task.node = node;
                    task.hideFlags = HideFlags.HideInHierarchy;                    
                }

                node.task = task;

                createTaskAttributes(node.task);

                if (task is Composite)
                {
                    Composite comp = task as Composite;

                    Node defaultNode = createNode(comp);
                    {
                        defaultNode.title = "Start";
                        defaultNode.rect.position = new Vector2(100 + (defaultNode.rect.width + 20) * 0, 200);
                        createTask(typeof(Start), defaultNode);
                    }

                    if (node.parent)
                    {
                        Node exitNode = createNode(comp);
                        {
                            exitNode.title = "Exit";
                            exitNode.rect.position = new Vector2(100 + (exitNode.rect.width + 20) * 2, 200);
                            createTask(typeof(End), exitNode);
                        }                        
                    }
                }

                AssetDatabase.AddObjectToAsset(task, node.graph);

                EditorUtility.SetDirty(node);

                return task;
            }

            public static void createTaskAttributes(Task task)
            {
                #region INPUTS
                {
                    List<InputAttribute> attrs = getInputAttributes(task.GetType());

                    if (task.inputs == null)
                    {
                        task.inputs = new List<Input>();
                    }

                    for (int i = 0; i < attrs.Count; ++i)
                    {
                        Input input = task.inputs.Find(x => x.title == attrs[i].Title);
                        if (input == null)
                        {
                            input = ScriptableObject.CreateInstance<Input>();
                            AssetDatabase.AddObjectToAsset(input, task.node.graph);
                        }

                        input.task = task;
                        input.type = attrs[i].Type;
                        input.title = attrs[i].Title;
                        input.order = attrs[i].Order;

                        task.inputs.Add(input);
                    }

                    List<Input> inputsToDelete = new List<Input>();
                    for (int i = 0; i < task.inputs.Count; ++i)
                    {
                        if (attrs.FindIndex(x => x.Title == task.inputs[i].title) == -1)
                        {
                            inputsToDelete.Add(task.inputs[i]);
                        }
                    }

                    for (int i = 0; i < inputsToDelete.Count; ++i)
                    {
                        removeInput(task, inputsToDelete[i]);
                    }

                    task.outputs.Sort((a, b) => a.order.CompareTo(b.order));
                }
                #endregion

                #region OUTPUTS
                {
                    List<OutputAttribute> attrs = getOutputAttributes(task.GetType());

                    if (task.outputs == null)
                    {
                        task.outputs = new List<Output>();
                    }

                    for (int i = 0; i < attrs.Count; ++i)
                    {
                        Output output = task.outputs.Find(x => x.title == attrs[i].Title);
                        if (output == null)
                        {
                            output = ScriptableObject.CreateInstance<Output>();
                            AssetDatabase.AddObjectToAsset(output, task.node.graph);
                        }

                        output.task = task;
                        output.type = attrs[i].Type;
                        output.title = attrs[i].Title;
                        output.order = attrs[i].Order;

                        EditorUtility.SetDirty(output);

                        task.outputs.Add(output);
                    }

                    List<Output> outputsToDelete = new List<Output>();
                    for (int i = 0; i < task.outputs.Count; ++i)
                    {
                        if (attrs.FindIndex(x => x.Title == task.outputs[i].title) == -1)
                        {
                            outputsToDelete.Add(task.outputs[i]);
                        }
                    }

                    for (int i = 0; i < outputsToDelete.Count; ++i)
                    {
                        removeOutput(task, outputsToDelete[i]);
                    }

                    task.inputs.Sort((a, b) => a.order.CompareTo(b.order));
                }
                #endregion

                EditorUtility.SetDirty(task);
            }            

            #endregion

            #region REMOVE        

            public static void removeNode(Node node, Composite parent = null)
            {
                if (node)
                {
                    if (parent)
                    {
                        parent.nodes.Remove(node);
                        EditorUtility.SetDirty(parent);
                    }                    

                    removeTask(node.task);

                    Object.DestroyImmediate(node, true);
                }
            }

            public static void removeTask(Task task, Node parent = null)
            {
                if (task)
                {
                    if (task is Composite)
                    {
                        Composite comp = task as Composite;
                        for (int i = 0; i < comp.nodes.Count; ++i)
                        {
                            removeNode(comp.nodes[i]);
                        }                        
                    }

                    if (parent)
                    {
                        parent.task = null;
                        EditorUtility.SetDirty(parent);
                    }

                    List<Input> inputsToDelete = new List<Input>(task.inputs);
                    for (int i = 0; i < inputsToDelete.Count; ++i)
                    {
                        removeInput(task, inputsToDelete[i]);
                    }

                    List<Output> outputsToDelete = new List<Output>(task.outputs);
                    for (int i = 0; i < outputsToDelete.Count; ++i)
                    {
                        removeOutput(task, outputsToDelete[i]);
                    }

                    Object.DestroyImmediate(task, true);
                }
            }

            public static void removeInput(Task task, Input input)
            {
                if (task && task.node && task.node.parent && task.node.parent.task is Composite)
                { 
                    Composite comp = task.node.parent.task as Composite;

                    foreach (Node tempNode in comp.nodes)
                    {
                        if (tempNode.task)
                        {
                            Task tempTask = tempNode.task;
                            foreach (Output tempOutput in tempTask.outputs)
                            {
                                if (tempOutput.input == input)
                                {
                                    tempOutput.input = null;
                                }
                            }
                        }
                    }
                }

                if (task)
                {
                    task.inputs.Remove(input);

                    Object.DestroyImmediate(input, true);                    
                }
            }            

            public static void removeOutput(Task task, Output output)
            {
                if (task && task.node && task.node.parent && task.node.parent.task is Composite)
                {
                    Composite comp = task.node.parent.task as Composite;

                    foreach (Node tempNode in comp.nodes)
                    {
                        if (tempNode.task)
                        {
                            Task tempTask = tempNode.task;
                            foreach (Input tempInput in tempTask.inputs)
                            {
                                if (tempInput.output == output)
                                {
                                    tempInput.output = null;
                                }
                            }
                        }
                    }
                }

                if (task)
                {
                    task.outputs.Remove(output);

                    Object.DestroyImmediate(output, true);
                }
            }

            #endregion

            #region MISC

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            static T1 CreateAsset<T1>(string assetPath) where T1 : ScriptableObject
            {
                T1 asset = ScriptableObject.CreateInstance<T1>();

                string assetFileName = AssetDatabase.GenerateUniqueAssetPath(System.IO.Path.GetDirectoryName(assetPath) + "/" + System.IO.Path.GetFileNameWithoutExtension(assetPath) + ".asset");

                AssetDatabase.CreateAsset(asset, assetFileName);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;

                return asset;
            }

            #endregion

            public static List<T> getAttributes<T>(System.Type type) where T : System.Attribute, new()
            {
                List<T> list = new List<T>();
                object[] attributes = type.GetCustomAttributes(typeof(T), true);
                if (attributes != null)
                {
                    foreach (object attr in attributes)
                    {
                        if (attr is T)
                        {
                            list.Add(attr as T);
                        }
                    }
                }
                return list;
            }

            public static T getAttribute<T>(System.Type type) where T : System.Attribute, new()
            {
                object[] attributes = type.GetCustomAttributes(typeof(T), true);
                if (attributes != null)
                {
                    foreach (object attr in attributes)
                    {
                        if (attr is T)
                        {
                            return  attr as T;
                        }
                    }
                }
                return null;
            }

            public static List<InputAttribute> getInputAttributes(System.Type type)
            {
                return getAttributes<InputAttribute>(type);
            }

            public static List<OutputAttribute> getOutputAttributes(System.Type type)
            {
                return getAttributes<OutputAttribute>(type);
            }

            public static List<FuncAttribute> getFuncAttributes(System.Type type)
            {
                return getAttributes<FuncAttribute>(type);
            }

            public static List<EventAttribute> getEventAttributes(System.Type type)
            {
                return getAttributes<EventAttribute>(type);
            }            

            //        public static void attachTask(Node node, ITask task)
            //        {
            //            if (node.task != null)
            //            {
            //#if UNITY_EDITOR
            //                ScriptableObject.DestroyImmediate(node.task);
            //                node.task = null;
            //#endif
            //            }

            //            node.task = task;

            //#if UNITY_EDITOR
            //            UnityEditor.EditorUtility.SetDirty(node);
            //#endif

            //            if (node.task)
            //            {
            //                #region INPUTS

            //                List<Input> _inputs = new List<Input>();

            //                if (node.inputs == null)
            //                {
            //                    node.inputs = new List<Input>();
            //                }

            //                #region EVENTS
            //                {
            //                    List<FuncAttribute> list = getFuncAttributes(task.GetType());
            //                    foreach (FuncAttribute attr in list)
            //                    {
            //                        Input temp = node.getInput(attr.Title);
            //                        if (temp == null)
            //                        {
            //                            temp = createInput(attr.Title, ValueType.None, node, null);
            //                        }

            //                        _inputs.Add(temp);
            //                    }
            //                }

            //                #endregion

            //                #region GET VALUES
            //                {
            //                    List<InputAttribute> list = GraphUtils.getInputAttributes(task.GetType());
            //                    foreach (InputAttribute attr in list)
            //                    {
            //                        Input temp = node.getInput(attr.Title);
            //                        if (temp == null)
            //                        {
            //                            createOutput(attr.Title, attr.Type, node, null);
            //                        }

            //                        _inputs.Add(temp);
            //                    }
            //                }
            //                #endregion

            //                List<Input> inputsToDelete = node.inputs.FindAll(x => _inputs.Contains(x) == false);
            //                foreach (Input temp in inputsToDelete)
            //                {
            //                    if (temp != null)
            //                    {
            //                        temp.linkedOutput.linkedInput = null;
            //                    }

            //                    node.inputs.Remove(temp);
            //                }

            //                #endregion

            //                #region OUTPUTS

            //                if (node.outputs == null)
            //                {
            //                    node.outputs = new List<Output>();
            //                }

            //                List<Output> _outputs = new List<Output>();

            //                #region EVENTS
            //                {
            //                    List<EventAttribute> list = GraphUtils.getEventAttributes(task.GetType());
            //                    foreach (EventAttribute attr in list)
            //                    {
            //                        Output temp = node.getOutput(attr.Title);
            //                        if (temp == null)
            //                        {
            //                            temp = createOutput(attr.Title, ValueType.None, node, null);
            //                        }

            //                        _outputs.Add(temp);
            //                    }
            //                }

            //                #endregion

            //                #region GET VALUES
            //                {
            //                    List<OutputAttribute> list = GraphUtils.getOutputAttributes(task.GetType());
            //                    foreach (OutputAttribute attr in list)
            //                    {
            //                        Output temp = node.getOutput(attr.Title);
            //                        if (temp == null)
            //                        {
            //                            temp = createOutput(attr.Title, attr.Type, node, null);
            //                        }

            //                        _outputs.Add(temp);
            //                    }
            //                }

            //                #endregion

            //                List<Output> outputsToDelete = node.outputs.FindAll(x => _outputs.Contains(x) == false);
            //                foreach (Output temp in outputsToDelete)
            //                {
            //                    if (temp != null)
            //                    {
            //                        temp.linkedInput.linkedOutput = null;
            //                    }

            //                    node.outputs.Remove(temp);
            //                }

            //                #endregion
            //            }
            //        }
        }
    }    
}