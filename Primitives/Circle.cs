using UnityEngine;

namespace Forge.Primitives {

	public class Circle {

		public OrientationPlane OrientationPlane = OrientationPlane.XZ;
		public int Segments = 8;
		public bool Filled = false;
		public Vector3 Center = Vector3.zero;
		public float Radius = .5f;

		public Geometry Output() {
			int vertexCount = Filled ? Segments + 1 : Segments;
			int triCount = Filled ? Segments * 3 : 0;
			Geometry geo = new Geometry();
			geo.Vertices = new Vector3[vertexCount];
			geo.Normals = new Vector3[vertexCount];
			geo.UV = new Vector2[vertexCount];
			geo.Triangles = new int[triCount];

			float radiansInACircle = Mathf.PI * 2;

			for (int i = 0; i < Segments; i++) {
				float angle = radiansInACircle * i / Segments;
				float cos = Mathf.Cos(angle) * Radius;
				float sin = Mathf.Sin(angle) * Radius;

				switch (OrientationPlane) {
					case OrientationPlane.XZ:
						geo.Vertices[i] = new Vector3(Center.x + cos, Center.y, Center.z + sin);
						geo.Normals[i] = Vector3.up;
						break;
					case OrientationPlane.YZ:
						geo.Vertices[i] = new Vector3(Center.x, Center.y + cos, Center.z + sin);
						geo.Normals[i] = Vector3.right;
						break;
					case OrientationPlane.XY:
						geo.Vertices[i] = new Vector3(Center.x + cos, Center.y + sin, Center.z);
						geo.Normals[i] = Vector3.forward;
						break;
				}

				geo.UV[i] = new Vector2((i % 2 == 0) ? 0f : 1f, 0f);
				
				if (Filled && OrientationPlane == OrientationPlane.XZ) {
					geo.Triangles[i * 3] = i;
					geo.Triangles[i * 3 + 1] = Segments;
					geo.Triangles[i * 3 + 2] = (i == Segments - 1) ? 0 : i + 1;
				} else if (Filled) {
					geo.Triangles[i * 3] = i;
					geo.Triangles[i * 3 + 1] = (i == Segments - 1) ? 0 : i + 1;
					geo.Triangles[i * 3 + 2] = Segments;
				}
			}

			if (Filled) {
				geo.Vertices[Segments] = Center;
				geo.UV[Segments] = new Vector2(.5f, 1f);
				switch (OrientationPlane) {
					case OrientationPlane.XZ:
						geo.Normals[Segments] = Vector3.up; break;
					case OrientationPlane.YZ:
						geo.Normals[Segments] = Vector3.right; break;
					case OrientationPlane.XY:
						geo.Normals[Segments] = Vector3.forward; break;
				}
			}

			return geo;
		}

	} // class

} // namespace