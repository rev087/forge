using UnityEngine;
using UnityEditor;
using Forge.Editor.Renderers;

namespace Forge.Editor {

	public class GraphEditor : EditorWindow {

		private GridRenderer _gridRenderer;
		private NodeRenderer _nodeRenderer;
		public Vector2 ScrollPoint = Vector2.zero;
		public float Zoom = 1f;
		public Rect Canvas;

		[MenuItem ("Window/Forge/Graph Editor")]
		public static void ShowEditor() {
			var editor = (GraphEditor) EditorWindow.GetWindow(typeof(GraphEditor));
			editor.title = "Forge Graph";
			editor.Show();
		}

		public void OnEnable() {
			Canvas = new Rect(0, 0, position.width*2, position.height*2);
		}

		void OnGUI () {
			if (_gridRenderer == null) _gridRenderer = new GridRenderer();
			if (_nodeRenderer == null) _nodeRenderer = new NodeRenderer();

			ScrollPoint = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), ScrollPoint, Canvas);

			bool needsRepaint = false;

			if (Event.current.type == EventType.MouseDrag && Event.current.button == 0) {
				if (Event.current.delta.magnitude > 0) {
					ScrollPoint.x += - Event.current.delta.x;
					ScrollPoint.y += - Event.current.delta.y;
					needsRepaint = true;
					Event.current.Use();
				}
			}

			if (Event.current.type == EventType.ScrollWheel) {
				Zoom += -Event.current.delta.y / 50;
				if (Zoom < 0.25f) Zoom = 0.25f;
				if (Zoom > 1f) Zoom = 1f;
				needsRepaint = true;
				Event.current.Use();
			}

			Canvas = new Rect(0f, 0f, position.width*4*Zoom, position.height*4*Zoom);

			_gridRenderer.Draw(ScrollPoint, Zoom, Canvas);
			_nodeRenderer.Draw(ScrollPoint, Zoom);

			if (needsRepaint) {
				Repaint();
			}

			GUI.EndScrollView();
		}

	}

} //namespace