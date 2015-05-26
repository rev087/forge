using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Forge.EditorUtils {

	public class MeshDisplay : ScriptableObject {

		public bool DisplayDefaultGizmo = false;

		public bool DisplayVertices = false;
		public bool DisplayVertexPosition = false;
		public bool DisplayVertexIndex = false;
		public bool DisplayVertexNormal = false;

		public bool DisplayFaces = false;
		public bool DisplayFaceIndex = false;
		public bool DisplayFaceNormal = false;

		public bool DisplayOrigin = false;
		public bool DisplayPolygon = false;

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

		private void DrawDefaultGizmo(ProceduralAsset asset) {
			// Workaround for the disappearing default gizmo
			switch (Tools.current){
				case Tool.Move:
					asset.transform.position = Handles.PositionHandle(asset.transform.position, Quaternion.identity);
					break;
				case Tool.Rotate:
					asset.transform.rotation = Handles.RotationHandle(asset.transform.rotation, asset.transform.position);
					break;
				case Tool.Scale:
					asset.transform.localScale = Handles.ScaleHandle(
						asset.transform.localScale,
						asset.transform.position,
						asset.transform.rotation,
						HandleUtility.GetHandleSize(asset.transform.position)
					);
					break;
				case Tool.View:
					break;
				// case Tool.Rect:
				// 	break;
				case Tool.None:
					break;
			}
		}

		public void DrawHandles(ProceduralAsset asset, Transform transform) {
			Mesh mesh = asset.Mesh;

			if (_vertStyle == null) {
				_vertStyle = MakeStyle(Color.cyan, 16, new Vector2(-5f, 5f));
				_faceStyle = MakeStyle(Color.red, 16, new Vector2(-5f, 5f));
				_shadowStyle = MakeStyle(Color.black, 16, new Vector2(-4f, 6f));
			}

			if (DisplayDefaultGizmo) {
				DrawDefaultGizmo(asset);
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
				DisplayVertexNormal || DisplayVertexIndex)) {

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

					// Vertex Polygon
					if (DisplayPolygon) {
						Handles.color = Color.yellow;
						if (i > 0) {
							Vector3 prev = transform.TransformPoint(mesh.vertices[i-1]);
							Handles.DrawLine(origin, prev);
						} else {
							Vector3 prev = transform.TransformPoint(mesh.vertices[mesh.vertices.Length-1]);
							Handles.DrawDottedLine(origin, prev, 4f);
						}
					}

				} // vertices
				
			} // vertex display options

			// Origin
			if (DisplayOrigin) {
				Handles.color = Color.yellow;
				float camDist = Vector3.Distance(Vector3.zero, camPos);
				int originId = mesh.vertices.Length + mesh.triangles.Length / 3 + 1;
				Handles.DotCap(originId, transform.TransformPoint(Vector3.zero), Quaternion.identity, camDist / 180);
			}

		} // public void DrawHandles

	} // public class MeshDisplay
	
}