using UnityEngine;
using UnityEditor;
using Forge.Editor.Renderers;
using System.Collections.Generic;

namespace Forge.Editor {

	public enum GEType { None, Unresolved, Click, Drag }
	public enum GEContext { None, Node, Output, Input }
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

		public bool IsConnecting() {
			return Type == GEType.Drag &&
				(Context == GEContext.Input || Context == GEContext.Output);
		}

		public bool IsNodeDrag(Node node) {
			return Node == node && Context == GEContext.Node &&
				(Type == GEType.Unresolved || Type == GEType.Drag);
		}

		public bool CanDragOutlet(Node node, GEContext context) {
			return Node == node && Context == context &&
				(Type == GEType.Unresolved || Type == GEType.Drag);
		}
	}

	public class GraphEditor : EditorWindow {

		private GridRenderer _gridRenderer;
		public Vector2 ScrollPoint = Vector2.zero;
		public float Zoom = 1f;
		public Rect Canvas;
		private Dictionary<string, Node> _nodes = new Dictionary<string, Node>();
		public Template Template = new Template();

		public static GraphEvent CurrentEvent;

		[MenuItem ("Window/Forge/Graph Editor")]
		public static void ShowEditor() {
			var editor = (GraphEditor) EditorWindow.GetWindow(typeof(GraphEditor));
			editor.title = "Forge Graph";
			editor.Show();
		}

		public void OnEnable() {
			Canvas = new Rect(0, 0, position.width*2, position.height*2);

			_nodes = new Dictionary<string, Node>();
			foreach (KeyValuePair<string, Operator> op in Template.Operators) {
				var node = new Node(op.Value);
				_nodes.Add(op.Key, node);
			}

			wantsMouseMove = true;
		}

		void OnGUI () {
			if (_gridRenderer == null) _gridRenderer = new GridRenderer();

			ScrollPoint = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), ScrollPoint, Canvas);

			bool needsRepaint = false;

			if (Event.current.type == EventType.ScrollWheel) {
				Zoom += -Event.current.delta.y / 50;
				if (Zoom < 0.25f) Zoom = 0.25f;
				if (Zoom > 1f) Zoom = 1f;
				needsRepaint = true;
				Event.current.Use();
			}

			Canvas = new Rect(0f, 0f, position.width*4*Zoom, position.height*4*Zoom);

			_gridRenderer.Draw(ScrollPoint, Zoom, Canvas);

			Template.DrawConnections(_nodes);

			foreach (KeyValuePair<string, Node> node in _nodes) {
				needsRepaint = needsRepaint || node.Value.EventsNeedRepaint(Zoom, this);
				node.Value.Draw(Zoom);
			}

			// Drag the ScrollView
			if (CurrentEvent.Type == GEType.None && Event.current.type == EventType.MouseDrag && Event.current.button == 0) {
				if (Event.current.delta.magnitude > 0) {
					ScrollPoint.x += - Event.current.delta.x;
					ScrollPoint.y += - Event.current.delta.y;
					needsRepaint = true;
					Event.current.Use();
				}
			}

			// Releasing mouse button
			if (Event.current.type == EventType.MouseUp) {
				needsRepaint = needsRepaint || CurrentEvent.IsConnecting();
				CurrentEvent.Empty();
			}

			if (needsRepaint) {
				Repaint();
			}

			GUI.EndScrollView();
		} // OnGUI

	}

} //namespace