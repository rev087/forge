using UnityEngine;
using System.Collections.Generic;

namespace Forge.Operators {

	public class Reverse {

		public bool FacesOnly = false;

		public Reverse() {}

		public Reverse(Geometry geometry) {
			Input(geometry);
		}

		private Geometry _geometry;

		public void Input(Geometry geometry) {
			_geometry = geometry.Copy();
		}

		public Geometry Output() {

			Geometry geo = _geometry.Copy();

			if (!FacesOnly) {			
				System.Array.Reverse(geo.Vertices);
			}
			else {
				int[] revTriangles = new int[_geometry.Triangles.Length];
				for (int t = 0; t < _geometry.Triangles.Length; t += 3) {
					revTriangles[t+2] = _geometry.Triangles[t  ];
					revTriangles[t+1] = _geometry.Triangles[t+1];
					revTriangles[t  ] = _geometry.Triangles[t+2];
				}
				_geometry.Triangles = revTriangles;
			}

			// Normals
			geo.RecalculateNormals();

			return geo;
		}

		public static Geometry Process(Geometry geometry) {
			Reverse reverse = new Reverse();
			reverse.Input(geometry);
			return reverse.Output();
		}

	}

}