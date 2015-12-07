using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace Forge.Editor {

	public class GraphSelection {

		public List<Node> Nodes = new List<Node>();
		public Node ActiveNode = null;

		public void Add(Node node) {
			Nodes.Add(node);
			Node previous = ActiveNode;
			ActiveNode = node;
			OnSelectionChange(previous, node);
		}

		public void Clear() {
			Nodes.Clear();
			Node previous = ActiveNode;
			ActiveNode = null;
			OnSelectionChange(previous, null);
		}

		public bool Contains(Node node) {
			return Nodes.Contains(node);
		}

		public void OnSelectionChange(Node previous, Node current) {
			ProceduralAsset pa = (ProceduralAsset) Selection.activeGameObject.GetComponent(typeof(ProceduralAsset));

			if (pa == null) return;

			if (previous != null) {
				Operator previousOp = previous.Operator;
				MethodInfo onDrawGizmos = previousOp.GetType().GetMethod("OnDrawGizmos");
				if (onDrawGizmos != null) {
					pa.OnDrawGizmos -= previousOp.OnDrawGizmos;
				}
			}

			if (current != null) {
				Operator currentOp = current.Operator;
				MethodInfo onDrawGizmos = currentOp.GetType().GetMethod("OnDrawGizmos");
				if (onDrawGizmos != null) {
					pa.OnDrawGizmos += currentOp.OnDrawGizmos;
				}
			}

			SceneView.RepaintAll();
		}

	}

}