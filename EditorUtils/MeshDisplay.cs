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

		private GUIStyle MakeStyle(Color textColor, int fontSize, Vector2 contentOffset) {
			var style = new GUIStyle();
			style.normal.textColor = textColor;
			style.fontSize = fontSize;
			style.contentOffset = contentOffset;
			return style;
		}

		public void DrawHandles(Mesh mesh, Transform transform) {
			if (_vertStyle == null) {
				_vertStyle = MakeStyle(Color.cyan, 16, new Vector2(-5f, 5f));
				_faceStyle = MakeStyle(Color.red, 16, new Vector2(-5f, 5f));
				_shadowStyle = MakeStyle(Color.black, 16, new Vector2(-4f, 6f));
			}

			Vector3 camPos = SceneView.lastActiveSceneView.camera.transform.position;

			if (mesh != null && (DisplayFaces || DisplayFaceIndex || DisplayFaceNormal)) {

				// Triangle based handles
				for (int a = 0; a <= mesh.triangles.Length - 3; a += 3) {
					
					Vector3 aVert = transform.TransformPoint(mesh.vertices[mesh.triangles[a]]);
					Vector3 bVert = transform.TransformPoint(mesh.vertices[mesh.triangles[a+1]]);
					Vector3 cVert = transform.TransformPoint(mesh.vertices[mesh.triangles[a+2]]);

					Vector3 mid = (aVert + bVert + cVert) / 3;

					Vector3 la = Vector3.Lerp(aVert, mid, 0.025f);
					Vector3 lb = Vector3.Lerp(bVert, mid, 0.025f);
					Vector3 lc = Vector3.Lerp(cVert, mid, 0.025f);

					float camDist = Vector3.Distance(mid, camPos);

					int nth = a / 3;

					if (DisplayFaces) {
						Handles.color = new Color(1f, 0f, 0f, 0.15f);
						var tri = new Vector3[] {la, lb, lc};
						Handles.DrawAAConvexPolygon(tri);

						Handles.color = new Color(1f, 0f, 0f, 0.6f);
						int id = mesh.vertices.Length + nth;
						Handles.DotCap(id, mid, Quaternion.identity, camDist / 220);
					}


					if (DisplayFaceNormal) {
						Handles.color = Color.red;
						Vector3 normal = Vector3.ClampMagnitude(Vector3.Cross(bVert-aVert, cVert-aVert), camDist / 20);
						Handles.DrawPolyLine(mid, mid + normal);
					}

					if (DisplayFaceIndex) {
						Handles.color = Color.red;
						Handles.Label(mid, "" + nth, _shadowStyle);
						Handles.Label(mid, "" + nth, _faceStyle);
					}

				} // triangles

			} // if

			if (mesh != null && (DisplayVertices || DisplayVertexPosition ||
				DisplayVertexNormal || DisplayVertexIndex || DisplayOrigin)) {

				// Vertex based handles
				for (int i = 0; i < mesh.vertices.Length; i++) {

					Vector3 origin = transform.TransformPoint(mesh.vertices[i]);
					float camDist = Vector3.Distance(origin, camPos);

					Handles.color = Color.cyan;

					// Normals
					if (DisplayVertexNormal && i < mesh.normals.Length) {
						Vector3 normalEnd = Vector3.ClampMagnitude(mesh.normals[i], camDist / 20);
						Handles.DrawPolyLine(origin, origin + normalEnd);
					}

					// Vertices
					if (DisplayVertices) {
						Handles.DotCap(i * 2, origin, Quaternion.identity, camDist / 220);
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
					Handles.DotCap(originId, transform.TransformPoint(Vector3.zero), Quaternion.identity, camDist / 180);
				}
				
			} // if

		} // public void DrawHandles

	} // public class MeshDisplay
	
}