using UnityEngine;
using System.Collections.Generic;

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

			var polyLength = GetBridgeLength(_geometry.Polygons);

			Geometry closeLoopPass;

			// CloseLoop is achieved by adding a new polygon of the appropriate length,
			// with its vertices at the same positions as the vertices in the first polygon
			// in the input geometry
			if (CloseLoop) {
				var newLength = _geometry.Vertices.Length + polyLength;
				Vector3[] vertices = new Vector3[newLength];
				Vector3[] normals = new Vector3[newLength];
				Vector2[] uvs = new Vector2[newLength];
				Vector4[] tangents = new Vector4[newLength];

				_geometry.Vertices.CopyTo(vertices, 0);
				_geometry.Normals.CopyTo(normals, 0);
				_geometry.UV.CopyTo(uvs, 0);
				_geometry.Tangents.CopyTo(tangents, 0);

				// Copy all the geometry data from the first polygon
				for (int i = _geometry.Vertices.Length; i < newLength; i++) {
					var f = i - _geometry.Vertices.Length;
					vertices[i] = _geometry.Vertices[f];
					normals[i] = _geometry.Normals[f];
					uvs[i] = _geometry.UV[f];
					tangents[i] = _geometry.Tangents[f];
				}

				// Create the new polygon
				int[] polygons = new int[_geometry.Polygons.Length + 2];
				_geometry.Polygons.CopyTo(polygons, 0);
				polygons[_geometry.Polygons.Length] = _geometry.Vertices.Length;
				polygons[_geometry.Polygons.Length + 1] = polyLength;

				// Assemble the geometry for this pass
				closeLoopPass = Geometry.Empty;
				closeLoopPass.Vertices = vertices;
				closeLoopPass.Normals = normals;
				closeLoopPass.UV = uvs;
				closeLoopPass.Tangents = tangents;
				closeLoopPass.Polygons = polygons;
			} else {
				closeLoopPass = _geometry;
			}

			var polyCount = closeLoopPass.Polygons.Length / 2;
			polyLength += ClosePolygons ? 1 : 0;
			var vertexCount = polyLength * polyCount;
			var triCount = (polyLength - 1) * (polyCount - 1) * 6;

			Geometry closePolygonsPass = Geometry.Empty;

			// ClosePolygons is achieved by adding a new vertex a the end of each polygon,
			// at the same position as the first vertex in that polygon
			if (ClosePolygons) {

				// Polygons need one additional vertex if CloseLoop is true
				closePolygonsPass.Polygons = (int[])closeLoopPass.Polygons.Clone();

				// Temporary lists
				List<Vector3> vertexList = new List<Vector3>();
				List<Vector3> normalList = new List<Vector3>();
				List<Vector2> uvList = new List<Vector2>();
				List<Vector4> tangentList = new List<Vector4>();

				for (int p = 0; p < closeLoopPass.Polygons.Length; p += 2) {
					int start = closeLoopPass.Polygons[p];
					int length = closeLoopPass.Polygons[p + 1];

					// Add one vertex to each polygon
					closePolygonsPass.Polygons[p] = p / 2 * polyLength;
					closePolygonsPass.Polygons[p + 1] = length + 1;

					// Add existing vertices
					for (int v = 0; v < length; v++) {
						vertexList.Add(closeLoopPass.Vertices[start + v]);
						normalList.Add(closeLoopPass.Normals[start + v]);
						uvList.Add(closeLoopPass.UV[start + v]);
						tangentList.Add(closeLoopPass.Tangents[start + v]);
					}

					// Add the additional vertex
					vertexList.Add(closeLoopPass.Vertices[start]);
					normalList.Add(closeLoopPass.Normals[start]);
					uvList.Add(closeLoopPass.UV[start]);
					tangentList.Add(closeLoopPass.Tangents[start]);
				}

				// Prepare the subject geometry
				closePolygonsPass.Vertices = vertexList.ToArray();
				closePolygonsPass.Normals = normalList.ToArray();
				closePolygonsPass.UV = uvList.ToArray();
				closePolygonsPass.Tangents = tangentList.ToArray();

			}
			else {
				closePolygonsPass = closeLoopPass.Copy();
			}

			// Build the surface
			var result = new Geometry(vertexCount, triCount);

			int vCount = 0;
			int tCount = 0;

			for (var pIndex = 0; pIndex < closePolygonsPass.Polygons.Length; pIndex+=2) {
				var start = closePolygonsPass.Polygons[pIndex];

				if (closePolygonsPass.Polygons[pIndex+1] != polyLength) {
					Debug.LogErrorFormat("Bridge error: input polygons have different numbers of vertices\nGot {0}, expected {1}", closePolygonsPass.Polygons[pIndex+1], polyLength);
					return Geometry.Empty;
				}

				for (var vIndex = start; vIndex < start + polyLength; vIndex++) {
					result.Vertices[vCount] = closePolygonsPass.Vertices[vIndex];
					result.Normals[vCount] = closePolygonsPass.Normals[vIndex];
					result.Tangents[vCount] = closePolygonsPass.Tangents[vIndex];

					// UV
					float u = (vIndex - start) / (polyLength - 1f); // U progresses along the vertices in the polygons
					float v = (pIndex / 2f) / (polyCount - 1f); // V progresses along the polygons
					result.UV[vCount] = new Vector2(u, v);

					vCount++;
					
					// Skip the first polygon and the last vertex of each polygon
					if (pIndex > 0 &&  vIndex != start + polyLength - 1) {
						// Lower-right triangle
						result.Triangles[tCount++] = vIndex;
						result.Triangles[tCount++] = vIndex - polyLength;
						result.Triangles[tCount++] = (vIndex - polyLength + 1 == start) ? start - polyLength : vIndex - polyLength + 1;

						// Upper-left triangle
						result.Triangles[tCount++] = (vIndex - polyLength + 1 == start) ? start - polyLength : vIndex - polyLength + 1;
						result.Triangles[tCount++] = vIndex + 1 < start + polyLength ? vIndex + 1 : start;
						result.Triangles[tCount++] = vIndex;
					}
				}
			}

			result.Polygons = closePolygonsPass.Polygons;

			if (RecalculateNormals) {
				result.RecalculateNormals();
			}

			return result;
		}

	}

}