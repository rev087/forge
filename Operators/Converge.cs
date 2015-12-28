using UnityEngine;
using System.Collections.Generic;

namespace Forge.Operators {

	public class Converge : Operator {

		[Input] public Vector3 Point = Vector3.zero;
		[Input] public bool RecalculateNormals = false;

		private Geometry _geometry;

		public Converge() {}

		public Converge(Geometry geometry) {
			Input(geometry);
		}

		[Input] public void Input(Geometry geometry) {
			_geometry = geometry.Copy();
		}

		[Output] public Geometry Output() {
			int vertexCount = _geometry.Vertices.Length;

			Geometry geo = new Geometry(vertexCount+1);

			var triangles = new List<int>();

			// Add the converge point
			geo.Vertices[vertexCount] = Point;
			geo.UV[vertexCount] = new Vector2(.5f, .5f);

			Vector3 normalSum = Vector3.zero;

			for (int i = 0; i < vertexCount; i++) {
				geo.Vertices[i] = _geometry.Vertices[i];
				if (!RecalculateNormals && i < _geometry.Normals.Length) {
					geo.Normals[i] = _geometry.Normals[i];
					normalSum += geo.Normals[i];
				}
				geo.UV[i] = _geometry.UV[i];

				int a = (i == _geometry.Vertices.Length-1) ? 0 : i+1, b = vertexCount, c = i;

				var dot = Vector3.Dot(_geometry.Vertices[a], _geometry.Vertices[c]);
				if (dot == 1f || dot == -1) {
					continue;
				}

				triangles.Add(a);
				triangles.Add(b);
				triangles.Add(c);
			}

			geo.Normals[vertexCount] = (normalSum / vertexCount).normalized;

			geo.Triangles = triangles.ToArray();
			geo.Polygons = _geometry.Polygons;

			if (RecalculateNormals) {
				geo.RecalculateNormals();
			}

			return geo;
		}

		public Geometry Process(Geometry geometry) {
			Converge converge = new Converge();
			converge.Input(geometry);
			return converge.Output();
		}

	} // class

} // namespace