using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Forge.EditorUtils {

	[CustomEditor(typeof(ProceduralAsset), true)]
	public class ProceduralAssetEditor : Editor {

		private static bool ShowDisplayControls = true;
		private static bool ShowStatistics = true;
		private static bool ShowDataFile = true;

		private ProceduralAsset Asset = null;

		public override void OnInspectorGUI() {
			if (Asset == null)
				Asset = (ProceduralAsset) serializedObject.targetObject;

			DrawDefaultInspector();

			if (GUI.changed || GUILayout.Button("Build")) {
				Asset.Generate();
			}

			// Display Controls
			ShowDisplayControls = EditorGUILayout.Foldout(ShowDisplayControls, "Display");
			if (ShowDisplayControls) {
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Vertex", GUILayout.Width(40f));
				Asset.DisplayVertices = GUILayout.Toggle(Asset.DisplayVertices, "Position", "button");
				Asset.DisplayVertexIndex = GUILayout.Toggle(Asset.DisplayVertexIndex, "Index", "button");
				Asset.DisplayVertexNormal = GUILayout.Toggle(Asset.DisplayVertexNormal, "Normal", "button");
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Face", GUILayout.Width(40f));
				Asset.DisplayFaces = GUILayout.Toggle(Asset.DisplayFaces, "Position", "button");
				Asset.DisplayFaceIndex = GUILayout.Toggle(Asset.DisplayFaceIndex, "Index", "button");
				Asset.DisplayFaceNormal = GUILayout.Toggle(Asset.DisplayFaceNormal, "Normal", "button");
				GUILayout.EndHorizontal();
			}

			// Statistics
			ShowStatistics = EditorGUILayout.Foldout(ShowStatistics, "Statistics");
			if (ShowStatistics) {
				EditorGUILayout.LabelField("Last build time: ", Asset.IsBuilt ? System.String.Format("{0}ms", Asset.BuildMilliseconds) : "-");
				EditorGUILayout.LabelField(
					System.String.Format("Vertices: {0}", Asset.IsBuilt ? Asset.VertexCount.ToString() : "-"),
					System.String.Format("Triangles: {0}", Asset.IsBuilt ? Asset.TriangleCount.ToString() : "-")
				);
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
				Asset.Mesh.Optimize();
				AssetDatabase.SaveAssets();
			} else {
				Debug.Log("No mesh data");
			}
		}

	}
	
}