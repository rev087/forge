using UnityEngine;

namespace Forge.Filters {

	public class Converge {

		public Vector3 Point = Vector3.zero;

		private Geometry _geometry;

		public Converge() {}

		public Converge(Geometry geometry) {
			Input(geometry);
		}

		public void Input(Geometry geometry) {
			_geometry = geometry.Copy();
		}

		public Geometry Output() {
			int vertexCount = _geometry.Vertices.Length;

			Geometry geo = new Geometry();
			geo.Vertices = new Vector3[vertexCount+1];
			geo.UV = new Vector2[vertexCount+1];
			geo.Triangles = new int[vertexCount*3];

			// Add the converge point
			geo.Vertices[vertexCount] = Point;
			geo.UV[vertexCount] = new Vector2(.5f, .5f);

			for (int i = 0; i < vertexCount; i++) {
				geo.Vertices[i] = _geometry.Vertices[i];
				geo.UV[i] = new Vector2((i % 2 == 0) ? 0f : 1f, 0f);

				geo.Triangles[i*3  ] = i;
				geo.Triangles[i*3+1] = vertexCount;
				geo.Triangles[i*3+2] = (i == _geometry.Vertices.Length-1) ? 0 : i+1;
			}

			geo.CalculateNormals();

			return geo;
		}

		public Geometry Process(Geometry geometry) {
			Converge converge = new Converge();
			converge.Input(geometry);
			return converge.Output();
		}

	} // class

} // namespace