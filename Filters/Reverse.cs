using UnityEngine;
using System;
using System.Collections.Generic;

namespace Forge.Filters {

	public class Reverse {

		private Geometry _geometry;

		public void Input(Geometry geometry) {
			_geometry = geometry;
		}

		public Geometry Output() {

			int[] revTriangles = new int[_geometry.Triangles.Length];
			for (int t = 0; t < _geometry.Triangles.Length; t += 3) {
				revTriangles[t+2] = _geometry.Triangles[t  ];
				revTriangles[t+1] = _geometry.Triangles[t+1];
				revTriangles[t  ] = _geometry.Triangles[t+2];
			}
			_geometry.Triangles = revTriangles;

			Vector3[] revNormals = new Vector3[_geometry.Normals.Length];
			for (int v = 0; v < _geometry.Normals.Length; v++) {
				revNormals[v] = _geometry.Normals[v] * -1;
			}
			_geometry.Normals = revNormals;

			return _geometry;
		}

		public static Geometry Process(Geometry geometry) {
			Reverse reverse = new Reverse();
			reverse.Input(geometry);
			return reverse.Output();
		}

	}

}