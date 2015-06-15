using UnityEngine;
using System.Collections.Generic;

namespace Forge.Editor {

	public class GraphSelection {

		public event SelectionChangedHandler SelectionChanged;
		public delegate void SelectionChangedHandler(GraphSelection graphSelection);

		public List<Node> Nodes = new List<Node>();
		public Node ActiveNode = null;

		public void Add(Node node) {
			Nodes.Add(node);
			ActiveNode = node;
			if (SelectionChanged != null) SelectionChanged(this);
		}

		public void Clear() {
			Nodes.Clear();
			ActiveNode = null;
			if (SelectionChanged != null) SelectionChanged(this);
		}

		public bool Contains(Node node) {
			return Nodes.Contains(node);
		}

	}

}