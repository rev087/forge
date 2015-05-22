using UnityEngine;
using System;
using System.Collections.Generic;

namespace Forge.Filters {

	public class Fuse {

		public float Threshold = 0f;

		private Geometry _inputGeo;

		public void Input(Geometry geometry) {
			_inputGeo = geometry;
		}

		public Geometry Output() {
			List<Vector3> vertices = new List<Vector3>();
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> uv = new List<Vector2>();
			int[] triangles = new int[_inputGeo.Triangles.Length];
			System.Array.Copy(_inputGeo.Triangles, triangles, _inputGeo.Triangles.Length);

			for (int v = 0; v < _inputGeo.Vertices.Length; v++) {
				Vector3 vertex = _inputGeo.Vertices[v];

				int index = vertices.FindIndex(delegate(Vector3 existing) {
					if (Threshold <= 0)
						return existing.Equals(vertex);
					else
						return Vector3.Distance(existing, vertex) <= Threshold;
				});

				if (index < 0) {
					index = vertices.Count;
					vertices.Add(vertex);
					normals.Add(_inputGeo.Normals[v]);
					uv.Add(new Vector2(0f, 0f));
				} else {
					normals[index] = Vector3.Lerp(_inputGeo.Normals[v], normals[index], .5f).normalized;
				}

				for (int t = 0; t < triangles.Length; t++) {
					if (triangles[t] == v) {
						triangles[t] = index;
					}
				}
			}

			Geometry output = new Geometry() {
				Vertices = vertices.ToArray(),
				Normals = normals.ToArray(),
				UV = uv.ToArray(),
				Triangles = triangles
			};

			return output;
		}

		public static Geometry Process(Geometry geometry) {
			Fuse fuse = new Fuse();
			fuse.Input(geometry);
			return fuse.Output();
		}

	} // class

} // namespace