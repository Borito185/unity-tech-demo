using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Generation.Noise.Nodes;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace Generation.Noise.Editor
{
    [CustomNodeGraphEditor(typeof(NoiseGraph))]
    public class NoiseGraphEditor : NodeGraphEditor
    {
        public override void OnOpen()
        {
            window.titleContent.text  = target.name + " (Noise Graph)";
            window.titleContent.image = EditorGUIUtility.IconContent("UnityEditor.Graphs.AnimatorControllerTool").image;
            base.OnOpen();
        }
        
        public override string GetNodeMenuName(Type type)
        {
            return typeof(NoiseNodeBase).IsAssignableFrom(type) ? Name(type) : null;
        }

        private string Name(Type type)
        {
            string s = type.Name.Split('.')[^1];
            s = s.Replace("Node", "");
            s = Regex.Replace(s, "(\\B[A-Z])", " $1");
            return s;
        }

        public override bool CanRemove(Node node)
        {
            return (node.GetType() != typeof(OutputNode) || target.nodes.Count(n => n.GetType() == typeof(OutputNode)) > 1) && base.CanRemove(node);
        }

        public override void OnCreate()
        {
            var  outputs      = target.nodes.Where(n => n.GetType() == typeof(OutputNode)).ToList();
            bool firstSkipped = false;
            foreach (Node output in outputs)
            {
                if (firstSkipped) 
                    RemoveNode(output);

                firstSkipped = true;
            }
            if (!firstSkipped)
                CreateNode(typeof(OutputNode), Vector2.zero);
            
            base.OnCreate();
        }
    }
}