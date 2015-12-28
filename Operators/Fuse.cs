using UnityEngine;
using System.Collections.Generic;

namespace Forge.Operators {

	public class Fuse : Operator {

		[Input]
		public float Threshold = 0f;

		[Input]
		public bool RecalculateNormals = false;

		private Geometry _geometry;

		public Fuse(){}

		public Fuse(Geometry geometry) {
			Input(geometry);
		}

		[Input]
		public void Input(Geometry geometry) {
			_geometry = geometry.Copy();
		}

		[Output]
		public Geometry Output() {
			List<Vector3> vertices = new List<Vector3>();
			List<Vector4> tangents = new List<Vector4>();
			List<Vector2> uvs = new List<Vector2>();
			List<Vector3> normals = new List<Vector3>();

			Geometry output = _geometry.Copy();
			output.Polygons = new int[0];

			for (int v = 0; v < output.Vertices.Length; v++) {
				Vector3 vertex = output.Vertices[v];

				int index = vertices.FindIndex(delegate(Vector3 existing) {
					if (Threshold <= 0)
						return existing.Equals(vertex);
					else
						return Vector3.Distance(existing, vertex) <= Threshold;
				});

				if (index < 0) {
					index = vertices.Count;
					vertices.Add(vertex);
					tangents.Add(output.Tangents[v]);
					uvs.Add(output.UV[v]);
					normals.Add(_geometry.Normals[v]);
				}

				for (int t = 0; t < output.Triangles.Length; t++) {
					if (v != index && output.Triangles[t] == v) {
						output.Triangles[t] = index;
					}
				}
			}

			output.Vertices = vertices.ToArray();
			output.Tangents = tangents.ToArray();
			output.UV = uvs.ToArray();

			if (RecalculateNormals) {
				output.RecalculateNormals();
			} else {
				output.Normals = normals.ToArray();
			}
			
			return output;
		}

		public static Geometry Process(Geometry geometry) {
			Fuse fuse = new Fuse();
			fuse.Input(geometry);
			return fuse.Output();
		}

	} // class

} // namespace