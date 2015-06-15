using UnityEngine;
using UnityEditor;
using Forge.Editor.Renderers;
using System.Collections.Generic;

namespace Forge.Editor {

	public class GraphEditor : EditorWindow {

		private GridRenderer _gridRenderer;
		public Vector2 ScrollPoint = Vector2.zero;
		public float Zoom = 1f;
		public Rect Canvas;
		private static Dictionary<string, Node> _nodes = new Dictionary<string, Node>();
		public static Template Template = null;

		public static GraphSelection Selection = new GraphSelection();
		public static GraphEvent CurrentEvent;

		[MenuItem ("Window/Forge/Graph Editor")]
		public static void ShowEditor() {
			var editor = (GraphEditor) EditorWindow.GetWindow(typeof(GraphEditor));
			editor.title = "Forge Graph";
			editor.Show();
		}

		public static void LoadTemplate(Template template) {
			// Debug.Log("Loading Template");
			Template = template;

			_nodes = new Dictionary<string, Node>();
			foreach (KeyValuePair<string, Operator> op in Template.Operators) {
				var node = new Node(op.Value);
				_nodes.Add(op.Key, node);
			}
		}

		public void OnEnable() {
			Template template = (Template) ScriptableObject.CreateInstance(typeof(Template));
			template.LoadDemo();
			LoadTemplate(template);

			Canvas = new Rect(0, 0, position.width*2, position.height*2);

			wantsMouseMove = true;
		}

		void OnGUI () {
			if (_gridRenderer == null) _gridRenderer = new GridRenderer();
			Event currentEvent = Event.current;

			Rect scrollViewRect = new Rect(0, 0, position.width, position.height);
			ScrollPoint = GUI.BeginScrollView(scrollViewRect, ScrollPoint, Canvas);

			bool needsRepaint = false;

			if (currentEvent.type == EventType.ScrollWheel) {
				Zoom += -currentEvent.delta.y / 50;
				if (Zoom < 0.25f) Zoom = 0.25f;
				if (Zoom > 1f) Zoom = 1f;
				needsRepaint = true;
				currentEvent.Use();
			}

			Canvas = new Rect(0f, 0f, position.width*4*Zoom, position.height*4*Zoom);

			_gridRenderer.Draw(ScrollPoint, Zoom, Canvas);

			Template.DrawConnections(_nodes);

			foreach (KeyValuePair<string, Node> node in _nodes) {
				needsRepaint = needsRepaint || node.Value.EventsNeedRepaint(Zoom, this);
				node.Value.Draw(Zoom);
			}

			if (currentEvent.button == 0) {

				// MouseDown
				if (currentEvent.type == EventType.MouseDown && CurrentEvent.Type == GEType.None) {
					CurrentEvent = new GraphEvent(GEType.Unresolved, GEContext.Grid, null, IOOutlet.None);
				}

				// MouseDrag
				if (currentEvent.type == EventType.MouseDrag && CurrentEvent.IsType(GEType.Unresolved, GEType.Drag) && CurrentEvent.Context == GEContext.Grid) {
					if (currentEvent.delta.magnitude > 0) {
						CurrentEvent = new GraphEvent(GEType.Drag, GEContext.Grid, null, IOOutlet.None);
						ScrollPoint.x += - currentEvent.delta.x;
						ScrollPoint.y += - currentEvent.delta.y;
						needsRepaint = true;
					}
				}

				// MouseUp
				if (currentEvent.type == EventType.MouseUp) {
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