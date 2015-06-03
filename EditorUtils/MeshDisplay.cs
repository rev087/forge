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
		public bool DisplayVertexTangent = false;

		public bool DisplayFaces = false;
		public bool DisplayFaceIndex = false;
		public bool DisplayFaceNormal = false;

		public bool DisplayPolygons = false;
		public bool DisplayPolygonIndex = false;

		public bool DisplayUVs = false;
		public bool DisplayOrigin = false;

		private static GUIStyle _vertStyle = null;
		private static GUIStyle _faceStyle = null;
		private static GUIStyle _polyStyle = null;
		private static GUIStyle _uvStyle = null;
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
			Geometry geo = asset.Geometry;
			bool canDisplayVertexData = geo.Vertices.Length <= MAX_VERTEX_COUNT;

			if (_vertStyle == null) {
				_vertStyle = MakeStyle(Color.cyan, new Vector2(0, 0));
				_faceStyle = MakeStyle(Color.red, new Vector2(0f, 0f));
				_polyStyle = MakeStyle(Color.yellow, new Vector2(0f, 0f));
				_uvStyle = MakeStyle(Color.magenta, new Vector2(0f, 0f));
				_shadowStyle = MakeStyle(Color.black, new Vector2(1f, 1f));
			}

			if (DisplayDefaultGizmo) {
				DrawDefaultGizmo(asset);
			}

			Vector3 camPos = SceneView.lastActiveSceneView.camera.transform.position;

			// Face display options
			if (geo.Triangles != null && (DisplayFaces || DisplayFaceIndex || DisplayFaceNormal) && canDisplayVertexData) {

				for (int i = 0; i <= geo.Triangles.Length - 3; i += 3) {
					
					Vector3 aVert = transform.TransformPoint(geo.Vertices[geo.Triangles[i]]);
					Vector3 bVert = transform.TransformPoint(geo.Vertices[geo.Triangles[i+1]]);
					Vector3 cVert = transform.TransformPoint(geo.Vertices[geo.Triangles[i+2]]);

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
						int id = geo.Vertices.Length + nth;
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
						// Handles.Label(la, geo.Triangles[i].ToString(), _faceStyle);
						// Handles.Label(lb, geo.Triangles[i+1].ToString(), _faceStyle);
						// Handles.Label(lc, geo.Triangles[i+2].ToString(), _faceStyle);
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
			if (geo.Vertices != null && (DisplayVertices || DisplayVertexPosition || DisplayVertexNormal ||
				DisplayVertexTangent || DisplayVertexIndex || DisplayUVs) && canDisplayVertexData) {

				for (int i = 0; i < geo.Vertices.Length; i++) {

					Vector3 origin = transform.TransformPoint(geo.Vertices[i]);
					float camDist = Vector3.Distance(origin, camPos);

					Handles.color = Color.cyan;

					// Vertices
					if (DisplayVertices) {
						Handles.DotCap(i * 2, origin, Quaternion.identity, camDist / 220);
					}

					// Normals
					if (DisplayVertexNormal && i < geo.Normals.Length) {
						Vector3 normalEnd = Vector3.ClampMagnitude(geo.Normals[i], camDist / 20);
						Handles.DrawLine(origin, origin + normalEnd);
					}

					// Tangents
					if (DisplayVertexTangent && i < geo.Tangents.Length) {
						Vector3 normalEnd = Vector3.ClampMagnitude(geo.Tangents[i], camDist / 20);
						Handles.DrawDottedLine(origin, origin + normalEnd, 2f);
					}

					// Index and Position
					if (DisplayVertexIndex || DisplayVertexPosition) {
						string label = "";
						if (DisplayVertexIndex) {
							label += i + "\n";
						}
						if (DisplayVertexPosition) {
							label += geo.Vertices[i].ToString() + "\n";
						}
						Handles.Label(origin, label, _shadowStyle);
						Handles.Label(origin, label, _vertStyle);
					}

					// UVs
					if (DisplayUVs) {
						string label = geo.UV[i].ToString();
						Handles.Label(origin, label, _shadowStyle);
						Handles.Label(origin, label, _uvStyle);
					}

				} // vertices
				
			} // vertex display options

			// Polygons
			if (geo.Polygons != null && (DisplayPolygons || DisplayPolygonIndex) && canDisplayVertexData) {
				Handles.color = Color.yellow;
				for (int p = 0; p < geo.Polygons.Length; p += 2) {
					int start = geo.Polygons[p];
					int count = geo.Polygons[p+1];
					Vector3 origin = transform.TransformPoint(geo.Vertices[start]);

					// Index
					if (DisplayPolygonIndex) {
						Handles.Label(origin, (p/2).ToString(), _shadowStyle);
						Handles.Label(origin, (p/2).ToString(), _polyStyle);
					}

					if (DisplayPolygons) {

						// Cap
						float camDist = Vector3.Distance(origin, camPos);
						Handles.DotCap(GUIUtility.GetControlID (FocusType.Passive), origin, Quaternion.identity, camDist / 220);

						// Lines
						for (int v = start; v < start + count; v++) {
							Vector3 point = transform.TransformPoint(geo.Vertices[v]);
							if (v > start) {
								Vector3 prev = transform.TransformPoint(geo.Vertices[v-1]);
								Handles.DrawLine(point, prev);
							} else {
								Vector3 prev = transform.TransformPoint(geo.Vertices[start+count-1]);
								Handles.DrawDottedLine(point, prev, 4f);
							}
						}
					}

				}
			}

			// Misc: Origin
			if (DisplayOrigin) {
				Handles.color = Color.green;
				Vector3 origin = transform.TransformPoint(Vector3.zero);
				int originId = geo.Vertices.Length + geo.Triangles.Length / 3 + 1;
				float camDist = Vector3.Distance(origin, camPos);
				Handles.DotCap(originId, origin, Quaternion.identity, camDist / 180);
			}

			// Misc: Ghost Mesh
			if (asset.GhostMesh != null) {
				Gizmos.color = new Color(0.3f, 0.65f, 1f, 0.7f);
				Gizmos.DrawMesh(asset.GhostMesh, transform.position, transform.rotation, transform.localScale);
			}

		} // DrawHandles

	} // class
	
} // namespace