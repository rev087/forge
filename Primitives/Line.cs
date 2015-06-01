using UnityEngine;

namespace Forge.Primitives {

	public class Line {

		public Vector3 Start = new Vector3(-0.5f, 0f, 0f);
		public Vector3 End = new Vector3(0.5f, 0f, 0f);
		public int Segments = 2;

		public Geometry Output() {

			if (Segments == 0) return Geometry.Empty;
			if (Segments == 1) return Point.At((Start + End) / 2);

			var geo = new Geometry();
			geo.Vertices = new Vector3[Segments];
			geo.Normals = new Vector3[0];
			geo.UV = new Vector2[0];
			geo.Triangles = new int[0];

			for (int i = 0; i < Segments; i++) {
				float f = (float)i / (Segments-1);
				geo.Vertices[i] = Vector3.Lerp(Start, End, f);
			}

			return geo;
		}

	}

}