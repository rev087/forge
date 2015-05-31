using UnityEngine;
using System.Collections.Generic;

namespace Forge.Filters {

	public class ExtractFaces {

		public int[] Indexes = new int[0];
		public bool Invert = false;
		public bool RecalculateNormals = true;

		public Vector3 Point = Vector3.zero;

		private Geometry _geometry;

		public ExtractFaces() {}

		public ExtractFaces(Geometry geometry) {
			Input(geometry);
		}

		public void Input(Geometry geometry) {
			_geometry = geometry.Copy();
		}

		public Geometry Output() {

			Geometry geo = new Geometry();

			var triangles = new List<int>();
			geo.Vertices = new Vector3[_geometry.Vertices.Length];
			geo.Normals = new Vector3[_geometry.Normals.Length];
			geo.UV = new Vector2[_geometry.UV.Length];

			for (int i = 0; i < _geometry.Vertices.Length; i++) {
				geo.Vertices[i] = _geometry.Vertices[i];
				if (i < _geometry.Normals.Length) geo.Normals[i] = _geometry.Normals[i];
				if (i < _geometry.UV.Length) geo.UV[i] = _geometry.UV[i];
			}

			for (int i = 0; i < _geometry.Triangles.Length; i+=3) {
				int t = i / 3;
				if ((!Invert && System.Array.IndexOf(Indexes, t) >=  0) ||
				    ( Invert && System.Array.IndexOf(Indexes, t) == -1))
					{
						triangles.Add(_geometry.Triangles[i  ]);
						triangles.Add(_geometry.Triangles[i+1]);
						triangles.Add(_geometry.Triangles[i+2]);
					}
			}
			geo.Triangles = triangles.ToArray();

			if (RecalculateNormals) {
				geo.RecalculateNormals();
			}

			return geo;
		}

	} // class

} // namespace