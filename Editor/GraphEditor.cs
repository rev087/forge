using UnityEngine;
using UnityEditor;
using Forge.Editor.Renderers;
using System.Collections.Generic;

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

	public class GraphEditor : EditorWindow {

		private GridRenderer _gridRenderer;
		public Vector2 ScrollPoint = Vector2.zero;
		public float Zoom = 1f;
		public Rect Canvas;
		private Dictionary<string, Node> _nodes = new Dictionary<string, Node>();
		public Template Template = new Template();

		public static List<Node> Selection = new List<Node>();
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

			if (Event.current.button == 0) {

				// MouseDown
				if (Event.current.type == EventType.MouseDown && CurrentEvent.Type == GEType.None) {
					CurrentEvent = new GraphEvent(GEType.Unresolved, GEContext.Grid, null, IOOutlet.None);
				}

				// MouseDrag
				if (Event.current.type == EventType.MouseDrag && CurrentEvent.IsType(GEType.Unresolved, GEType.Drag) && CurrentEvent.Context == GEContext.Grid) {
					if (Event.current.delta.magnitude > 0) {
						CurrentEvent = new GraphEvent(GEType.Drag, GEContext.Grid, null, IOOutlet.None);
						ScrollPoint.x += - Event.current.delta.x;
						ScrollPoint.y += - Event.current.delta.y;
						needsRepaint = true;
					}
				}

				// MouseUp
				if (Event.current.type == EventType.MouseUp) {
					if (CurrentEvent.Type == GEType.Unresolved && CurrentEvent.Context == GEContext.Grid) {
						Selection.Clear();
						needsRepaint = true;
					}
					needsRepaint = needsRepaint || CurrentEvent.IsConnecting();
					CurrentEvent.Empty();
				}

			} // Left mouse down/drag/up

			if (needsRepaint) {
				Repaint();
			}

			GUI.EndScrollView();
		} // OnGUI

	}

} //namespace