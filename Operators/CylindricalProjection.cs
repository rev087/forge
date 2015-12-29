using UnityEngine;
using Forge.Extensions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Forge.Operators {

	[OperatorMetadata(Category = "UV")]
	class CylindricalProjection : Operator {
		private Geometry _geometry;

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
			float uMin = Mathf.Infinity, uMax = 0f; // Debugging
			float minAngle = -90f * Mathf.Deg2Rad, maxAngle = 270f * Mathf.Deg2Rad;

			float prevAngle = 0f;

			for (int i = 0; i < _geometry.Vertices.Length; i++) {
				Vector3 vert = _geometry.Vertices[i];
				float radius = Mathf.Sqrt(vert[u] * vert[u] + vert[w] * vert[w]);
				float azimuth = Mathf.Atan2(vert[u], vert[w]);
				float height = vert[v].Remap(vMin, vMax, 0f, 1f);

				if (vert[u] == 0 && vert[w] == 0) {
					azimuth = 0;
				} else if (vert[u] >= 0) {
					azimuth = Mathf.Asin(vert[w] / radius);
				} else if (vert[u] < 0) {
					azimuth = -Mathf.Asin(vert[w] / radius) + Mathf.PI;
				}

				if (azimuth == minAngle) azimuth = maxAngle;

				output.UV[i] = new Vector2(azimuth.Remap(minAngle, maxAngle, 0f, 1f), height);

				if (azimuth < uMin) uMin = azimuth;
				if (azimuth > uMax) uMax = azimuth;

				float delta = Mathf.Abs(prevAngle - azimuth);

				if (delta >= maxAngle) {
					Debug.LogFormat("Right seam at {0}, p:{1} c:{2}", i, prevAngle * Mathf.Rad2Deg, azimuth * Mathf.Rad2Deg);
				}
				else if (delta == 0) {
					Debug.LogFormat("Left seam at {0}, p:{1} c:{2}", i, prevAngle * Mathf.Rad2Deg, azimuth * Mathf.Rad2Deg);
				}

				Debug.LogFormat("[{0}] deg:{1} delta:{2}", i, (azimuth * Mathf.Rad2Deg), Mathf.Round(delta * Mathf.Rad2Deg));
				if (azimuth != prevAngle) prevAngle = azimuth;
			}
			Debug.LogFormat("aMin:{0} aMax:{1}", uMin, uMax);
			Debug.LogFormat("minDeg:{0}, maxDeg{1}", uMin * Mathf.Rad2Deg, uMax * Mathf.Rad2Deg);
			Debug.LogFormat("uMin:{0}, uMax:{1}", uMin.Remap(minAngle, maxAngle, 0f, 1f), uMax.Remap(minAngle, maxAngle, 0f, 1f));

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
	}
#endif

}
