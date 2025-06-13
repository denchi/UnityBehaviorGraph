using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

namespace Behaviours.Graph.Editor
{
    public class GraphWindow : GenericWindow<Node>
    {
        public Graph graph;
        public Composite currentGroup;

        Flowgraph flowgraph;

        Node nodeWithTransition;
        Node nodeBuffer;

        int _DoMyWindowTab = 0;
        UnityEditor.Editor selectedStateEditor = null;

        [System.NonSerialized]
        Node lastRunningNode;

        Output draggingOutput = null;

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/Graph Window")]
        static void Init()
        {
            EditorWindow.GetWindow(typeof(GraphWindow), false);
        }        

        #region CONTEXT MENU EVENTS        

        void OnContextMenuItemClick_GraphSetState(object userData)
        {
            SetScriptInfo ssi = userData as SetScriptInfo;

            Dictionary<string, Output> oldOutputs = new Dictionary<string, Output>();
            Dictionary<string, Input> oldInputs = new Dictionary<string, Input>();

            if (ssi.node.task)
            {
                foreach (Input input in ssi.node.task.inputs)
                {
                    if (input.output)
                    {
                        oldOutputs[input.title] = input.output;
                    }
                }

                foreach (Output output in ssi.node.task.outputs)
                {
                    if (output.input)
                    {
                        oldInputs[output.title] = output.input;
                    }
                }
            }

            GraphUtils.createTask(ssi.type, ssi.node);

            if (ssi.node.task)
            {
                foreach (Input input in ssi.node.task.inputs)
                {
                    if (oldOutputs.ContainsKey(input.title))
                    {
                        input.output = oldOutputs[input.title];
                        EditorUtility.SetDirty(input);

                        input.output.input = input;
                        EditorUtility.SetDirty(input.output);
                    }
                }

                foreach (Output output in ssi.node.task.outputs)
                {
                    if (oldInputs.ContainsKey(output.title))
                    {
                        output.input = oldInputs[output.title];
                        EditorUtility.SetDirty(output);

                        output.input.output = output;
                        EditorUtility.SetDirty(output.input);
                    }
                }
            }

            Repaint();
        }

        void OnContextMenuItemClick_EnterGroup(object userData)
        {
            Node node = userData as Node;

            Composite newGroup = node.task as Composite;

            if (newGroup)
            {
                Composite oldGroup = currentGroup;

                OnGroupEnterred(oldGroup, newGroup);

                Repaint();
            }
        }        

        void OnContextMenuItemClick_RemoveState(object userData)
        {
            Node state = userData as Node;

            if (state.task != null)
            {
                GraphUtils.removeTask(state.task, state);
            }

            if (currentGroup)
            {
                UnityEditor.EditorUtility.SetDirty(currentGroup);
            }
            else
            {
                UnityEditor.EditorUtility.SetDirty(graph);
            }

            Repaint();
        }

        void OnContextMenuItemClick_CreateTransition(object userData)
        {
            nodeWithTransition = userData as Node;
        }

        void OnContextMenuItemClick_NewGraphNode(object userData)
        {
            Graph layer = this.graph as Graph;

            Vector2 mousePos = (Vector2)userData;

            AddEmptyItem(mousePos);
        }

        void OnContextMenuItemClick_ExitGroup(object userData)
        {
            Composite state = userData as Composite;

            if (state && state.node && state.node.parent && state.node.parent.task is Composite)
            {
                currentGroup = state.node.parent.task as Composite;
            }

            Repaint();
        }

        void OnContextMenuItemClick_NewGraphGroup(object userData)
        {
            Node node = userData as Node;

            GraphUtils.createTask(typeof(Composite), node);
        }

        void OnContextMenuItemClick_DeleteGraphNode(object userData)
        {
            Node node = userData as Node;

            GraphUtils.removeNode(node, currentGroup);
        }

        void OnContextMenuItemClick_SelectAsset(object userData)
        {
            Node state = userData as Node;

            Selection.activeObject = state;
        }

        #endregion

        #region MISC

        void setFlowgraph(Flowgraph flowgraph)
        {
            if (this.flowgraph)
            {

            }

            this.flowgraph = flowgraph;

            if (this.flowgraph)
            {
                setGraph(this.flowgraph.graph);
            }
        }

