using UnityEngine;
using Forge.Filters;

namespace Forge.Primitives {

	public class Sphere {

		public float Radius = 0.5f;
		public Vector3 Center = Vector3.zero;
		public int Segments = 8;
		public OrientationPreset Orientation = OrientationPreset.XZ;

		public Geometry Output() {

			if (Segments < 3) {
				Debug.LogError("Sphere error: Spheres must have at least 3 segments");
			}

			Merge sphere = new Merge();

			Geometry prevLatitude = Geometry.Empty;

			// Longitudes
			for (int i = 0; i < Segments; i++) {
				float angle = 90 + (180 * i / (Segments - 1));
				float sin = Mathf.Sin(angle * Mathf.Deg2Rad) * Radius;
				float cos = Mathf.Cos(angle * Mathf.Deg2Rad) * Radius;

				// Latitudes
				Geometry latitude = new Geometry();
				latitude.Vertices = new Vector3[Segments + 1];
				latitude.Normals = new Vector3[Segments + 1];
				latitude.UV = new Vector2[Segments + 1];

				// We iterate one additional time to create an overlapping longitude
				// at the UV seam
				for (int l = 0; l < Segments + 1; l++) {
					float ng = Mathf.PI * 2 * l / Segments;
					float h = Mathf.Cos(ng) * cos;
					float v = Mathf.Sin(ng) * cos;
					latitude.Vertices[Segments - l] = new Vector3(h, sin, v);
					latitude.Normals[Segments - l] = new Vector3(h, sin, v).normalized;
					latitude.UV[Segments - l] = new Vector2(ng / (Mathf.PI * 2), sin + Radius);
				}

				var bridge = new Bridge(latitude, prevLatitude);
				bridge.RecalculateNormals = false;
				sphere.Input(bridge.Output());

				prevLatitude = latitude;

			}

			Geometry geo = sphere.Output();

			geo.ApplyOrientation(Orientation);
			geo.Offset(Center);

			return geo;
		}

	}

}