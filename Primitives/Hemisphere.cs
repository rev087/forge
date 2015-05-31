using UnityEngine;
using Forge.Filters;

namespace Forge.Primitives {

	public class Hemisphere {

		public float Radius = 0.5f;
		public Vector3 Center = Vector3.zero;
		public int Segments = 8;
		public OrientationPreset Orientation;

		public Geometry Output() {

			if (Segments < 3) {
				Debug.LogError("Hemisphere error: Hemispheres must have at least 3 segments");
			}

			Merge hemi = new Merge();

			Geometry prevLatitude = Geometry.Empty;

			Vector3 northCap = Vector3.zero;

			for (int i = 0; i < Segments; i++) {
				float angle = 90 + (90 * i / (Segments - 1));
				float sin = Mathf.Sin(angle * Mathf.Deg2Rad) * Radius;

				if (i == 0) {
					northCap = new Vector3(0f, sin, 0f);
				}

				if (i > 0) {
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

					if (i == 1) {
						var converge = new Converge(latitude);
						converge.RecalculateNormals = true;
						converge.Point = northCap;
						hemi.Input(converge.Output());
					} else {
						var bridge = new Bridge(latitude, prevLatitude);
						bridge.RecalculateNormals = false;
						hemi.Input(bridge.Output());
					}

					prevLatitude = latitude;
				}

			}

			return hemi.Output();
		}

	} // class

} // namespace