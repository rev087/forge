using UnityEngine;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Primitives")]
	public class Line : Operator {

		[Input] public Vector3 Start = new Vector3(-0.5f, 0f, 0f);
		[Input] public Vector3 End = new Vector3(0.5f, 0f, 0f);
		[Input] public int Segments = 2;

		[Output] public Geometry Output() {

			if (Segments < 2) {
				OperatorError = "Segments cannot be less than 2";
				return Geometry.Empty;
			} else {
				OperatorError = null;
			}

			if (Segments == 0) return Geometry.Empty;
			if (Segments == 1) return Point.At((Start + End) / 2);

			var geo = new Geometry(Segments);
			geo.Polygons = new int[] {0, Segments};

			for (int i = 0; i < Segments; i++) {
				float f = (float)i / (Segments-1);
				geo.Vertices[i] = Vector3.Lerp(Start, End, f);
			}

			return geo;
		}

	}

}