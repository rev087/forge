using UnityEngine;
using Forge.Filters;

namespace Forge.Primitives {

	public class Sphere {

		public float Radius = 0.5f;
		public Vector3 Center = Vector3.zero;
		public int Segments = 8;
		public OrientationPreset Orientation;

		public Geometry Output() {

			if (Segments < 3) {
				Debug.LogError("Sphere error: Spheres must have at least 3 segments");
			}

			Merge sphere = new Merge();

			Geometry prevLatitude = Geometry.Empty;
			Vector3 northCap = Vector3.zero;

			// Longitudes
			for (int i = 0; i < Segments; i++) {
				float angle = 90 + (180 * i / (Segments - 1));
				float sin = Mathf.Sin(angle * Mathf.Deg2Rad) * Radius;

				// North-most point
				if (i == 0) {
					northCap = new Vector3(0f, sin, 0f);
				}

				if (i > 0 && i < Segments - 1) {
					float cos = Mathf.Cos(angle * Mathf.Deg2Rad) * Radius;

					// Latitudes
					Geometry latitude = new Geometry();
					latitude.Vertices = new Vector3[Segments];
					latitude.Normals = new Vector3[Segments];
					latitude.UV = new Vector2[Segments];

					for (int f = 0; f < Segments; f++) {
						float ng = Mathf.PI * 2 * f / Segments;
						float h = Mathf.Cos(ng) * cos;
						float v = Mathf.Sin(ng) * cos;
						latitude.Vertices[Segments - f - 1] = new Vector3(h, sin, v);
						latitude.Normals[Segments - f - 1] = new Vector3(h, sin, v).normalized;
					}

					// Converge north pole or bridge latitudes
					if (i == 1) {
						var converge = new Converge(latitude);
						converge.RecalculateNormals = false;
						converge.Point = northCap;
						sphere.Input(converge.Output());
					} else {
						var bridge = new Bridge(latitude, prevLatitude);
						bridge.RecalculateNormals = false;
						sphere.Input(bridge.Output());
					}

					prevLatitude = latitude;
				}

				// Converge south pole
				if (i == Segments - 1) {
					var converge = new Converge(prevLatitude);
					converge.RecalculateNormals = true;
					converge.Point = new Vector3(0f, sin, 0f);
					sphere.Input(Reverse.Process(converge.Output()));
				}

			}

			Geometry geo = sphere.Output();

			geo.ApplyOrientation(Orientation);
			geo.Offset(Center);

			return geo;
		}

	} // class

} // namespace