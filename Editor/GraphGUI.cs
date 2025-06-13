using UnityEngine;
using UnityEditor;

namespace Behaviours.Graph.Editor
{
    public class GraphGUI
    {
        public static void DrawGraph(Graph graph)
        {
            if (graph)
            {
                GUILayout.BeginVertical(GUI.skin.box);

                // DRAW LAYER NAME
                EditorGUI.BeginChangeCheck();
                graph.name = EditorGUILayout.TextField("Graph name", graph.name);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(graph);
                }

                // DRAW LAYER ENABLED
                EditorGUI.BeginChangeCheck();
                graph.enabled = EditorGUILayout.Toggle("Graph enabled", graph.enabled);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(graph);
                }

                GUILayout.EndVertical();
            }
        }

        public static void DrawState(Graph graph, Composite group, Node node)
        {
            // DRAW STATE NAME
            EditorGUI.BeginChangeCheck();
            node.title = EditorGUILayout.TextField("Node name", node.title);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(node);
            }            
            EditorGUILayout.EndVertical();
        }

        public static Color colorStateDefault = new Color(1.0f, 0.5f, 0.25f, 1);
        public static Color colorStateAny = new Color(0.5f, 0.25f, 0.5f, 1);
        public static Color colorStateExit = Color.cyan;
        public static Color colorGroupNormal = Color.green;
        public static Color colorStateNormal = new Color(0.8f, 0.8f, 0.8f, 1);

        public static Color getNodeBackgroundColor(Node node, bool running, bool selected)
        {
            Color color = colorStateNormal;

            if (node)
            {
                if (node.task is Composite)
                {
                    color = colorGroupNormal;
                }

                if (node.parent)
                {
                    Composite parent = node.parent.task as Composite;
                    if (parent)
                    {
                        //    int idx = parent.findIdxByNode(node);

                        //    if (idx == parent.anyNodeIndex)
                        //    {
                        //        color = colorStateAny;
                        //    }

                        //    if (idx == parent.defaultNodeIndex)
                        //    {
                        //        color = colorStateDefault;
                        //    }

                        //    if (idx == parent.exitNodeIndex)
                        //    {
                        //        color = colorStateExit;
                        //    }

                        if (running)
                            color = new Color(1, 0, 0, 1);
                    }
                }
            }

            if (selected)
            {
                color += new Color(0.5f, 0.5f, 0.5f, 0);
            }

            return color;
        }

        public const float anchorDistanceFromMarginLeft = 5;
        public const float anchorDistanceFromMarginRight = 8;
        public const float anchorInterval = 2;
        public const float anchorBoxWidth = 15;
        public const float anchorBoxHeight = 15;
        public const float anchorHeight = 20;        
        public const float anchorYStartFromTop = 30;
        public const float anchorDistanceToText = 2;
    }
}