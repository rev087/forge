using UnityEngine;
using System.Collections.Generic;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Geometry")]
	public class FlipFaces : Operator {

		[Input] public Geometry Input = Geometry.Empty;

		[Input] public bool FlipNormals = true;

		[Output]
		public Geometry Output() {

			Geometry geo = Input.Copy();

			int[] revTriangles = new int[Input.Triangles.Length];
			for (int t = 0; t < Input.Triangles.Length; t += 3) {
				revTriangles[t+2] = Input.Triangles[t  ];
				revTriangles[t+1] = Input.Triangles[t+1];
				revTriangles[t  ] = Input.Triangles[t+2];
			}
			geo.Triangles = revTriangles;

			// Normals
			if (FlipNormals) {
				for (int i = 0; i < geo.Normals.Length; i++) {
					geo.Normals[i] *= -1;
				}
			}

			return geo;
		}

		public static Geometry Process(Geometry geometry) {
			FlipFaces reverse = new FlipFaces();
			reverse.Input = geometry;
			return reverse.Output();
		}

	}

}