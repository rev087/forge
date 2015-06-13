using UnityEngine;

namespace Forge.Editor {

	public enum GEType { None, Unresolved, Click, Drag }
	public enum GEContext { None, Grid, Node, Output, Input }
	public struct GraphEvent {
		public GEType Type;
		public GEContext Context;
		public Node Node;
		public IOOutlet Outlet;

		public GraphEvent(GEType type, GEContext context, Node node, IOOutlet outlet) {
			Type = type;
			Context = context;
			Node = node;
			Outlet = outlet;
		}

		public void Empty() {
			Type = GEType.None;
			Context = GEContext.None;
			Node = null;
		}

		public bool IsType(params GEType[] types) {
			foreach (GEType type in types) {
				if (Type == type) return true;
			}
			return false;;
		}

		public bool IsConnecting() {
			return Type == GEType.Drag &&
				(Context == GEContext.Input || Context == GEContext.Output);
		}

		public bool IsNodeDrag(Node node) {
			return Node == node && Context == GEContext.Node &&
				IsType(GEType.Unresolved, GEType.Drag);
		}

		public bool CanDragOutlet(Node node, GEContext context) {
			return Node == node && Context == context &&
				IsType(GEType.Unresolved, GEType.Drag);
		}
	}

}