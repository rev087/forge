using UnityEngine;
using Forge.Filters;

namespace Forge.Primitives {

	public class Hemisphere {

		public float Radius = 0.5f;
		public Vector3 Center = Vector3.zero;
		public int Segments = 8;
		public OrientationPreset Orientation = OrientationPreset.XZ;

		public Geometry Output() {

			if (Segments < 3) {
				Debug.LogError("Hemisphere error: Hemispheres must have at least 3 segments");
			}

			Merge hemi = new Merge();

			Geometry prevLatitude = Geometry.Empty;

			for (int i = 0; i < Segments; i++) {
				float lonAngle = 90 + (90 * i / (Segments - 1));
				float lonCos = Mathf.Cos(lonAngle * Mathf.Deg2Rad) * Radius;
				float lonSin = Mathf.Sin(lonAngle * Mathf.Deg2Rad) * Radius;

				// Latitudes
				Geometry latitude = new Geometry(Segments+1);

				// We iterate one additional time to create an overlapping longitude
				// at the UV seam
				for (int l = 0; l < Segments + 1; l++) {
					float latAng = Mathf.PI * 2 * l / Segments;
					float latCos = Mathf.Cos(latAng) * lonCos;
					float latSin = Mathf.Sin(latAng) * lonCos;
					latitude.Vertices[Segments - l] = new Vector3(latCos, lonSin, latSin);
					latitude.Normals[Segments - l] = new Vector3(latCos, lonSin, latSin).normalized;
					latitude.UV[Segments - l] = new Vector2(latAng / Mathf.PI, lonSin + Radius);

					float tanAng = latAng - Mathf.PI / 2;
					float tanCos = Mathf.Cos(tanAng) * lonCos;
					float tanSin = Mathf.Sin(tanAng) * lonCos;
					latitude.Tangents[Segments - l] = new Vector4(-tanCos, 0f, -tanSin, -1).normalized;
				}

				var bridge = new Bridge(latitude, prevLatitude);
				bridge.RecalculateNormals = false;
				hemi.Input(bridge.Output());

				prevLatitude = latitude;

			}

			Geometry geo = hemi.Output();

			geo.ApplyOrientation(Orientation);
			geo.Offset(Center);

			return geo;
		}

	} // class

} // namespace