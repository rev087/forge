using UnityEngine;
using UnityEditor;
using Forge.Editor.Renderers;
using System.Collections.Generic;
using Forge.Extensions;

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
		private static GUIStyle LabelStyle = null;
		public const float SidebarWidth = 250f;
		private const float MaxZoom = 1f;
		private const float MinZoom = 0.2f;
		private const float CanvasWidth = 6000f;
		private const float CanvasHeight = 6000f;

		private static Template _template = null;
		public static Template Template {
			get { return _template; }
			set {
				_template = value;
				if (_template != null) {
					_template.Changed += OnTemplateChange;
				}
			}
		}

		public static GraphSelection Selection = new GraphSelection();
		public static GraphEvent CurrentEvent;

		[MenuItem("Window/Forge/Graph Editor")]
		public static void ShowEditor() {
			GraphEditor editor = (GraphEditor) EditorWindow.GetWindow(typeof(GraphEditor));
			editor.titleContent = new GUIContent("Forge Graph");
			editor.Show();
		}

		public static void OnTemplateChange(Template template) {
			var go = UnityEditor.Selection.activeObject as GameObject;
			if (go != null) {
				var asset = go.GetComponent<ProceduralAsset>();
				if (asset != null) {
					asset.Generate();
				}
			}
		}

		public void OnSelectionChange() {

			// Selected a Template asset
			var templateAsset = UnityEditor.Selection.activeObject as Template;
			if (templateAsset != null) {
				Template = templateAsset;
				_nodes.Clear();
				Selection.Clear();
				Repaint();
				return;
			}

			// Selected a ProceduralAsset in hierarchy or prefab asset
			var go = UnityEditor.Selection.activeObject as GameObject;
			if (go != null) {
				var asset = go.GetComponent<ProceduralAsset>();
				if (asset != null) {
					Template = asset.Template;
					_nodes.Clear();
					Selection.Clear();
					Repaint();
				}
				return;
			}

			// Non-template selection
			Template = null;
			Repaint();
		}

		public static Node GetNode(string GUID) {
			if (!_nodes.ContainsKey(GUID)) {
				if (Template.Operators.ContainsKey(GUID)) {
					_nodes[GUID] = new Node(Template.Operators[GUID]);
				} else {
					Debug.LogWarningFormat("GraphEditor.GetNode error: Template does not contain an Operator with GUID {0}", GUID);
				}
			}
			return _nodes[GUID];
		}

		public void OnEnable() {
			OnSelectionChange();
			wantsMouseMove = true;
		}

		void OnGUI () {
			if (_gridRenderer == null) _gridRenderer = new GridRenderer();
			Event currentEvent = Event.current;

			Rect scrollViewRect = new Rect(0, 0, position.width - SidebarWidth, position.height);
			Rect sidebarRect = new Rect(position.width - SidebarWidth, 0, SidebarWidth, position.height);

			OperatorInspector.DrawInspector(sidebarRect);

			ScrollPoint = GUI.BeginScrollView(scrollViewRect, ScrollPoint, Canvas);

			bool needsRepaint = false;

			// Mouse wheel event to control zoom
			if (currentEvent.type == EventType.ScrollWheel) {

				float previousZoom = Zoom;
				Vector2 pivotPoint = currentEvent.mousePosition / Zoom;
				float wheelDelta = Event.current.delta.y > 0 ? -0.1f : 0.1f;

				// Apply the delta to the zoom level and clamp to min and max
				Zoom = (Zoom + wheelDelta).Clamp(MinZoom, MaxZoom);
				Zoom = (float) System.Math.Round(Zoom, 2);
				
				if (previousZoom != Zoom) {
					// Keep the viewport anchored to the position of the mouse cursor
					float zoomDelta = Zoom - previousZoom;
					ScrollPoint += pivotPoint * zoomDelta;

					// Repaint if there was a zoom change (after clamping)
					needsRepaint = true;
				}

				currentEvent.Use();
			}

			Canvas = new Rect(0f, 0f, CanvasWidth * Zoom, CanvasHeight * Zoom);

			_gridRenderer.Draw(ScrollPoint, Zoom, Canvas);

			if (Template != null) {

				// Draw the connections
				Template.DrawConnections();

				// Draw the nodes
				string[] guids = new string[Template.Operators.Count];
				Template.Operators.Keys.CopyTo(guids, 0);
				for (int i = 0; i < guids.Length; i++) {
					Node node = GetNode(guids[i]); // kvp.Key = GUID
					needsRepaint = needsRepaint || node.EventsNeedRepaint(Zoom, this);
					node.Draw(Zoom);
				}

				// Handle left mouse button events
				if (currentEvent.button == 0) {

					// MouseDown
					if (currentEvent.type == EventType.MouseDown && CurrentEvent.Type == GEType.None) {
						CurrentEvent = new GraphEvent(GEType.Unresolved, GEContext.Grid, null, IOOutlet.None, currentEvent.mousePosition);
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

				// Right mouse button
				if (currentEvent.type == EventType.ContextClick) {
					var menu = new GenericMenu();

					var opTypes = Operator.GetAvailableOperators();
					foreach (var opType in opTypes) {
						string menuLabel = opType.Name;
						var meta = System.Attribute.GetCustomAttribute(opType, typeof(OperatorMetadataAttribute)) as OperatorMetadataAttribute;
						if (meta != null) {
							menuLabel = meta.Category + "/" + (meta.Title != null ? meta.Title : opType.Name);
						}
						var payload = new MenuActionPayload() {OperatorType=opType, Position=currentEvent.mousePosition / Zoom};
						menu.AddItem(new GUIContent(menuLabel), false, MenuAction, payload);
					}

					menu.ShowAsContext();
				}

				if (currentEvent.isKey && currentEvent.keyCode == KeyCode.Delete) {
					foreach (var node in Selection.Nodes) {
						Template.RemoveOperator(node.Operator);
					}
					Selection.Clear();
					needsRepaint = true;
				}

			}

			if (needsRepaint) {
				Repaint();
			}

			GUI.EndScrollView();

			// Current Zoom level
			float zoomLabelWidth = 150f;
			float zoomLabelHeight = 20f;
			Rect zoomLabelRect = new Rect(position.width - SidebarWidth - zoomLabelWidth - 20f, position.height - zoomLabelHeight - 20f, zoomLabelWidth, zoomLabelHeight);
			if (LabelStyle == null) {
				LabelStyle = new GUIStyle();
				LabelStyle.normal.textColor = Color.white;
				LabelStyle.alignment = TextAnchor.LowerRight;
			}
			GUI.Label(zoomLabelRect, string.Format("Zoom: {0}%", Zoom * 100f), LabelStyle);

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