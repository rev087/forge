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

			Geometry outputGeo = new Geometry(totalVerts, totalTris, totalPolys);

			int vCount = 0;
			int tCount = 0;
			int pCount = 0;
			foreach (Geometry inputGeo in Input) {

				// Vertices, Normals and UV
				for (int v = 0; v < inputGeo.Vertices.Length; v++) {

					outputGeo.Vertices[vCount + v] = inputGeo.Vertices[v];

					if (v < inputGeo.Normals.Length) {
						outputGeo.Normals[vCount + v] = inputGeo.Normals[v];
					}

					if (v < inputGeo.Tangents.Length) {
						outputGeo.Tangents[vCount + v] = inputGeo.Tangents[v];
					}

					if (v < inputGeo.UV.Length) {
						outputGeo.UV[vCount + v] = inputGeo.UV[v];
					}
				}

				// Faces
				for (int f = 0; f < inputGeo.Triangles.Length; f++) {
					outputGeo.Triangles[tCount + f] = inputGeo.Triangles[f] + vCount;
				}

				// Polygons
				for (int p = 0; p < inputGeo.Polygons.Length; p+=2) {
					outputGeo.Polygons[pCount+p] = inputGeo.Polygons[p] + vCount;
					outputGeo.Polygons[pCount+p+1] = inputGeo.Polygons[p+1];
				}

				vCount += inputGeo.Vertices.Length;
				tCount += inputGeo.Triangles.Length;
				pCount += inputGeo.Polygons.Length;
			}

			return outputGeo;
		}

		public static Geometry Process(params Geometry[] geometries) {
			Merge merge = new Merge(geometries);
			return merge.Output();
		}

	}

}