        void setGraph(Graph newGraph)
        {
            graph = newGraph;

            if (graph && graph.root && graph.root.task is Composite)
            {
                currentGroup = graph.root.task as Composite;                
            }
            else
            {
                currentGroup = null;
            }

            OnSelectedGroupChanged();
        }

        void setGroup(Composite group)
        {
            currentGroup = group;

            OnSelectedGroupChanged();
        }

        void OnSelectedLayerChanged()
        {

        }

        void OnSelectedGroupChanged()
        {

        }

        void OnGroupEnterred(Composite oldGroup, Composite newGroup)
        {
            if (newGroup)
            {
                clearSelection();

                currentGroup = newGroup;
            }
        }

        void DrawNodeCurve(Vector2 start, Vector2 end, Color color, float width)
        {
            Vector3 startPos = new Vector3(start.x, start.y, 0);
            Vector3 endPos = new Vector3(end.x, end.y, 0);

            Vector3 startTan = startPos + Vector3.right * Vector2.Distance(startPos, endPos) / 2;
            Vector3 endTan = endPos + Vector3.left * Vector2.Distance(startPos, endPos) / 2;

            Color shadowCol = new Color(0, 0, 0, 0.06f);

            for (int i = 0; i < 3; i++)
            {
                // Draw a shadow
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (width + 1) * 5);
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, width);
        }

        void displayNodeIsNull(int nodeIndex)
        {
            if (EditorUtility.DisplayDialog("Your graph has encounterred a structure corruption!", "An item was null. Would you like to remove it?", "Yes", "No"))
            {
                currentGroup.nodes.RemoveAt(nodeIndex);
                Repaint();
            }
        }

        Input getInputAtPos(Node node, Vector2 mousePosition)
        {
            if (node && node.task && node.task.inputs.Count > 0)
            {
                Vector2 mousePosInNodeSpace = mousePosition - node.rect.position;
                if (mousePosInNodeSpace.y > GraphGUI.anchorYStartFromTop)
                {
                    if (mousePosInNodeSpace.x < (GraphGUI.anchorDistanceFromMarginLeft + GraphGUI.anchorBoxWidth))
                    {
                        int inputIndex = (int)((mousePosInNodeSpace.y - GraphGUI.anchorYStartFromTop) / GraphGUI.anchorHeight);
                        if (inputIndex >= 0 && inputIndex < node.task.inputs.Count)
                        {
                            return node.task.inputs[inputIndex];
                        }
                    }
                }
            }
            return null;
        }

        Output getOutputAtPos(Node node, Vector2 mousePosition)
        {
            if (node && node.task && node.task.outputs.Count > 0)
            {
                Vector2 mousePosInNodeSpace = mousePosition - node.rect.position;
                if (mousePosInNodeSpace.y > GraphGUI.anchorYStartFromTop)
                {
                    if (mousePosInNodeSpace.x > (node.rect.width - (GraphGUI.anchorDistanceFromMarginRight + GraphGUI.anchorBoxWidth)))
                    {
                        int outputIndex = (int)((mousePosInNodeSpace.y - GraphGUI.anchorYStartFromTop) / GraphGUI.anchorHeight);
                        if (outputIndex >= 0 && outputIndex < node.task.outputs.Count)
                        {
                            return node.task.outputs[outputIndex];
                        }
                    }
                }
            }
            return null;
        }

        Output findExistingOutputForInput(Input input)
        {
            foreach (Node node in currentGroup.nodes)
            {
                if (node && node.task)
                {
                    foreach (Output temp in node.task.outputs)
                    {
                        if (temp.input == input)
                        {
                            return temp;
                        }
                    }
                }
            }
            return null;
        }

        Input findExistingInputForOutput(Output output)
        {
            foreach (Node node in currentGroup.nodes)
            {
                if (node && node.task)
                {
                    foreach (Input temp in node.task.inputs)
                    {
                        if (temp.output == output)
                        {
                            return temp;
                        }
                    }
                }
            }
            return null;
        }

