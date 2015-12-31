using UnityEngine;
using Forge.Operators;

namespace Forge.Operators {

	public class Hemisphere {

		public float Radius = 0.5f;
		public Vector3 Center = Vector3.zero;
		public int Segments = 8;
		public OrientationPreset Orientation = OrientationPreset.XZ;

		public Geometry Output() {

			if (Segments < 3) {
				Debug.LogError("Hemisphere error: Spheres must have at least 3 segments");
			}

			Geometry geo = new Geometry(
				Segments*(Segments+1),
				(Segments*2) *(Segments-1)*3 -Segments*3, 
				Segments*2
			);

			float tau = Mathf.PI * 2;
			int triIndex = 0;

			// Longitudes (meridians)
			for (int i = 0; i < Segments; i++) {
				float lonAng = Mathf.PI/2 + (Mathf.PI / 2 * i / (Segments - 1));
				float lonSin = Mathf.Sin(lonAng) * Radius;
				float lonCos = Mathf.Cos(lonAng) * Radius;

				// Latitudes (parallels)
				int offset = i * (Segments + 1);

				// We iterate one additional time to create an overlapping longitude
				// at the UV seam
				for (int l = 0; l < Segments + 1; l++) {

					float latAng = tau * l / Segments;
					float latCos = Mathf.Cos(latAng) * lonCos;
					float latSin = Mathf.Sin(latAng) * lonCos;
					
					// Vertices, Normals, UV
					geo.Vertices[offset + Segments - l] = new Vector3(latCos, lonSin, latSin);
					geo.Normals[offset + Segments - l] = new Vector3(latCos, lonSin, latSin);
					geo.UV[offset + Segments - l] = new Vector2(latAng / tau, (lonSin+Radius) / (Radius*2));

					// Tangents
					geo.Tangents[offset + Segments - l] = new Vector4(-latSin, 0f, latCos, -1).normalized;
				}

				// Polygons
				var polyOrigin = geo.Polygons[i*2  ] = offset;
				var polyLength = geo.Polygons[i*2+1] = Segments+1;

				if (i > 0) {
					
					for (int t = 0; t < polyLength-1; t++) {

						// First Triangle
						geo.Triangles[triIndex++] = polyOrigin + t;
						geo.Triangles[triIndex++] = polyOrigin + t + 1;
						geo.Triangles[triIndex++] = polyOrigin + t - polyLength + 1;

						// Second Triangle
						if (i > 1) {
							geo.Triangles[triIndex++] = polyOrigin + t - polyLength + 1;
							geo.Triangles[triIndex++] = polyOrigin + t - polyLength;
							geo.Triangles[triIndex++] = polyOrigin + t;
						}
					}

				}
			}

			geo.Offset(Center);
			geo.ApplyOrientation(Orientation);

			return geo;
		}

	}

}