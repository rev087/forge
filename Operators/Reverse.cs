using UnityEngine;
using System.Collections.Generic;

namespace Forge.Operators {

	public class Reverse : Operator {

		[Input] public Geometry Input = Geometry.Empty;

		public Reverse() {}

		public Reverse(Geometry geometry) {
			Input = geometry;
		}

		[Output]
		public Geometry Output() {

			Geometry output = Input.Copy();

			// Faces
			System.Array.Reverse(output.Vertices);

			// Normals
			for (int i = 0; i < output.Vertices.Length; i++) {
				output.Normals[i] *= - 1;
			}

			return output;
		}

		public static Geometry Process(Geometry geometry) {
			Reverse reverse = new Reverse();
			reverse.Input = geometry;
			return reverse.Output();
		}

	}

}