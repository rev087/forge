using UnityEngine;
using System.Collections.Generic;

namespace Forge.Operators {

	public class ExtractVertices {

		public int[] Indexes = new int[0];
		public bool Invert = false;

		public Vector3 Point = Vector3.zero;

		private Geometry _geometry;

		public ExtractVertices() {}

		public ExtractVertices(Geometry geometry) {
			Input(geometry);
		}

		public void Input(Geometry geometry) {
			_geometry = geometry.Copy();
		}

		public Geometry Output() {

			Geometry geo = new Geometry();

			var vertices = new List<Vector3>();
			var normals = new List<Vector3>();
			var tangents = new List<Vector4>();
			var uv = new List<Vector2>();

			for (int i = 0; i < _geometry.Vertices.Length; i++) {

				if ((!Invert && System.Array.IndexOf(Indexes, i) >=  0) ||
				    ( Invert && System.Array.IndexOf(Indexes, i) == -1))
				{
					vertices.Add(_geometry.Vertices[i]);
					if (i < _geometry.Normals.Length) normals.Add(_geometry.Normals[i]);
					if (i < _geometry.Tangents.Length) tangents.Add(_geometry.Tangents[i]);
					if (i < _geometry.UV.Length) uv.Add(_geometry.UV[i]);
				}
			}

			geo.Vertices = vertices.ToArray();
			geo.Normals = normals.ToArray();
			geo.Tangents = tangents.ToArray();
			geo.UV = uv.ToArray();
			geo.Triangles = new int[0];
			geo.Polygons = new int[0];

			return geo;
		}

	} // class

} // namespace