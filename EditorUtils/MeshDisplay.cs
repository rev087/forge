using UnityEngine;
using System.Collections;
using Forge.Util;

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

		public const int MAX_VERTEX_COUNT = 1000;

		private GUIStyle MakeStyle(Color textColor, Vector2 contentOffset) {
			var style = new GUIStyle();
			style.normal.textColor = textColor;
			style.fontSize = 16;
			style.contentOffset = contentOffset;
			style.alignment = TextAnchor.UpperCenter;
			style.fixedHeight = 20f;
			style.fixedWidth = 100f;
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
			if (!asset.IsBuilt) asset.Generate();
			Mesh mesh = asset.Mesh;
			bool canDisplayVertexData = mesh.vertices.Length <= MAX_VERTEX_COUNT;

			if (_vertStyle == null) {
				_vertStyle = MakeStyle(Color.cyan, new Vector2(0, 0));
				_faceStyle = MakeStyle(Color.red, new Vector2(0f, 0f));
				_shadowStyle = MakeStyle(Color.black, new Vector2(1f, 1f));
			}

			if (DisplayDefaultGizmo) {
				DrawDefaultGizmo(asset);
			}

			Vector3 camPos = SceneView.lastActiveSceneView.camera.transform.position;

			// Face display options
			if (mesh != null && (DisplayFaces || DisplayFaceIndex || DisplayFaceNormal) && canDisplayVertexData) {

				for (int i = 0; i <= mesh.triangles.Length - 3; i += 3) {
					
					Vector3 aVert = transform.TransformPoint(mesh.vertices[mesh.triangles[i]]);
					Vector3 bVert = transform.TransformPoint(mesh.vertices[mesh.triangles[i+1]]);
					Vector3 cVert = transform.TransformPoint(mesh.vertices[mesh.triangles[i+2]]);

					Vector3 mid = (aVert + bVert + cVert) / 3;

					float camDist = Vector3.Distance(mid, camPos);

					int nth = i / 3;

					// Faces
					if (DisplayFaces) {
						Handles.color = new Color(1f, 0f, 0f, 0.15f);
						var tri = new Vector3[] {
							Vector3.Lerp(aVert, mid, 0.025f),
							Vector3.Lerp(bVert, mid, 0.025f),
							Vector3.Lerp(cVert, mid, 0.025f)
						};
						Handles.DrawAAConvexPolygon(tri);

						Handles.color = new Color(1f, 0f, 0f, 0.6f);
						int id = mesh.vertices.Length + nth;
						Handles.DotCap(id, mid, Quaternion.identity, camDist / 220);
					}

					// Normals
					if (DisplayFaceNormal) {
						Handles.color = Color.red;
						Vector3 normal = Vector3.ClampMagnitude(Vector3.Cross(bVert-aVert, cVert-aVert).normalized, camDist / 20);
						Handles.DrawPolyLine(mid, mid + normal);

						// Face vertex order arrows
						// Vector3 la = Vector3.Lerp(aVert, mid, 0.15f);
						// Vector3 lb = Vector3.Lerp(bVert, mid, 0.15f);
						// Vector3 lc = Vector3.Lerp(cVert, mid, 0.15f);
						// Handles.DrawLine(la, Vector3.Lerp(la, lb, 0.25f));
						// Handles.DrawLine(lb, Vector3.Lerp(lb, lc, 0.25f));
						// Handles.DrawLine(lc, Vector3.Lerp(lc, la, 0.25f));
						// Handles.Label(la, mesh.triangles[i].ToString(), _faceStyle);
						// Handles.Label(lb, mesh.triangles[i+1].ToString(), _faceStyle);
						// Handles.Label(lc, mesh.triangles[i+2].ToString(), _faceStyle);
					}

					// Index
					if (DisplayFaceIndex) {
						Handles.color = Color.red;
						Handles.Label(mid, "" + nth, _shadowStyle);
						Handles.Label(mid, "" + nth, _faceStyle);
					}

				} // triangles

			} // face display options

			// Vertex display options
			if (mesh != null && (DisplayVertices || DisplayVertexPosition ||
				DisplayVertexNormal || DisplayVertexIndex || DisplayPolygon) && canDisplayVertexData) {

				for (int i = 0; i < mesh.vertices.Length; i++) {

					Vector3 origin = transform.TransformPoint(mesh.vertices[i]);
					float camDist = Vector3.Distance(origin, camPos);

					Handles.color = Color.cyan;

					// Vertices
					if (DisplayVertices) {
						Handles.DotCap(i * 2, origin, Quaternion.identity, camDist / 220);
					}

					// Normals
					if (DisplayVertexNormal && i < mesh.normals.Length) {
						Vector3 normalEnd = Vector3.ClampMagnitude(mesh.normals[i], camDist / 20);
						Handles.DrawPolyLine(origin, origin + normalEnd);
					}

					// Index and Position
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

					// Misc: Polygon
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

			// Misc: Origin
			if (DisplayOrigin) {
				Handles.color = Color.yellow;
				float camDist = Vector3.Distance(Vector3.zero, camPos);
				int originId = mesh.vertices.Length + mesh.triangles.Length / 3 + 1;
				Handles.DotCap(originId, transform.TransformPoint(Vector3.zero), Quaternion.identity, camDist / 180);
			}

			// Misc: Ghost Mesh
			if (asset.GhostMesh != null) {
				Gizmos.color = new Color(0.3f, 0.65f, 1f, 0.7f);
				Gizmos.DrawMesh(asset.GhostMesh, transform.position, transform.rotation, transform.localScale);
			}

		} // DrawHandles

	} // class
	
} // namespace