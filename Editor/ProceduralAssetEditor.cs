using UnityEngine;
using UnityEditor;
using Forge.EditorUtils;

namespace Forge.Editor {

	[CustomEditor(typeof(ProceduralAsset), true)]
	public class ProceduralAssetEditor : UnityEditor.Editor {

		private static IconLoader IconLoader = null;

		private static bool ShowParameters = true;
		private static bool ShowStatistics = true;
		private static bool ShowDataFile = true;

		private ProceduralAsset Asset = null;

		public override void OnInspectorGUI() {
			if (Asset == null)
				Asset = (ProceduralAsset) serializedObject.targetObject;

			DrawDefaultInspector();

			// Parameters
			ShowParameters = EditorGUILayout.Foldout(ShowParameters, "Parameters");
			if (ShowParameters) {
				if (Asset.Template != null) {
					Parameter[] parameters = Asset.Template.Parameters;
					for (int i = 0; i <  parameters.Length; i++) {
						Parameter param = parameters[i];
						var parameterGUI = param.ParameterGUI;
						if (parameterGUI != null) {
							object paramValue = Asset.GetParameter(param.GUID);
							paramValue = param.ParameterGUI.Invoke(param, new object[] { paramValue });
							Asset.SetParameterByGUID(param.GUID, paramValue);
						}
					}
				}
			}

			if (GUI.changed || GUILayout.Button("Rebuild")) {
				Asset.Generate();
			}

			// Statistics
			ShowStatistics = EditorGUILayout.Foldout(ShowStatistics, "Statistics");
			if (ShowStatistics) {
				EditorGUILayout.LabelField("Last build time: ", Asset.IsBuilt ? System.String.Format("{0}ms", Asset.LastBuildTime) : "-");
				EditorGUILayout.LabelField("Vertices: " , Asset.IsBuilt ? Asset.Geometry.Vertices.Length.ToString()  : "-");
				EditorGUILayout.LabelField("Triangles: ", Asset.IsBuilt ? (Asset.Geometry.Triangles.Length/3).ToString() : "-");
				EditorGUILayout.LabelField("Polygons: " , Asset.IsBuilt ? (Asset.Geometry.Polygons.Length/2).ToString()  : "-");
			}

			// Data File
			ShowDataFile = EditorGUILayout.Foldout(ShowDataFile, "Mesh Data File");
			if (ShowDataFile) {
				GUILayout.BeginHorizontal();

				string assetPath = AssetDatabase.GetAssetPath(Asset.Mesh);
				Asset.Mesh = (Mesh) EditorGUILayout.ObjectField(Asset.Mesh, typeof(Mesh), false);
				
				if (!string.IsNullOrEmpty(assetPath)) {
					if (GUILayout.Button("Disconnect")) {
						Asset.Mesh = new Mesh();
						Asset.Generate();
					}				
				} else {
					if (GUILayout.Button("Save as...")) {
						SaveAsset();
					}
				}

				GUILayout.EndHorizontal();
			}

		}

		private void SaveAsset() {
			if (Asset.Mesh != null) {

				string assetPath = AssetDatabase.GetAssetPath(Asset.Mesh);
				assetPath = EditorUtility.SaveFilePanelInProject("Save Mesh", "Untitled", "asset", "Please enter a name to save the generated mesh data");
				if (string.IsNullOrEmpty(assetPath)) return;

				if (!AssetDatabase.Contains(Asset.Mesh)) {
					AssetDatabase.CreateAsset(Asset.Mesh, assetPath);
				}
				
				AssetDatabase.SaveAssets();
			} else {
				Debug.Log("No mesh data");
			}
		}

		void OnSceneGUI() {
			if (Asset == null) return;
			if (IconLoader == null) IconLoader = new IconLoader();

			float height = 32f * 13 + 30f;
			float width = 32f;

			bool disableVertexData = Asset.Mesh.vertices.Length >= MeshDisplay.MAX_VERTEX_COUNT;

			var rect = new Rect(Screen.width - width - 10, Screen.height/2 - height/2, width, height);

			GUILayout.BeginArea(rect);

			// Default Gizmo
			Asset.MeshDisplay.DisplayDefaultGizmo = GUILayout.Toggle(Asset.MeshDisplay.DisplayDefaultGizmo, IconLoader.Icons["defaultGizmo"], "button");
			GUILayout.Space(10);

			// Vertex Display
			EditorGUI.BeginDisabledGroup(disableVertexData);
			Asset.MeshDisplay.DisplayVertices = GUILayout.Toggle(Asset.MeshDisplay.DisplayVertices, IconLoader.Icons["vertex"], "button");
			Asset.MeshDisplay.DisplayVertexNormal = GUILayout.Toggle(Asset.MeshDisplay.DisplayVertexNormal, IconLoader.Icons["vertexNormal"], "button");
			Asset.MeshDisplay.DisplayVertexTangent = GUILayout.Toggle(Asset.MeshDisplay.DisplayVertexTangent, IconLoader.Icons["vertexTangent"], "button");
			Asset.MeshDisplay.DisplayVertexIndex = GUILayout.Toggle(Asset.MeshDisplay.DisplayVertexIndex, IconLoader.Icons["vertexIndex"], "button");
			Asset.MeshDisplay.DisplayVertexPosition = GUILayout.Toggle(Asset.MeshDisplay.DisplayVertexPosition, IconLoader.Icons["vertexPosition"], "button");
			EditorGUI.EndDisabledGroup();
			GUILayout.Space(10);

			// Face Display
			EditorGUI.BeginDisabledGroup(disableVertexData);
			Asset.MeshDisplay.DisplayFaces = GUILayout.Toggle(Asset.MeshDisplay.DisplayFaces, IconLoader.Icons["face"], "button");
			//Asset.MeshDisplay.DisplayFaceOrder = GUILayout.Toggle(Asset.MeshDisplay.DisplayFaceOrder, IconLoader.Icons["face"], "button");
			Asset.MeshDisplay.DisplayFaceNormal = GUILayout.Toggle(Asset.MeshDisplay.DisplayFaceNormal, IconLoader.Icons["faceNormal"], "button");
			Asset.MeshDisplay.DisplayFaceIndex = GUILayout.Toggle(Asset.MeshDisplay.DisplayFaceIndex, IconLoader.Icons["faceIndex"], "button");
			EditorGUI.EndDisabledGroup();
			GUILayout.Space(10);

			// Polygons
			EditorGUI.BeginDisabledGroup(disableVertexData);
			Asset.MeshDisplay.DisplayPolygons = GUILayout.Toggle(Asset.MeshDisplay.DisplayPolygons, IconLoader.Icons["polygon"], "button");
			Asset.MeshDisplay.DisplayPolygonIndex = GUILayout.Toggle(Asset.MeshDisplay.DisplayPolygonIndex, IconLoader.Icons["polygonIndex"], "button");
			EditorGUI.EndDisabledGroup();

			GUILayout.Space(10);

			// UV
			EditorGUI.BeginDisabledGroup(disableVertexData);
			Asset.MeshDisplay.DisplayUVs = GUILayout.Toggle(Asset.MeshDisplay.DisplayUVs, IconLoader.Icons["uv"], "button");
			EditorGUI.EndDisabledGroup();

			// origin
			Asset.MeshDisplay.DisplayOrigin = GUILayout.Toggle(Asset.MeshDisplay.DisplayOrigin, IconLoader.Icons["origin"], "button");

			GUILayout.EndArea();
		}

	}
	
}