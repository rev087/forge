using UnityEngine;
using UnityEditor;

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

		public static void DrawNodeInspector(Operator op) {

			if (GraphEditor.Template == null) return;

			if (_TitleStyle == null) {
				_TitleStyle = new GUIStyle();
				_TitleStyle.fontSize = 16;
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(op.Title, _TitleStyle, GUILayout.Width(GraphEditor.SidebarWidth/2-20f));
			if (op.IsGeometryOutput) {
				EditorGUILayout.LabelField("(output)", GUILayout.Width(GraphEditor.SidebarWidth/2-20f));
			} else {
				if (GUILayout.Button("Output")) {
					foreach (var kvp in GraphEditor.Template.Operators) {
						kvp.Value.IsGeometryOutput = kvp.Value.GUID == op.GUID;
					}
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

			if (op.IsGeometryOutput) {
				EditorGUILayout.LabelField("[Geometry Output]");
			}

			foreach (IOOutlet input in op.Inputs) {

				var isConnectedInput = false;

				// Value comes from outlet connection
				foreach (IOConnection conn in GraphEditor.Template.Connections) {
					if (op.GUID == conn.To.GUID && input.Name == conn.Input.Name) {
						var valueFrom = System.String.Format("{0}.{1}", conn.From.Title, conn.Output.Name);
						EditorGUILayout.LabelField(input.Name, valueFrom);
						isConnectedInput = true;
						continue;
					}
				}

				// Not connected, draw the input
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

					// Unsupported
					else {
						EditorGUILayout.LabelField(input.Name, input.DataType.ToString());
					}

				} // !isCOnnectedInput
			}

			if (GUILayout.Button("Delete Selection")) {
				foreach (var node in GraphEditor.Selection.Nodes) {
					GraphEditor.Template.RemoveOperator(node.Operator);
				}
				GraphEditor.Selection.Clear();
			}

			if (op != null && op.OperatorError != null) {
				EditorGUILayout.HelpBox(op.OperatorError, MessageType.Error);
			}

		}

	}

}