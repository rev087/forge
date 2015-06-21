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
			foreach (Node node in GraphEditor.Selection.Nodes) {
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

			if (_TitleStyle == null) {
				_TitleStyle = new GUIStyle();
				_TitleStyle.fontSize = 16;
			}

			EditorGUILayout.LabelField(op.Title, _TitleStyle);
			EditorGUILayout.Space();

			op.IsGeometryOutput = EditorGUILayout.Toggle("Geometry Output", op.IsGeometryOutput);

			foreach (IOOutlet input in op.Inputs) {

				// Value comes from outlet connection
				foreach (IOConnection conn in GraphEditor.Template.Connections) {
					if (op.GUID == conn.To.GUID && input.Name == conn.Input.Name) {
						var valueFrom = System.String.Format("{0}.{1}", conn.From.Title, conn.Output.Name);
						EditorGUILayout.LabelField(input.Name, valueFrom);
						continue;
					}
				}

				// Float input
				if (input.Type == typeof(System.Single)) {
					float newValue = EditorGUILayout.FloatField(input.Name, op.GetValue<float>(input));
					op.SetValue<float>(input, newValue);
				}

				// Integer field
				else if (input.Type == typeof(System.Int32)) {
					int newValue = EditorGUILayout.IntField(input.Name, op.GetValue<int>(input));
					op.SetValue<int>(input, newValue);
				}

				// Boolean input
				else if (input.Type == typeof(System.Boolean)) {
					bool newValue = EditorGUILayout.Toggle(input.Name, op.GetValue<bool>(input));
					op.SetValue<bool>(input, newValue);
				}

				// Enum input
				else if (input.Type.IsEnum) {
					if (input.Member is System.Reflection.FieldInfo) {
						object objValue = ((System.Reflection.FieldInfo) input.Member).GetValue(op);
						string[] enumNames = System.Enum.GetNames(input.Type);
						System.Array enumValues = System.Enum.GetValues(input.Type);
						int selectedIndex = System.Array.IndexOf(enumValues, objValue);

						selectedIndex = EditorGUILayout.Popup(input.Name, selectedIndex, enumNames);

						((System.Reflection.FieldInfo) input.Member).SetValue(op, enumValues.GetValue(selectedIndex));
					} else {
						Debug.LogFormat("Enum {0} is not a field", input.Name);
					}
				}

				// Vector3
				else if (input.Type == typeof(Vector3)) {
					Vector3 newValue = EditorGUILayout.Vector3Field(input.Name, op.GetValue<Vector3>(input));
					op.SetValue<Vector3>(input, newValue);
				}

				// Unsupported
				else {
					EditorGUILayout.LabelField(input.Name, input.Type.ToString());
				}
			}

		}

	}

}