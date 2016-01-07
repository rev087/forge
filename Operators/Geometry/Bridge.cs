using UnityEngine;

namespace Forge.Operators {
	
	[OperatorMetadata(Category = "Geometry")]
	public class Bridge : Operator {

		[Input] public bool RecalculateNormals = false;

		[Input]
		public bool ClosePolygons = false;

		[Input]
		public bool CloseLoop = false;

		private Geometry _geometry = Geometry.Empty;

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

			var polyCount = _geometry.Polygons.Length / 2;
			var polyLength = GetBridgeLength(_geometry.Polygons);
			var vertexCount = polyLength * polyCount;
			var triCount = (polyLength - 1) * (polyCount - 1) * 6;

			if (ClosePolygons) {
				triCount += (polyCount - 1) * 6;
			}

			if (CloseLoop) {
				triCount += (polyLength - 1) * 6;

				if (ClosePolygons) {
					triCount += 6;
				}
			}

			var geo = new Geometry(vertexCount, triCount, polyCount);

			int vCount = 0;
			int tCount = 0;

			for (var p = 0; p < _geometry.Polygons.Length; p+=2) {
				var start = _geometry.Polygons[p];
				
				if (_geometry.Polygons[p+1] != polyLength) {
					Debug.LogErrorFormat("Bridge error: input polygons have different numbers of vertices\nGot {0}, expected {1}", _geometry.Polygons[p+1], polyLength);
					return Geometry.Empty;
				}

				for (var v = start; v < start + polyLength; v++) {
					geo.Vertices[vCount] = _geometry.Vertices[v];
					geo.Normals[vCount] = _geometry.Normals[v];
					geo.Tangents[vCount] = _geometry.Tangents[v];
					geo.UV[vCount] = _geometry.UV[v];
					vCount++;

					// Skip the last vertex of each polygon if ClosePolygons is fale
					if (v == start + polyLength - 1 && !ClosePolygons) {
						continue;
					}

					// 2nd to nth polygons require no special cases
					if (p > 0) {
						// Lower-right triangle
						geo.Triangles[tCount++] = v;
						geo.Triangles[tCount++] = v - polyLength;
						geo.Triangles[tCount++] = (v - polyLength + 1 == start) ? start - polyLength : v - polyLength + 1;

						// Upper-left triangle
						geo.Triangles[tCount++] = (v - polyLength + 1 == start) ? start - polyLength : v - polyLength + 1;
						geo.Triangles[tCount++] = v + 1 < start + polyLength ? v + 1 : start;
						geo.Triangles[tCount++] = v;
					}

					// First polygon is skipped if CloseLoop is false
					else if (p == 0 && CloseLoop) {

						// The last vertex of the first polygon
						if (v == polyLength - 1 && ClosePolygons) {
							geo.Triangles[tCount++] = v;
							geo.Triangles[tCount++] = vertexCount - 1;
							geo.Triangles[tCount++] = vertexCount - polyLength;
							Debug.LogFormat("{0}: {1}, {2}, {3}", v, geo.Triangles[tCount - 3], geo.Triangles[tCount - 2], geo.Triangles[tCount - 1]);

							geo.Triangles[tCount++] = vertexCount - polyLength;
							geo.Triangles[tCount++] = 0;
							geo.Triangles[tCount++] = v;
							Debug.LogFormat("{0}: {1}, {2}, {3}", v, geo.Triangles[tCount - 3], geo.Triangles[tCount - 2], geo.Triangles[tCount - 1]);
						}

						// The first to second-to-last vertices of the first polygon
						else {
							geo.Triangles[tCount++] = v;
							geo.Triangles[tCount++] = polyCount * polyLength - polyLength + v;
							geo.Triangles[tCount++] = polyCount * polyLength - polyLength + v + 1;

							geo.Triangles[tCount++] = polyCount * polyLength - polyLength + v + 1;
							geo.Triangles[tCount++] = v + 1;
							geo.Triangles[tCount++] = v;
						}
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