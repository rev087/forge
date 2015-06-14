using UnityEngine;
using System.Collections.Generic;

namespace Forge.Editor {

	public class Selection {

		public List<Node> Nodes = new List<Node>();
		public Node ActiveNode = null;

		public void Add(Node node) {
			Nodes.Add(node);
			ActiveNode = node;
		}

		public void Clear() {
			Nodes.Clear();
			ActiveNode = null;
		}

		public bool Contains(Node node) {
			return Nodes.Contains(node);
		}

	}

}