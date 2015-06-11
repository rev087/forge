using UnityEngine;
using System.Collections.Generic;

namespace Forge.Filters {

	public class Polygonize {

		private List<Geometry> _geometries = new List<Geometry>();

		private int _totalVerts = 0;
		private int _totalTris = 0;

		public Polygonize() {}

		public Polygonize(params Geometry[] geometries) {
			foreach (Geometry geo in geometries) {
				Input(geo);
			}
		}

		public void Input(Geometry geometry) {
			_totalVerts += geometry.Vertices.Length;
			_totalTris += geometry.Triangles.Length;
			_geometries.Add(geometry.Copy());
		}

		public Geometry Output() {
			var result = new Geometry(_totalVerts, _totalTris);

			int vCount = 0;
			foreach (Geometry geo in _geometries) {

				for (int v = 0; v < geo.Vertices.Length; v++) {
					result.Vertices[vCount+v] = geo.Vertices[v];
				}

				vCount += geo.Vertices.Length;
			}

			result.Polygons = new int[] {0, vCount};

			return result;
		}

	}

}