using UnityEngine;
using System.Collections;
using Forge.Filters;

namespace Forge.Primitives {

	// Equilateral triangles
	public class Triangle {

		public OrientationPreset Orientation = OrientationPreset.XZ;
		public float Height = 1f;
		public Vector3 Center = Vector3.zero;
		public bool Surface = false;

		public float Side() {
			return Height / (Mathf.Sqrt(3f) / 2);
		}

		public Geometry Output() {

			Geometry geo = new Geometry(3);

			// http://upload.wikimedia.org/wikipedia/commons/9/9a/Degree-Radian_Conversion.svg
			float radius = 2 * Height / 3;
			geo.Vertices = new Vector3[3];
			for (int i = 0; i < 3; i++) {
				float degrees = 90 + 120 * i;
				float radians = Mathf.PI / 180 * degrees;
				float cos = Mathf.Cos(radians);
				float sin = Mathf.Sin(radians);
				geo.Vertices[2 - i] = new Vector3(cos * radius, 0f, sin * radius);
			}

			// Normals
			Vector3 normal = new Vector3(0f, 1f, 0f);
			geo.Normals = new Vector3 [] {normal, normal, normal};

			// UV
			geo.UV = new Vector2 [] {
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(0.5f, 1.0f)
			};

			// Triangles
			if (Surface) {
				geo.Triangles = new int [] {0, 1, 2};
			} else {
				geo.Triangles = new int[0];
			}

			// Polygons
			geo.Polygons = new int[] {0, 3};

			geo.ApplyOrientation(Orientation);
			geo.Offset(Center);

			// Orientation
			return geo;
		}
		
	}

}