using UnityEngine;
using System.Collections.Generic;

namespace Forge.Filters {

	public class Merge {

		private List<Geometry> _geometries = null;

		public Merge() {}

		public Merge(params Geometry[] geometries) {
			foreach (Geometry geometry in geometries) {
				Input(geometry);
			}
		}

		public void Input(Geometry geometry) {
			if (_geometries == null) {
				_geometries = new List<Geometry>();
			}
			_geometries.Add(geometry.Copy());
		}

		public Geometry Output() {

			Geometry result = new Geometry();

			int vertexCount = VertexLength();
			result.Vertices = new Vector3[vertexCount];
			result.Normals = new Vector3[vertexCount];
			result.Tangents = new Vector4[vertexCount];
			result.UV = new Vector2[vertexCount];
			result.Triangles = new int[FaceLength()];
			result.Polygons = new int[PolyLength()];

			int vCount = 0;
			int tCount = 0;
			int pCount = 0;
			foreach (Geometry geo in _geometries) {

				// Vertices, Normals and UV
				for (int v = 0; v < geo.Vertices.Length; v++) {

					result.Vertices[vCount + v] = geo.Vertices[v];

					if (v < geo.Normals.Length) {
						result.Normals[vCount + v] = geo.Normals[v];
					}

					if (v < geo.Tangents.Length) {
						result.Tangents[vCount + v] = geo.Tangents[v];
					}

					if (v < geo.UV.Length) {
						result.UV[vCount + v] = geo.UV[v];
					}
				}

				// Faces
				for (int f = 0; f < geo.Triangles.Length; f++) {
					result.Triangles[tCount + f] = geo.Triangles[f] + vCount;
				}

				// Polygons
				for (int p = 0; p < geo.Polygons.Length; p++) {
					result.Polygons[pCount + p] = geo.Polygons[p] + pCount;
				}

				vCount += geo.Vertices.Length;
				tCount += geo.Triangles.Length;
				pCount += geo.Polygons.Length;
			}

			return result;
		}

		public static Geometry Process(params Geometry[] geometries) {
			Merge merge = new Merge();
			for (int i = 0; i < geometries.Length; i++) {
				merge.Input(geometries[i]);
			}
			return merge.Output();
		}

		private int VertexLength() {
			if (_geometries == null) return 0;
			int count = 0;
			foreach (Geometry geo in _geometries) {
				count += geo.Vertices.Length;
			}
			return count;
		}

		private int FaceLength (){
			if (_geometries == null) return 0;
			int count = 0;
			foreach (Geometry geo in _geometries) {
				count += geo.Triangles.Length;
			}
			return count;
		}

		private int PolyLength() {
			if (_geometries == null) return 0;
			int count = 0;
			foreach (Geometry geo in _geometries) {
				count += geo.Polygons.Length;
			}
			return count;
		}

	}

}