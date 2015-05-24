using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Forge.EditorUtils {

	public class MeshDisplay : ScriptableObject {

		public bool DisplayVertices = false;
		public bool DisplayVertexPosition = false;
		public bool DisplayVertexIndex = false;
		public bool DisplayVertexNormal = false;

		public bool DisplayFaces = false;
		public bool DisplayFaceIndex = false;
		public bool DisplayFaceNormal = false;

		public bool DisplayOrigin = false;

		private static GUIStyle _vertStyle = null;
		private static GUIStyle _faceStyle = null;
		private static GUIStyle _shadowStyle = null;
		private static GUIStyle _miscStyle = null;

		public void DrawHandles(Mesh mesh, Transform transform) {
			if (_vertStyle == null) {
				_vertStyle = new GUIStyle();
				_vertStyle.normal.textColor = Color.cyan;
				_vertStyle.fontSize = 16;
				_vertStyle.contentOffset = new Vector2(-5f, 5f);

				_faceStyle = new GUIStyle();
				_faceStyle.normal.textColor = Color.red;
				_faceStyle.fontSize = 16;
				_faceStyle.contentOffset = new Vector2(-5f, 5f);

				_miscStyle = new GUIStyle();
				_miscStyle.normal.textColor = Color.yellow;
				_miscStyle.fontSize = 16;
				_miscStyle.contentOffset = new Vector2(-5f, 5f);

				_shadowStyle = new GUIStyle();
				_shadowStyle.normal.textColor = Color.black;
				_shadowStyle.fontSize = 16;
				_shadowStyle.contentOffset = new Vector2(-4f, 6f);
			}

			Vector3 camPos = SceneView.lastActiveSceneView.camera.transform.position;

			if (mesh != null && (DisplayVertices || DisplayVertexPosition ||
				DisplayVertexNormal || DisplayVertexIndex || DisplayOrigin)) {

				// Vertex based handles
				for (int i = 0; i < mesh.vertices.Length; i++) {

					Vector3 origin = transform.TransformPoint(mesh.vertices[i]);
					float camDist = Vector3.Distance(origin, camPos);

					Handles.color = Color.cyan;

					// Normals
					if (DisplayVertexNormal) {
						Vector3 normalEnd = Vector3.ClampMagnitude(mesh.normals[i], camDist / 20);
						Handles.DrawAAPolyLine(origin, origin + normalEnd);
					}

					// "Edges"
					// if (i < mesh.vertices.Length - 1) {
					// 	Vector3 next = transform.TransformPoint(mesh.vertices[i+1]);
					// 	Handles.DrawAAPolyLine(origin, next);
					// }

					// Vertices
					if (DisplayVertices) {
						Handles.DotCap(i * 2, origin, Quaternion.identity, camDist / 180);
					}

					// Vertex Index and Position
					if (DisplayVertexIndex || DisplayVertexPosition) {
						string label = "";
						if (DisplayVertexIndex) {
							label += i + "\n";
						}
						if (DisplayVertexPosition) {
							label += mesh.vertices[i].ToString() + "\n";
						}
						Handles.Label(origin, label, _shadowStyle);
						Handles.Label(origin, label, _vertStyle);
					}

				} // vertices

				// Origin
				if (DisplayOrigin) {
					Handles.color = Color.yellow;
					float camDist = Vector3.Distance(Vector3.zero, camPos);
					int originId = mesh.vertices.Length + mesh.triangles.Length/3 + 1;
					Handles.DotCap(originId, Vector3.zero, Quaternion.identity, camDist / 180);
				}
				
			} // if

			if (mesh != null && (DisplayFaces || DisplayFaceIndex || DisplayFaceNormal)) {

				// Triangle based handles
				for (int a = 0; a <= mesh.triangles.Length - 3; a += 3) {

					// Faces
					Handles.color = Color.red;
					Vector3 aVert = mesh.vertices[mesh.triangles[a]];
					Vector3 bVert = mesh.vertices[mesh.triangles[a+1]];
					Vector3 cVert = mesh.vertices[mesh.triangles[a+2]];

					Vector3 mid = transform.TransformPoint((aVert + bVert + cVert) / 3);
					float camDist = Vector3.Distance(mid, camPos);

					int nth = a / 3;
					int id = mesh.vertices.Length + nth;

					if (DisplayFaces) {
						Handles.DotCap(id, mid, Quaternion.identity, camDist / 180);
					}

					if (DisplayFaceNormal) {
						Vector3 normal = Vector3.ClampMagnitude(Vector3.Cross(bVert-aVert, cVert-aVert), camDist / 20);
						Handles.DrawAAPolyLine(mid, mid + normal);
					}

					if (DisplayFaceIndex) {
						Handles.Label(mid, "" + nth, _shadowStyle);
						Handles.Label(mid, "" + nth, _faceStyle);
					}

					// Vertex Index and Position
					if (DisplayFaceIndex) {
						Handles.Label(mid, "" + nth, _shadowStyle);
						Handles.Label(mid, "" + nth, _faceStyle);
					}

				} // triangles

			} // if

		} // public void DrawHandles

	} // public class MeshDisplay
	
}