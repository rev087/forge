using UnityEngine;
using Forge.Extensions;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Forge.Operators {

	[OperatorMetadata(Category = "UV", Title = "Cylindrical Projection")]
	class CylindricalProjection : Operator {
		private Geometry _geometry = Geometry.Empty;

		[Input]
		public void Input(Geometry geometry) {
			_geometry = geometry.Copy();
		}
		[Input]
		public Axis Axis = Axis.X;

		[Output]
		public Geometry Output() {

			Geometry output = _geometry.Copy();

			int u = 0, v = 0, w = 0;

			switch (Axis) {
				case Axis.X:
					u = (int)Axis.Y; v = (int)Axis.X; w = (int)Axis.Z;
					break;
				case Axis.Y:
					u = (int)Axis.X; v = (int)Axis.Y; w = (int)Axis.Z;
					break;
				case Axis.Z:
					u = (int)Axis.X; v = (int)Axis.Z;
					break;
			}

			// ρ, φ, z
			// The radial distance ρ is the Euclidean distance from the z axis to the point P.
			// The azimuth φ is the angle between the reference direction on the chosen plane and the line from the origin to the projection of P on the plane.
			// The height z is the signed distance from the chosen plane to the point P.

			float vMin = _geometry.Min((Axis)v), vMax = _geometry.Max((Axis)v);
			//float uMin = Mathf.Infinity, uMax = 0f; // Debugging
			float minAngle = -90f * Mathf.Deg2Rad, maxAngle = 270f * Mathf.Deg2Rad;

			// Loop through vertices to find their heights and azimuths (V and U respectively)
			for (int i = 0; i < _geometry.Vertices.Length; i++) {
				Vector3 vert = _geometry.Vertices[i];

				float height = vert[v].Remap(vMin, vMax, 0f, 1f);
				float radius = Mathf.Sqrt(vert[u] * vert[u] + vert[w] * vert[w]);
				float azimuth = Mathf.Atan2(vert[u], vert[w]);

				if (vert[u] == 0 && vert[w] == 0) {
					azimuth = 0;
				} else if (vert[u] >= 0) {
					azimuth = Mathf.Asin(vert[w] / radius);
				} else if (vert[u] < 0) {
					azimuth = -Mathf.Asin(vert[w] / radius) + Mathf.PI;
				}

				//if (azimuth < uMin) uMin = azimuth;
				//if (azimuth > uMax) uMax = azimuth;


				output.UV[i] = new Vector2(azimuth.Remap(minAngle, maxAngle, 0f, 1f), height);
			}

			//Debug.LogFormat("uMin:{0} uMax:{1}", uMin * Mathf.Rad2Deg, uMax * Mathf.Rad2Deg);

			List<Vector3> vertices = new List<Vector3>();
			vertices.AddRange(output.Vertices);

			List<Vector2> uvs = new List<Vector2>();
			uvs.AddRange(output.UV);

			List<Vector3> normals = new List<Vector3>();
			normals.AddRange(output.Normals);

			List<Vector4> tangents = new List<Vector4>();
			tangents.AddRange(output.Tangents);

			Dictionary<int, int> splits = new Dictionary<int, int>();

			// Split vertices at seams when the face in the UV mapping is laid out counter-clockwise
			for (int i = 0; i < _geometry.Triangles.Length; i += 3) {
				int i0 = _geometry.Triangles[i + 0];
				int i1 = _geometry.Triangles[i + 1];
				int i2 = _geometry.Triangles[i + 2];

				Vector2 uv0 = output.UV[i0];
				Vector2 uv1 = output.UV[i1];
				Vector2 uv2 = output.UV[i2];

				// http://stackoverflow.com/questions/1165647/how-to-determine-if-a-list-of-polygon-points-are-in-clockwise-order
				// https://en.wikipedia.org/wiki/Shoelace_formula
				float edge0 = (uv1.x - uv0.x) * (uv1.y + uv0.y);
				float edge1 = (uv2.x - uv1.x) * (uv2.y + uv1.y);
				float edge2 = (uv0.x - uv2.x) * (uv0.y + uv2.y);
				float areaTimesTwo = edge0 + edge1 + edge2;

				// If areaTimesTwo is negative, the triangle is laid out counter-clockwise
				if (areaTimesTwo < 0) {

					// We split the left-most vertices
					if (uv0.x < 0.5f) {
						int newIndex;
						if (!splits.TryGetValue(i0, out newIndex)) {
							newIndex = vertices.Count;
							vertices.Add(_geometry.Vertices[i0]);
							uvs.Add(output.UV[i0] + new Vector2(1f, 0f));
							normals.Add(_geometry.Normals[i0]);
							tangents.Add(_geometry.Tangents[i0]);
						}
						output.Triangles[i + 0] = newIndex;
					}

					if (uv1.x < 0.5f) {
						int newIndex;
						if (!splits.TryGetValue(i1, out newIndex)) {
							newIndex = vertices.Count;
							vertices.Add(_geometry.Vertices[i1]);
							uvs.Add(output.UV[i1] + new Vector2(1f, 0f));
							normals.Add(_geometry.Normals[i1]);
							tangents.Add(_geometry.Tangents[i1]);
						}
						output.Triangles[i + 1] = newIndex;
					}

					if (uv2.x < 0.5f) {
						int newIndex;
						if (!splits.TryGetValue(i2, out newIndex)) {
							newIndex = vertices.Count;
							vertices.Add(_geometry.Vertices[i2]);
							uvs.Add(output.UV[i2] + new Vector2(1f, 0f));
							normals.Add(_geometry.Normals[i2]);
							tangents.Add(_geometry.Tangents[i2]);
						}
						output.Triangles[i + 2] = newIndex;
					}

				}

			}

			output.Vertices = vertices.ToArray();
			output.Normals = normals.ToArray();
			output.UV = uvs.ToArray();
			output.Tangents = tangents.ToArray();

			return output;
		}

#if UNITY_EDITOR
		private static Color PreviewFillColor = new Color(0f, 1f, 1f, .2f);
		private static Color PreviewLineColor = new Color(0f, 1f, 1f, 1f);

		private float CalculateRadius(int w, Vector2 origin) {
			float radius = 0f;
			for (int i = 0; i < _geometry.Vertices.Length; i++) {
				Vector2 planarPoint = Geometry.PlanarCoordinates(w, _geometry.Vertices[i]);
				float distance = Vector2.Distance(origin, planarPoint);
				if (distance > radius) radius = distance;
			}
			return radius;
		}

		private void DrawCylinder(Vector3 origin, Axis u, Axis v, Axis w) {
			DrawCylinder(origin, (int)u, (int)v, (int)w);
		}

		private void DrawCylinder(Vector3 origin, int u, int v, int w) {

			// Bounds and axis centroid
			float wMin = _geometry.Min((Axis)w), wMax = _geometry.Max((Axis)w);

			// Points
			int segments = 60;
			int wrap = 0;
			float radius = CalculateRadius(w, _geometry.Centroid());
			Vector3[] front = new Vector3[segments];
			Vector3[] back = new Vector3[segments];
			for (int i = 0; i < segments; i++) {
				float angle = (360f * i) / segments;
				front[i] = back[i] = origin;
				front[i][u] = back[i][u] = origin[u] + Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
				front[i][v] = back[i][v] = origin[v] + Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
				front[i][w] = origin[w] + wMin;
				back[i][w] = origin[w] + wMax;
				if (angle == 270) wrap = i;
			}
			
			// Draw wireframe
			Handles.color = PreviewLineColor;

			Vector3[] fullFront = new Vector3[segments + 1];
			front.CopyTo(fullFront, 0);
			fullFront[segments] = fullFront[0];
			Handles.DrawAAPolyLine(fullFront);

			Vector3[] fullBack = new Vector3[segments + 1];
			back.CopyTo(fullBack, 0);
			fullBack[segments] = fullBack[0];
			Handles.DrawAAPolyLine(fullBack);

			Handles.DrawAAPolyLine(new Vector3[] { front[wrap], back[wrap] });

			// Draw surface
			Handles.color = PreviewFillColor;
			for (int i = 0; i < segments-1; i++) {
				Handles.DrawAAConvexPolygon(new Vector3[] { front[i], front[i+1], back[i+1], back[i] });
			}
			Handles.DrawAAConvexPolygon(new Vector3[] { front[segments-1], front[0], back[0], back[segments-1] });
		}

		public override void OnDrawGizmos(GameObject go) {
			MeshFilter meshFilter = go.GetComponent<MeshFilter>();
			Vector3 origin = go.transform.TransformPoint(meshFilter.sharedMesh.bounds.center);

			switch (Axis) {
				case Axis.X:
					DrawCylinder(origin, Axis.Y, Axis.Z, Axis.X);
					break;
				case Axis.Y:
					DrawCylinder(origin, Axis.X, Axis.Z, Axis.Y);
					break;
				case Axis.Z:
					DrawCylinder(origin, Axis.X, Axis.Y, Axis.Z);
					break;
			}
		}
#endif
	}

}
