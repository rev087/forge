using UnityEngine;
using UnityEditor;

namespace Forge.Editor {

	[CustomEditor(typeof(Template))]
	public class TemplateEditor : UnityEditor.Editor {

		private bool m_displayJson = false;

		public override void OnInspectorGUI() {
			Template template = (Template) serializedObject.targetObject;
			if (GraphEditor.Template != template) GraphEditor.LoadTemplate(template);

			EditorGUILayout.LabelField(template.ToString());
			EditorGUILayout.LabelField("Operators", template.Operators.Count.ToString());
			EditorGUILayout.LabelField("Connections", template.Connections.Count.ToString());
			
			if (GUILayout.Button("Load Demo")) {
				template.Connections.Clear();
				template.Operators.Clear();
				template.LoadDemo();
				GraphEditor.LoadTemplate(template);
			}

			if (GUILayout.Button("Serialize")) {
				string jsonString = template.Serialize();
				template.JSON = jsonString;
			}

			EditorGUILayout.Space();

			m_displayJson = EditorGUILayout.Foldout(m_displayJson, "JSON");
			if (m_displayJson) {
				EditorGUILayout.TextArea(template.JSON);
			}

			EditorGUILayout.Space();

			foreach (Node node in GraphEditor.Selection.Nodes) {
				NodeInspector.Draw(node.Operator);
				EditorGUILayout.Space();
			}
		}

	}

}