using UnityEngine;
using System.Collections.Generic;

namespace Forge.Operators {

	public class Merge {

		private List<Geometry> _geometries = new List<Geometry>();

		private int _totalVerts = 0;
		private int _totalTris = 0;
		private int _totalPolys = 0;

		public Merge() {}

		public Merge(params Geometry[] geometries) {
			foreach (Geometry geometry in geometries) {
				Input(geometry);
			}
		}

		public void Input(Geometry geometry) {
			_totalVerts += geometry.Vertices.Length;
			_totalTris += geometry.Triangles.Length;
			_totalPolys += geometry.Polygons.Length;
			_geometries.Add(geometry.Copy());
		}

		public Geometry Output() {

			Geometry result = new Geometry(_totalVerts, _totalTris, _totalPolys);

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
				for (int p = 0; p < geo.Polygons.Length; p+=2) {
					result.Polygons[pCount+p] = geo.Polygons[p] + vCount;
					result.Polygons[pCount+p+1] = geo.Polygons[p+1];
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

	}

}