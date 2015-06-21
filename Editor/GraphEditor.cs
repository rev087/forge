using UnityEngine;
using UnityEditor;
using Forge.Editor.Renderers;
using System.Collections.Generic;

namespace Forge.Editor {

	public struct MenuActionPayload {
		public System.Type OperatorType;
		public Vector2 Position;
	}

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
			GraphEditor editor = (GraphEditor) EditorWindow.GetWindow(typeof(GraphEditor));
			editor.title = "Forge Graph";
			editor.Show();
		}

		public void OnSelectionChange() {
			// Selected a Template asset
			var selected = UnityEditor.Selection.activeObject as Template;
			if (Template != selected) {
				Template = selected;
				_nodes.Clear();
				Selection.Clear();
				Repaint();
				return;
			}

			// Selected a ProceduralAsset in hierarchy or prefab asset
			var go = UnityEditor.Selection.activeObject as GameObject;
			if (go != null) {
				var asset = go.GetComponent<ProceduralAsset>();
				if (asset != null && Template != asset.Template) {
					Template = asset.Template;
					_nodes.Clear();
					Selection.Clear();
					Repaint();
				}
			}
		}

		public static Node GetNode(string GUID) {
			if (!_nodes.ContainsKey(GUID)) {
				_nodes[GUID] = new Node(Template.Operators[GUID]);
			}
			return _nodes[GUID];
		}

		public void OnEnable() {
			OnSelectionChange();
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

			if (Template != null) {

				// Draw the connections
				Template.DrawConnections();

				// Draw the nodes
				foreach (var kvp in Template.Operators) {
					Node node = GetNode(kvp.Key); // kvp.Key = GUID
					needsRepaint = needsRepaint || node.EventsNeedRepaint(Zoom, this);
					node.Draw(Zoom);
				}

				// Handle left mouse button events
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

				if (currentEvent.button == 1 && currentEvent.type == EventType.MouseUp) {
					var menu = new GenericMenu();

					var opTypes = Operator.GetAvailableOperators();
					foreach (var opType in opTypes) {
						var payload = new MenuActionPayload() {OperatorType=opType, Position=currentEvent.mousePosition};
						menu.AddItem(new GUIContent(opType.Name), false, MenuAction, payload);
					}

					menu.ShowAsContext();
				}

			}

			if (needsRepaint) {
				Repaint();
			}

			GUI.EndScrollView();

		} // OnGUI

		public void MenuAction(object payloadObj) {
			var payload = (MenuActionPayload)payloadObj;
			var opType = (System.Type)payload.OperatorType;
			var op = (Operator)System.Activator.CreateInstance(opType);
			op.EditorPosition = payload.Position;
			Template.AddOperator(op);
			Repaint();
		}

	}

} //namespace