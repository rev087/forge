using UnityEngine;
using UnityEditor;

namespace Forge.Editor {

	public class NodeInspector {

		private static GUIStyle _TitleStyle = null;
		private const float Margin = 5f;
		private static Vector2 _scrollPoint = Vector2.zero;

		public static void Draw(Rect rect) {

			if (_TitleStyle == null) {
				_TitleStyle = new GUIStyle();
				_TitleStyle.fontSize = 16;
			}

			Node activeNode = GraphEditor.Selection.ActiveNode;

			if (activeNode != null) {
				Operator op = activeNode.Operator;

				var controlsRect = new Rect(
					rect.x + Margin,
					rect.y + Margin,
					rect.width - Margin*2,
					Mathf.Max(rect.height, op.Inputs.Length * 25) - Margin*2
				);

				var canvasRect = new Rect(
					rect.x,
					rect.y,
					rect.width,
					Mathf.Max(rect.height, op.Inputs.Length * 25)
				);

				_scrollPoint = GUI.BeginScrollView(rect, _scrollPoint, canvasRect);
				GUILayout.BeginArea(controlsRect);

				EditorGUILayout.LabelField(op.Title, _TitleStyle);
				EditorGUILayout.Space();


				foreach (IOOutlet input in op.Inputs) {
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

					else {
						EditorGUILayout.LabelField(input.Name, input.Type.ToString());
					}
				}

				GUILayout.EndArea();
				GUI.EndScrollView();

			} // activeNode != null

		}

	}

}