        void updateNodeSize(Node node, GUIStyle style, GUIContent content)
        {
            float width = Mathf.Max(200, style.CalcSize(content).x + 20);
            node.rect = new Rect(node.rect.position.x, node.rect.position.y, width, 50);

            if (node.task != null)
            {
                float height = GraphGUI.anchorYStartFromTop + (GraphGUI.anchorHeight + GraphGUI.anchorInterval) * Mathf.Max(node.task.inputs.Count, node.task.outputs.Count) + GraphGUI.anchorInterval;
                height = Mathf.Max(height, 50);
                node.rect.size = new Vector2(node.rect.size.x, height);
            }
        }

        int getItemIndex(Node node)
        {
            if (currentGroup)
                return currentGroup.nodes.IndexOf(node);
            return -1;
        }

        class SetScriptInfo
        {
            public System.Type type;
            public Node node;
        }

        #endregion

        #region OVERRIDE

        public override void OnEnable()
        {
            base.OnEnable();

            windowWidth = 300;

            OnSelectionChange();
        }

        public override void Update()
        {
            if (draggingOutput)
            {
                Repaint();
            }

            base.Update();
        }

        public override void OnSelectionChange()
        {
            base.OnSelectionChange();

            if (selectedStateEditor != null)
            {
                DestroyImmediate(selectedStateEditor);
                selectedStateEditor = null;
            }

            if (Selection.activeGameObject)
            {
                Flowgraph flowgraph = Selection.activeGameObject.GetComponent<Flowgraph>();
                if (flowgraph)
                {
                    setFlowgraph(flowgraph);
                }
            }
            else if (Selection.activeObject is Graph)
            {
                setGraph(graph);
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();

            if (selectedStateEditor != null)
            {
                DestroyImmediate(selectedStateEditor);
                selectedStateEditor = null;
            }
        }

        public override int AddEmptyItem(Vector2 position)
        {
            if (currentGroup)
            {
                Node childNode = GraphUtils.createNode(currentGroup);
                {
                    childNode.rect.position = position;
                    EditorUtility.SetDirty(childNode);
                }

                int idx = currentGroup.nodes.Count - 1;

                setSelectedItem(idx);

                Repaint();

                return idx;
            }
            return -1;
        }

        public override void RemoveItemAtIndex(int index)
        {
            if (currentGroup)
            {
                Node node = currentGroup.nodes[index];
                GraphUtils.removeNode(node, currentGroup);

                EditorUtility.SetDirty(currentGroup);
            }

            base.RemoveItemAtIndex(index);
        }

        public override Node GetItem(int index)
        {
            if (currentGroup)
            {
                if (index >= 0 && index < GetItemsCount())
                    return currentGroup.nodes[index];
            }
            return null;
        }

        public override int GetItemsCount()
        {
            if (currentGroup)
            {
                return currentGroup.nodes.Count;
            }
            return 0;
        }

        public override string GetItemName(int index)
        {
            if (currentGroup)
            {
                return currentGroup.name;
            }
            return null;
        }        

        public override Rect GetItemRect(int index)
        {
            if (currentGroup)
            {
                if (currentGroup.nodes[index])
                {
                    return currentGroup.nodes[index].rect;
                }
            }
            return Rect.MinMaxRect(0, 0, 1, 1);
        }

        public override void SetItemRect(int index, Rect newRect)
        {
            if (currentGroup)
            {
                currentGroup.nodes[index].rect = newRect;
                EditorUtility.SetDirty(currentGroup.nodes[index]);
            }
        }


        public override void OnItemMouseDown(Event evt, int index)
        {
            base.OnItemMouseDown(evt, index);

            if (currentGroup)
            {
                #region DESTOY STATE EDITOR

                if (selectedStateEditor != null)
                {
                    DestroyImmediate(selectedStateEditor);
                    selectedStateEditor = null;
                }

                if (getSelectedItems().Count == 1)
                {
                    if (currentGroup.nodes[getSelectedItems()[0]].task != null)
                    {
                        selectedStateEditor = UnityEditor.Editor.CreateEditor(currentGroup.nodes[getSelectedItems()[0]].task);
                    }
                }

                #endregion

                Node node = GetItem(index);

                Input draggingInput = getInputAtPos(node, evt.mousePosition);
                if (draggingInput)
                {
                    if (draggingInput.output)
                    {
                        draggingOutput = draggingInput.output;
                        draggingInput.output.input = null;
                        draggingInput.output = null;
                        EditorUtility.SetDirty(draggingInput);
                    }
                    else
                    {
                        Output output = findExistingOutputForInput(draggingInput);

                        if (output)
                        {
                            if (EditorUtility.DisplayDialog("This input has no associated output!!!", "Remove it?", "Yes", "Cancel"))
                            {
                                output.input = null;
                                EditorUtility.SetDirty(output);
                            }
                        }
                    }
                }
                else
                {
                    draggingOutput = getOutputAtPos(node, evt.mousePosition);
                }
            }
        }

        public override void OnCanvasMouseDragged(Event evt)
        {
            if (draggingOutput)
            {
                return;
            }

            base.OnCanvasMouseDragged(evt);
        }
        
        public override void OnCanvasMouseUp(Event evt)
        {
            if (draggingOutput)
            {
                int index = FindItemIndexAtPosition(evt.mousePosition);
                Node node = GetItem(index);
                Input linkedInput = getInputAtPos(node, evt.mousePosition);

                if (linkedInput && (linkedInput.task != draggingOutput.task))
                {
                    if (linkedInput.output == null)
                    {
                        draggingOutput.input = linkedInput;
                        draggingOutput.input.output = draggingOutput;

                        EditorUtility.SetDirty(draggingOutput);

                        Debug.LogFormat("Linked output {0} with input {1}", draggingOutput.title, linkedInput.title);
                    }
                    else
                    {
                        if (EditorUtility.DisplayDialog("Multiple Inputs Not Supported!", "Do you want to replace the existing link?", "Yes", "No"))
                        {
                            Input input = findExistingInputForOutput(linkedInput.output);
                            if (input)
                            {
                                linkedInput.output.input = null;
                                EditorUtility.SetDirty(linkedInput.output);

                                input.output = null;
                                EditorUtility.SetDirty(input);
                            }

                            draggingOutput.input = linkedInput;
                            draggingOutput.input.output = draggingOutput;

                            EditorUtility.SetDirty(draggingOutput);

                            Debug.LogFormat("Linked output {0} with input {1}", draggingOutput.title, linkedInput.title);
                        }
                    }
                }
                else
                {
                    Debug.LogFormat("Input not found");
                }

                draggingOutput = null;

                Repaint();

                return;
            }

            base.OnCanvasMouseUp(evt);
        }


        public override void DrawItem(int index)
        {
            if (currentGroup)
            {
                Node node = GetItem(index);                

                if (node == null)
                {
                    displayNodeIsNull(index);
                    return;
                }
                else
                {
                    bool running = false;

                    bool selected = isSelected(index);
                    if (selected)
                    {
                        lastRunningNode = node;
                    }                    

                    GUI.BeginGroup(node.rect);
                    {
                        Color backColor = GraphGUI.getNodeBackgroundColor(node, running, selected);
                        if (node.task != null)
                        {
                            EditorBehaviourGUI.BeginTint(backColor);
                            {
                                GUIContent content = new GUIContent(node.task.Title, EditorBehaviourGUI.GetTextureByFSMStateType(node.task.GetType()));
                                updateNodeSize(node, EditorBehaviourGUI.boxState, content);
                                GUI.Box(new Rect(0, 0, node.rect.width, node.rect.height), content, EditorBehaviourGUI.boxState);
                            }
                            EditorBehaviourGUI.EndTint();

                            float posY = GraphGUI.anchorYStartFromTop;

                            int maxAnchors = Mathf.Max(node.task.inputs.Count, node.task.outputs.Count);
                            for (int i = 0; i < maxAnchors; i++)
                            {
                                if (i < node.task.inputs.Count)
                                {
                                    if (node.task.inputs[i].output != null)
                                    {
                                        GUI.DrawTexture(new Rect(GraphGUI.anchorDistanceFromMarginLeft, posY, GraphGUI.anchorBoxWidth, GraphGUI.anchorBoxHeight), EditorBehaviourGUI.getTexture("anchor2")); 
                                    }
                                    else                                    
                                    {
                                        GUI.DrawTexture(new Rect(GraphGUI.anchorDistanceFromMarginLeft, posY, GraphGUI.anchorBoxWidth, GraphGUI.anchorBoxHeight), EditorBehaviourGUI.getTexture("anchor1"));
                                    }

                                    GUI.Label
                                    (
                                        new Rect
                                        (
                                            GraphGUI.anchorDistanceFromMarginLeft + GraphGUI.anchorBoxWidth + GraphGUI.anchorDistanceToText, 
                                            posY, 
                                            node.rect.width - (GraphGUI.anchorDistanceFromMarginLeft + GraphGUI.anchorBoxWidth + GraphGUI.anchorDistanceToText) * 2,
                                            GraphGUI.anchorHeight
                                        ),
                                        node.task.inputs[i].title, 
                                        EditorBehaviourGUI.labelAttributeLeft
                                    );
                                }

                                if (i < node.task.outputs.Count)
                                {
                                    if (node.task.outputs[i].input != null)
                                    {
                                        GUI.DrawTexture(new Rect(node.rect.width - GraphGUI.anchorDistanceFromMarginRight - GraphGUI.anchorBoxWidth, posY, GraphGUI.anchorBoxWidth, GraphGUI.anchorBoxHeight), EditorBehaviourGUI.getTexture("anchor2"));
                                    }
                                    else
                                    {
                                        GUI.DrawTexture(new Rect(node.rect.width - GraphGUI.anchorDistanceFromMarginRight - GraphGUI.anchorBoxWidth, posY, GraphGUI.anchorBoxWidth, GraphGUI.anchorBoxHeight), EditorBehaviourGUI.getTexture("anchor1"));
                                    }

                                    GUI.Label
                                    (
                                        new Rect
                                        (
                                            GraphGUI.anchorDistanceFromMarginRight + GraphGUI.anchorBoxWidth + GraphGUI.anchorDistanceToText,
                                            posY,
                                            node.rect.width - (GraphGUI.anchorDistanceFromMarginRight + GraphGUI.anchorBoxWidth + GraphGUI.anchorDistanceToText) * 2,
                                            GraphGUI.anchorHeight
                                        ), 
                                        node.task.outputs[i].title, 
                                        EditorBehaviourGUI.labelAttributeRight
                                    );
                                }

                                posY += 20;
                            }
                        }
                        else
                        {
                            EditorBehaviourGUI.BeginTint(backColor);
                            {
                                GUIContent content = new GUIContent(node.title);
                                updateNodeSize(node, EditorBehaviourGUI.boxEmptyState, content);
                                GUI.Box(new Rect(0, 0, node.rect.width, node.rect.height), content, EditorBehaviourGUI.boxEmptyState);
                            }
                            EditorBehaviourGUI.EndTint();
                        }
                    }
                    GUI.EndGroup();
                }                
            }
        }

        public override void DrawBeforeItems()
        {
            base.DrawBeforeItems();

            if (currentGroup)
            {
                int objectsCount = GetItemsCount();
                for (int i = objectsCount - 1; i >= 0; --i)
                {
                    Node node0 = GetItem(i);

                    if (node0.task)
                    {
                        for (int j = node0.task.outputs.Count - 1; j >= 0; --j)
                        {
                            if (node0.task.outputs[j].input && node0.task.outputs[j].input.task)
                            {
                                Node node1 = node0.task.outputs[j].input.task.node;

                                Vector2 p0 = new Vector2(node0.rect.xMax - GraphGUI.anchorBoxWidth / 2, node0.rect.yMin + GraphGUI.anchorYStartFromTop + GraphGUI.anchorHeight * j + GraphGUI.anchorBoxHeight / 2);

                                int k = node0.task.outputs[j].input.task.inputs.IndexOf(node0.task.outputs[j].input);

                                Vector2 p1 = new Vector2(node1.rect.xMin + GraphGUI.anchorBoxWidth / 2, node1.rect.yMin + GraphGUI.anchorYStartFromTop + GraphGUI.anchorHeight * k + GraphGUI.anchorBoxHeight / 2);

                                DrawNodeCurve(p0, p1, Color.white, node0.task.outputs[j].type == ValueType.None ? 4 : 2);
                            }
                        }
                    }
                }

                if (draggingOutput)
                {
                    Node node0 = draggingOutput.task.node;

                    int j = node0.task.outputs.IndexOf(draggingOutput);

                    Vector2 p0 = new Vector2(node0.rect.xMax - GraphGUI.anchorBoxWidth / 2, node0.rect.yMin + GraphGUI.anchorYStartFromTop + GraphGUI.anchorHeight * j + GraphGUI.anchorBoxHeight / 2);

                    Vector2 p1 = Event.current.mousePosition;

                    DrawNodeCurve(p0, p1, Color.green, 2);
                }
            }
        }

        public override string GetWindowTitle()
        {
            return "Graph";
        }

        public override string GetViewTitle()
        {
            if (currentGroup)
            {
                if (currentGroup.node == graph.root)
                {
                    return graph.name;
                }
                else
                {
                    return currentGroup.name;
                }
            }
            return "";
        }        

        public override void DoPropertiesWindowGUI(int wndId)
        {
            if (graph == null)
            {
                EditorBehaviourGUI.BeginEnable(!Application.isPlaying);
                {
                    EditorGUILayout.HelpBox("Please select an existing layer!", MessageType.Info);

                    Graph newGraph = EditorGUILayout.ObjectField("Graph", graph, typeof(Graph), false) as Graph;
                    setGraph(newGraph);

                    EditorGUILayout.HelpBox("You can also create a new layer!", MessageType.Info);

                    if (GUILayout.Button("Create New Layer"))
                    {
                        string assetPath = EditorUtility.SaveFilePanelInProject("Create new Graph Layer", "New Graph Layer", "asset", "");
                        if (string.IsNullOrEmpty(assetPath) == false)
                        {
                            newGraph = GraphUtils.createGraph(assetPath);
                            setGraph(newGraph);
                        }
                    }
                }
                EditorBehaviourGUI.EndEnable();
            }
            else
            {
                EditorBehaviourGUI.BeginEnable(!Application.isPlaying);
                {
                    GraphGUI.DrawGraph(graph);                

                    _DoMyWindowTab = GUILayout.Toolbar(_DoMyWindowTab, new string[] { "Graph", "Transitions", "State" });
                    if (_DoMyWindowTab == 0)
                    {
                        ValueGUI.DrawLayerValues("Vars", graph);
                    }
                    else if (_DoMyWindowTab == 1)
                    {
                    }
                    else if (_DoMyWindowTab == 2)
                    {
                        if (getSelectedItems().Count == 1)
                        {
                            // draw editor
                            if (selectedStateEditor != null)
                            {
                                selectedStateEditor.OnInspectorGUI();
                            }
                        }                    
                    }
                }
                EditorBehaviourGUI.EndEnable();
            }

            base.DoPropertiesWindowGUI(wndId);            
        }

        public override GenericMenu CreateMenu(int mouseDownIndex, Vector2 mousePosition)
        {
            GenericMenu menu = base.CreateMenu(mouseDownIndex, mousePosition);
            {
                if (mouseDownIndex != -1)
                {
                    Node node = GetItem(mouseDownIndex);

                    menu.AddSeparator("");
                    if (nodeBuffer)
                    {
                    }

                    menu.AddItem(new GUIContent("Select asset"), false, OnContextMenuItemClick_SelectAsset, node);
                    if (node.task is Composite)
                    {
                        menu.AddItem(new GUIContent("Enter group"), false, OnContextMenuItemClick_EnterGroup, node);
                    }
                    else
                    {
                        System.Type baseType = typeof(Task);
                        System.Type[] types = System.Reflection.Assembly.GetAssembly(baseType).GetTypes();
                        for (int i = 0; i < types.Length; ++i)
                        {
                            if (types[i].IsSubclassOf(baseType))
                            {
                                if (types[i] != typeof(Task))
                                {
                                    SetScriptInfo ssi = new SetScriptInfo();
                                    ssi.node = node;
                                    ssi.type = types[i];
                                    menu.AddItem(new GUIContent("Set script.../" + types[i].FullName.Replace("Behaviour.Graph.", "").Replace(".", "/")), false, OnContextMenuItemClick_GraphSetState, ssi);
                                }
                            }
                        }

                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Remove Script"), false, OnContextMenuItemClick_RemoveState, node);
                    }
                }
                else
                {
                    if (currentGroup && currentGroup.node.parent)
                    {
                        menu.AddItem(new GUIContent("Exit group"), false, OnContextMenuItemClick_ExitGroup, currentGroup);
                    }
                }
            }
            return menu;
        }                

        public override void OnPlayModeChanged(bool isPlaying)
        {
            base.OnPlayModeChanged(isPlaying);

            OnSelectionChange();
        }

        #endregion
    }
}