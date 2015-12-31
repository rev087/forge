using UnityEngine;
using System.Collections.Generic;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Geometry", Title = "Reverse Vertices")]
	public class Reverse : Operator {

		[Input] public Geometry Input = Geometry.Empty;

		[Output]
		public Geometry Output() {

			Geometry output = Input.Copy();

			// Vertices
			System.Array.Reverse(output.Vertices);
			System.Array.Reverse(output.UV);
			System.Array.Reverse(output.Normals);
			System.Array.Reverse(output.Tangents);

			return output;
		}

		public static Geometry Process(Geometry geometry) {
			Reverse reverse = new Reverse();
			reverse.Input = geometry;
			return reverse.Output();
		}

	}

}