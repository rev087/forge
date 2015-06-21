using UnityEngine;

namespace Forge.Operators {
	
	public class Bridge : Operator {

		[Input] public bool RecalculateNormals = false;

		private Geometry _geometry;

		public Bridge() {}

		public Bridge(Geometry geometry) {
			Input(geometry);
		}

		[Input]
		public void Input(Geometry geometry) {
			_geometry = geometry.Copy();
		}

		[Output]
		public Geometry Output() {

			if (_geometry.Polygons.Length == 0) return Geometry.Empty;

			var polyIndices = _geometry.Polygons.Length;
			var polyCount = polyIndices / 2;
			var segments = _geometry.Polygons[1];
			var vertexCount = segments * polyCount;
			var triCount = segments * (polyCount - 1) * 6;

			var geo = new Geometry(vertexCount, triCount, polyCount);

			int vCount = 0;
			int tCount = 0;

			for (var p = 0; p < polyIndices; p+=2) {
				var start = _geometry.Polygons[p];
				
				if (_geometry.Polygons[p+1] != segments) {
					Debug.LogErrorFormat("Bridge error: input polygons have different numbers of vertices\nGot {0}, expected {1}", _geometry.Polygons[p+1], segments);
					return Geometry.Empty;
				}

				for (var v = start; v < start+segments; v++) {
					geo.Vertices[vCount] = _geometry.Vertices[v];
					if (v < _geometry.Normals.Length) geo.Normals[vCount] = _geometry.Normals[v];
					if (v < _geometry.Tangents.Length) geo.Tangents[vCount] = _geometry.Tangents[v];
					if (v < _geometry.UV.Length) geo.UV[vCount] = _geometry.UV[v];
					vCount++;

					if (p > 0) {
						geo.Triangles[tCount++] = v;
						geo.Triangles[tCount++] = v-segments;
						geo.Triangles[tCount++] = (v-segments+1 == start) ? start-segments : v-segments+1;

						geo.Triangles[tCount++] = (v-1 < start) ? start+segments-1 : v-1;
						geo.Triangles[tCount++] = v-segments;
						geo.Triangles[tCount++] = v;
					}

				}
			}

			geo.Polygons = _geometry.Polygons;

			if (RecalculateNormals) {
				geo.RecalculateNormals();
			}

			return geo;
		}

	}

}