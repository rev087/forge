using UnityEngine;
using System.Collections.Generic;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Geometry")]
	public class Polygonize : Operator {

		[Input] public List<Geometry> Input = new List<Geometry>();

		public Polygonize() {}

		public Polygonize(params Geometry[] geometries) {
			Input.AddRange(geometries);
		}

		[Output] public Geometry Output() {

			int totalVerts = 0;
			foreach (Geometry geo in Input) {
				totalVerts += geo.Vertices.Length;
			}

			var result = new Geometry(totalVerts);

			int vCount = 0;
			foreach (Geometry geo in Input) {
				for (int v = 0; v < geo.Vertices.Length; v++) {
					result.Vertices[vCount + v] = geo.Vertices[v];
					result.Normals[vCount + v] = geo.Normals[v];
					result.Tangents[vCount + v] = geo.Tangents[v];
				}
				vCount += geo.Vertices.Length;
			}

			if (vCount > 0) {
				result.Polygons = new int[] {0, totalVerts};
			}

			return result;
		}

	}

}