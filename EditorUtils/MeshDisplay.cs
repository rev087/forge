using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Forge.EditorUtils {

	public class MeshDisplay {

		public bool DisplayVertices = false;
		public bool DisplayVertexIndex = false;
		public bool DisplayVertexNormal = false;

		public bool DisplayFaces = false;
		public bool DisplayFaceIndex = false;
		public bool DisplayFaceNormal = false;

		private static GUIStyle _vertStyle = null;
		private static GUIStyle _faceStyle = null;
		private static GUIStyle _shadowStyle = null;

		public void DrawHandles(Mesh mesh, Transform transform) {
			if (_vertStyle == null) {
				_vertStyle = new GUIStyle();
				_vertStyle.normal.textColor = Color.cyan;
				_vertStyle.fontSize = 16;

				_faceStyle = new GUIStyle();
				_faceStyle.normal.textColor = Color.red;
				_faceStyle.fontSize = 16;

				_shadowStyle = new GUIStyle();
				_shadowStyle.normal.textColor = Color.black;
				_shadowStyle.fontSize = 16;
				_shadowStyle.contentOffset = new Vector2(1f, 1f);
			}

			if (mesh != null && (DisplayVertexNormal || DisplayVertices || DisplayVertexIndex)) {

				// Vertex based handles
				for (int i = 0; i < mesh.vertices.Length; i++) {

					Vector3 origin = transform.TransformPoint(mesh.vertices[i]);
					Vector3 dir = Vector3.ClampMagnitude(mesh.normals[i], .25f);

					Handles.color = Color.cyan;

					// Normals
					if (DisplayVertexNormal) {
						Handles.DrawAAPolyLine(origin, origin + dir);
					}

					// Vertices
					if (DisplayVertices) {
						Handles.DotCap(i*2, origin, Quaternion.identity, 0.01f);
					}

					// Vertex Index
					if (DisplayVertexIndex) {
						Handles.Label(origin, ""+i, _shadowStyle);
						Handles.Label(origin, ""+i, _vertStyle);
					}

				} // vertices
				
			} // if

			if (mesh != null && (DisplayFaces || DisplayFaceIndex || DisplayFaceNormal)) {

				// Triangle based handles
				for (int a = 0; a <= mesh.triangles.Length - 3; a += 3) {

					// Faces
					if (true) {
						Handles.color = Color.red;
						Vector3 aVert = mesh.vertices[mesh.triangles[a]];
						Vector3 bVert = mesh.vertices[mesh.triangles[a+1]];
						Vector3 cVert = mesh.vertices[mesh.triangles[a+2]];

						Vector3 mid = transform.TransformPoint((aVert + bVert + cVert) / 3);
						Vector3 normal = Vector3.ClampMagnitude(Vector3.Cross(bVert-aVert, cVert-aVert), .25f);

						int nth = a / 3;
						int id = mesh.vertices.Length * 2 + nth;

						if (DisplayFaces) {
							Handles.DotCap(id, mid, Quaternion.identity, 0.01f);
						}

						if (DisplayFaceNormal) {
							Handles.DrawAAPolyLine(mid, mid + normal);
						}

						if (DisplayFaceIndex) {
							Handles.Label(mid, "" + nth, _shadowStyle);
							Handles.Label(mid, "" + nth, _faceStyle);
						}
					}

				} // triangles

			} // if

		} // public void DrawHandles

	} // public class MeshDisplay
	
}