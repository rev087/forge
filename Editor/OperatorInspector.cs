using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace Forge.Editor {

	public static class OperatorInspector {

		private static GUIStyle _TitleStyle = null;
		private const float Margin = 5f;
		private static Vector2 ScrollPosition = Vector2.zero;

		public static void DrawInspector(Rect rect) {
			GUILayout.BeginArea(rect);
			ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.Width(rect.width), GUILayout.Height(rect.height));
			for (int i = 0; i < GraphEditor.Selection.Nodes.Count; i++) {
				Node node = GraphEditor.Selection.Nodes[i];
				EditorGUILayout.LabelField(node.Operator.GUID);
				DrawNodeInspector(node.Operator);
				EditorGUILayout.Space();
			}
			if (GraphEditor.Selection.Nodes.Count > 0 && GUILayout.Button("Delete Selection")) {
				foreach (var node in GraphEditor.Selection.Nodes) {
					GraphEditor.Template.RemoveOperator(node.Operator);
				}
				GraphEditor.Selection.Clear();
			}
			GUILayout.EndScrollView();
			GUILayout.EndArea();

			if (GUI.changed) {
				GraphEditor.Template.Serialize();
				
				var go = UnityEditor.Selection.activeObject as GameObject;
				if (go != null) {
					var asset = go.GetComponent<ProceduralAsset>();
					if (asset != null) {
						asset.Generate();
					}
				}
			}
		}

		private static Dictionary<string, ReorderableList> _geoReorderableLists = new Dictionary<string, ReorderableList>();

		public static void DrawNodeInspector(Operator op) {

			if (GraphEditor.Template == null) return;

			if (_TitleStyle == null) {
				_TitleStyle = new GUIStyle();
				_TitleStyle.fontSize = 16;
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(op.Metadata.Title, _TitleStyle, GUILayout.Width(GraphEditor.SidebarWidth/2-20f));
			if (op.IsGeometryOutput) {
				EditorGUILayout.LabelField("(output)", GUILayout.Width(GraphEditor.SidebarWidth/2-20f));
			} else {
				if (GUILayout.Button("Output")) {
					foreach (var kvp in GraphEditor.Template.Operators) {
						kvp.Value.IsGeometryOutput = kvp.Value.GUID == op.GUID;
					}
					GraphEditor.Template.Serialize();
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

			if (op.IsGeometryOutput) {
				EditorGUILayout.LabelField("[Geometry Output]");
			}

			foreach (IOOutlet input in op.Inputs) {

				var isConnectedInput = false;

				// Collections
				if (input.DataType.GetInterface("ICollection") != null) {

					// http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
					ReorderableList geoList;
					string listKey = op.GUID + "." + input.Name;

					IOConnection[] inputConnections = GraphEditor.Template.ConnectionsTo(op, input);

					if (!_geoReorderableLists.TryGetValue(listKey, out geoList)) {
						geoList = new ReorderableList(inputConnections, typeof(string), true, true, false, true);
						_geoReorderableLists.Add(listKey, geoList);
					}

					// Update list
					geoList.list = inputConnections;

					geoList.drawHeaderCallback = (Rect rect) => {
						EditorGUI.LabelField(rect, input.Name);
					};
					geoList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
						string title = index + ": " + inputConnections[index].From.Metadata.Title;
						title += " (" + inputConnections[index].From.GUID.Substring(0, 5) + ")";
						EditorGUI.LabelField(rect, title);
					};
					geoList.onReorderCallback = (ReorderableList list) => {
						GraphEditor.Template.MoveConnection(inputConnections[list.index], list.index);
					};
					geoList.onRemoveCallback = (ReorderableList list) => {
						GraphEditor.Template.Disconnect(inputConnections[list.index]);
						list.list = GraphEditor.Template.ConnectionsTo(op, input);
					};
					geoList.DoLayoutList();
					isConnectedInput = true;

				// Value comes from outlet connection
				} else {
					foreach (IOConnection conn in GraphEditor.Template.Connections) {
						if (op.GUID == conn.To.GUID && input.Name == conn.Input.Name) {

							string valueDescription;

							// If the value is printable, print it
							if (conn.Output.DataType == typeof(System.Int32)
								|| conn.Output.DataType == typeof(System.Single)
								|| conn.Output.DataType == typeof(System.String)
								|| conn.Output.DataType == typeof(System.Boolean)
								|| conn.Output.DataType == typeof(Vector2)
								|| conn.Output.DataType == typeof(Vector3)
								|| conn.Output.DataType == typeof(Vector4)) {
								valueDescription = conn.From.GetValue(conn.Output).ToString();
							}

							// Otherwise print the outlet description
							else {
								valueDescription = System.String.Format("{0}.{1}", conn.From.Metadata.Title, conn.Output.Name);
							}

							EditorGUILayout.LabelField(input.Name, valueDescription);

							isConnectedInput = true;
							continue;
						}
					}
				}

				// Not a List<Geometry>, and not a connected input outlet: draw the input
				if (!isConnectedInput) {

					// Float input
					if (input.DataType == typeof(System.Single)) {
						float newValue = EditorGUILayout.FloatField(input.Name, op.GetValue<float>(input));
						op.SetValue<float>(input, newValue);
					}

					// Integer field
					else if (input.DataType == typeof(System.Int32)) {
						int newValue = EditorGUILayout.IntField(input.Name, op.GetValue<int>(input));
						op.SetValue<int>(input, newValue);
					}

					// String field
					else if (input.DataType == typeof(System.String)) {
						string newValue = EditorGUILayout.TextField(input.Name, op.GetValue<string>(input));
						op.SetValue<string>(input, newValue);
					}

					// Boolean input
					else if (input.DataType == typeof(System.Boolean)) {
						bool newValue = EditorGUILayout.Toggle(input.Name, op.GetValue<bool>(input));
						op.SetValue<bool>(input, newValue);
					}

					// Enum input
					else if (input.DataType.IsEnum) {
						if (input.Member is System.Reflection.FieldInfo) {
							object objValue = ((System.Reflection.FieldInfo) input.Member).GetValue(op);
							string[] enumNames = System.Enum.GetNames(input.DataType);
							System.Array enumValues = System.Enum.GetValues(input.DataType);
							int selectedIndex = System.Array.IndexOf(enumValues, objValue);

							selectedIndex = EditorGUILayout.Popup(input.Name, selectedIndex, enumNames);

							((System.Reflection.FieldInfo) input.Member).SetValue(op, enumValues.GetValue(selectedIndex));
						} else {
							Debug.LogFormat("Enum {0} is not a field", input.Name);
						}
					}

					// Vector2
					else if (input.DataType == typeof(Vector2)) {
						Vector2 newValue = EditorGUILayout.Vector2Field(input.Name, op.GetValue<Vector2>(input));
						op.SetValue<Vector2>(input, newValue);
					}

					// Vector3
					else if (input.DataType == typeof(Vector3)) {
						Vector3 newValue = EditorGUILayout.Vector3Field(input.Name, op.GetValue<Vector3>(input));
						op.SetValue<Vector3>(input, newValue);
					}

					// Vector4
					else if (input.DataType == typeof(Vector4)) {
						Vector4 newValue = EditorGUILayout.Vector4Field(input.Name, op.GetValue<Vector4>(input));
						op.SetValue<Vector4>(input, newValue);
					}

					// Unsupported
					else {
						EditorGUILayout.LabelField(input.Name, input.DataType.ToString());
					}

				} // !isCOnnectedInput
			}

			if (op != null && op.OperatorError != null) {
				EditorGUILayout.HelpBox(op.OperatorError, MessageType.Error);
			}

		}

	}

}