using UnityEngine;
using System.Collections.Generic;

namespace Forge.Operators {

	public class Merge : Operator {

		[Input]
		public List<Geometry> Input = new List<Geometry>();

		public Merge() {}

		public Merge(params Geometry[] geometries) {
			Input.AddRange(geometries);
		}

		[Output]
		public Geometry Output() {
			int totalVerts = 0, totalTris = 0, totalPolys = 0;
			foreach (var inputGeo in Input) {
				totalVerts += inputGeo.Vertices.Length;
				totalTris += inputGeo.Triangles.Length;
				totalPolys += inputGeo.Polygons.Length;
			}

			Geometry result = new Geometry(totalVerts, totalTris, totalPolys);

			int vCount = 0;
			int tCount = 0;
			int pCount = 0;
			foreach (Geometry geo in Input) {

				// Vertices, Normals and UV
				for (int v = 0; v < geo.Vertices.Length; v++) {

					result.Vertices[vCount + v] = geo.Vertices[v];

					if (v < geo.Normals.Length) {
						result.Normals[vCount + v] = geo.Normals[v];
					}

					if (v < geo.Tangents.Length) {
						result.Tangents[vCount + v] = geo.Tangents[v];
					}

					if (v < geo.UV.Length) {
						result.UV[vCount + v] = geo.UV[v];
					}
				}

				// Faces
				for (int f = 0; f < geo.Triangles.Length; f++) {
					result.Triangles[tCount + f] = geo.Triangles[f] + vCount;
				}

				// Polygons
				for (int p = 0; p < geo.Polygons.Length; p+=2) {
					result.Polygons[pCount+p] = geo.Polygons[p] + vCount;
					result.Polygons[pCount+p+1] = geo.Polygons[p+1];
				}

				vCount += geo.Vertices.Length;
				tCount += geo.Triangles.Length;
				pCount += geo.Polygons.Length;
			}

			return result;
		}

		public static Geometry Process(params Geometry[] geometries) {
			Merge merge = new Merge(geometries);
			return merge.Output();
		}

	}

}