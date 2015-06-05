using UnityEngine;
using UnityEditor;

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
				ScrollPoint.x += - Event.current.delta.x;
				ScrollPoint.y += - Event.current.delta.y;
				needsRepaint = true;
			}

			if (Event.current.type == EventType.ScrollWheel) {
				Event.current.Use();
				Zoom += -Event.current.delta.y / 50;
				if (Zoom < 0.2f) Zoom = 0.2f;
				if (Zoom > 1f) Zoom = 1f;
				needsRepaint = true;
			}

			_gridRenderer.Draw(ScrollPoint, Zoom, Canvas);
			
			_nodeRenderer.Title = "Circle";
			_nodeRenderer.Draw(ScrollPoint, Zoom);

			if (needsRepaint) Repaint();

			GUI.EndScrollView();
		}

	}

} //namespace