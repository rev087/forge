using UnityEngine;
using System;
using System.Collections.Generic;

namespace Forge.Filters {

	public class Fuse {

		public float Threshold = 0f;

		private Geometry _geometry;

		public void Input(Geometry geometry) {
			_geometry = geometry.Copy();
		}

		public Geometry Output() {
			List<Vector3> vertices = new List<Vector3>();
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> uv = new List<Vector2>();

			for (int v = 0; v < _geometry.Vertices.Length; v++) {
				Vector3 vertex = _geometry.Vertices[v];

				int index = vertices.FindIndex(delegate(Vector3 existing) {
					if (Threshold <= 0)
						return existing.Equals(vertex);
					else
						return Vector3.Distance(existing, vertex) <= Threshold;
				});

				if (index < 0) {
					index = vertices.Count;
					vertices.Add(vertex);
					normals.Add(_geometry.Normals[v]);
					uv.Add(new Vector2(0f, 0f));
				} else {
					normals[index] = Vector3.Lerp(_geometry.Normals[v], normals[index], .5f).normalized;
				}

				for (int t = 0; t < _geometry.Triangles.Length; t++) {
					if (_geometry.Triangles[t] == v) {
						_geometry.Triangles[t] = index;
					}
				}
			}

			_geometry.Vertices = vertices.ToArray();
			_geometry.Normals = normals.ToArray();
			_geometry.UV = uv.ToArray();
			
			return _geometry;
		}

		public static Geometry Process(Geometry geometry) {
			Fuse fuse = new Fuse();
			fuse.Input(geometry);
			return fuse.Output();
		}

	} // class

} // namespace