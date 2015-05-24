using UnityEngine;

namespace Forge.Primitives {

	public class Circle {

		public OrientationPlane OrientationPlane = OrientationPlane.XZ;
		public int Segments = 8;
		public bool Filled = false;
		public Vector3 Center = Vector3.zero;
		public float Radius = .5f;
		public float Angle = 360;

		public Geometry Output() {
			Segments = Segments + (Angle < 360 ? 1 : 0);
			int vertexCount = Segments + (Filled ? 1 : 0);

			Geometry geo = new Geometry();
			geo.Vertices = new Vector3[vertexCount];
			geo.UV = new Vector2[vertexCount];
			geo.Triangles = new int[Segments * 3 - (Angle < 360 ? 3 : 0)];

			float radians = Angle * Mathf.Deg2Rad;

			for (int i = 0; i < Segments; i++) {
				int offset = Angle < 360 ? 1 : 0;
				float angle = radians * i / (Segments - offset);
				float cos = Mathf.Cos(angle) * Radius;
				float sin = Mathf.Sin(angle) * Radius;

				switch (OrientationPlane) {
					case OrientationPlane.XZ:
						geo.Vertices[i] = new Vector3(Center.x + cos, Center.y, Center.z + sin);
						break;
					case OrientationPlane.YZ:
						geo.Vertices[i] = new Vector3(Center.x, Center.y + sin, Center.z + cos);
						break;
					case OrientationPlane.XY:
						geo.Vertices[i] = new Vector3(Center.x + cos, Center.y + sin, Center.z);
						break;
				}

				geo.UV[i] = new Vector2((i % 2 == 0) ? 0f : 1f, 0f);
				
			}

			if (Filled) {
				for (int i = 0; i < Segments - (Angle < 360 ? 1 : 0); i++) {
					if (OrientationPlane == OrientationPlane.XZ) {
						geo.Triangles[i*3  ] = i;
						geo.Triangles[i*3+1] = Segments;
						geo.Triangles[i*3+2] = (i == Segments - 1) ? 0 : i + 1;
					}
					else {
						geo.Triangles[i*3  ] = i;
						geo.Triangles[i*3+1] = (i == Segments - 1) ? 0 : i + 1;
						geo.Triangles[i*3+2] = Segments;
					}
				}

				geo.Vertices[Segments] = Center;
				geo.UV[Segments] = new Vector2(.5f, 1f);
				
			}

			geo.CalculateNormals();

			return geo;
		}

	} // class

} // namespace