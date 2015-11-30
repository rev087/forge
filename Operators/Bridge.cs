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

		// The bridge will be applied to identically sized polygons.
		// If the polygons are not of the same size, only the first nth vertices
		// in each polygon will be considered, where n is the vertex count of the
		// polygon with the least vertices.
		private static int GetBridgeLength(int[] polygons) {
			int minLength = 0;
			for (int i = 1; i < polygons.Length; i+=2) {
				if (polygons[i] < minLength || minLength == 0) {
					minLength = polygons[i];
				}
			}
			return minLength;
		}

		[Output]
		public Geometry Output() {
			if (_geometry.Polygons.Length == 0) return Geometry.Empty;

			// Validate the input geometry. All input vertices must be part
			// of a polygon.
			var lastIndex = 0;
			for (int i = 0; i < _geometry.Polygons.Length; i+=2) {
				if (lastIndex != 0 && _geometry.Polygons[i] != lastIndex) {
					OperatorError = "Invalid input: all vertices in input geometry must be part of a polygon";
					return Geometry.Empty;
				}
				lastIndex = _geometry.Polygons[i] + _geometry.Polygons[i + 1];
			}

			var polyLength = _geometry.Polygons.Length;
			var polyCount = polyLength / 2;
			var segments = GetBridgeLength(_geometry.Polygons);
			var vertexCount = segments * polyCount;
			var triCount = segments * (polyCount - 1) * 6;

			var geo = new Geometry(vertexCount, triCount, polyCount);

			int vCount = 0;
			int tCount = 0;

			for (var p = 0; p < polyLength; p+=2) {
				var start = _geometry.Polygons[p];
				
				if (_geometry.Polygons[p+1] != segments) {
					Debug.LogErrorFormat("Bridge error: input polygons have different numbers of vertices\nGot {0}, expected {1}", _geometry.Polygons[p+1], segments);
					return Geometry.Empty;
				}

				for (var v = start; v < start+segments; v++) {
					//Debug.Log(v);
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

						//Debug.LogFormat("tri: {0} {1} {2}", v, v-segments, (v - segments + 1 == start) ? start - segments : v - segments + 1, geo.Vertices.Length - 1);
						//Debug.LogFormat("tri: {0} {1} {2}", (v - 1 < start) ? start + segments - 1 : v - 1, v - segments, v);
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