using UnityEngine;
using Forge.Filters;

namespace Forge.Primitives {

	public class Circle {

		public enum OpeningType {Sector, Segment};

		public OpeningType Opening = OpeningType.Segment;
		public OrientationPreset Orientation = OrientationPreset.XZ;
		public bool Surface = false;
		public int Segments = 12;
		public Vector3 Center = Vector3.zero;
		public float Radius = .5f;
		public float StartAngle = 0;
		public float EndAngle = 360;

		public Geometry Output() {
			bool isOpen = Mathf.Abs(EndAngle - StartAngle) < 360;
			bool hasMidPoint = (Opening == OpeningType.Sector && isOpen) ||
				(Opening == OpeningType.Sector && Surface);

			int vertexCount = Segments;
			if (isOpen) vertexCount++;
			if (hasMidPoint) vertexCount++;

			Geometry geo = new Geometry(vertexCount);

			int arcVertices = Segments + (isOpen || hasMidPoint ? 1 : 0);

			// Vertices, Normals
			for (int i = 0; i < arcVertices; i++) {
				int seg = Segments + (hasMidPoint ? 0 : 0);
				float angle = StartAngle + ((EndAngle-StartAngle) * i / seg);
				float h = Mathf.Cos(angle * Mathf.Deg2Rad) * Radius;
				float v = Mathf.Sin(angle * Mathf.Deg2Rad) * Radius;
				geo.Vertices[arcVertices - i - 1] = new Vector3(h, 0f, v);
				geo.Normals[arcVertices - i - 1] = Vector3.up;
			}

			if (hasMidPoint) {
				geo.Vertices[vertexCount-1] = Vector3.zero;
				geo.Normals[vertexCount-1] = Vector3.up;
			}

			if (Surface) {
				var triangulate = new Triangulate(geo);
				geo = triangulate.Output();

				if (!isOpen && Opening == OpeningType.Sector) {
					System.Array.Resize<int>(ref geo.Triangles, geo.Triangles.Length + 3);
					geo.Triangles[geo.Triangles.Length-3] = 0;
					geo.Triangles[geo.Triangles.Length-2] = Segments;
					geo.Triangles[geo.Triangles.Length-1] = Segments-1;
				}
			}

			// Polygon
			if (hasMidPoint && !isOpen) {
				geo.Polygons = new int[] {0, vertexCount-1};
			} else {
				geo.Polygons = new int[] {0, vertexCount};
			}

			geo.ApplyOrientation(Orientation);
			geo.Offset(Center);

			return geo;

		} // Output

	} // class

} // namespace