using UnityEngine;
using System.Collections.Generic;

namespace Forge.Operators {

	public class Reverse : Operator {

		[Input] public Geometry Input = Geometry.Empty;
		[Input] bool RecomputeNormals = false;

		public Reverse() {}

		public Reverse(Geometry geometry) {
			Input = geometry;
		}

		[Output]
		public Geometry Output() {

			Geometry geo = Input.Copy();

			if (geo.Vertices != null) {
				System.Array.Reverse(geo.Vertices);
			}

			// Normals
			if (RecomputeNormals) {
				geo.RecalculateNormals();
			}

			return geo;
		}

		public static Geometry Process(Geometry geometry) {
			Reverse reverse = new Reverse();
			reverse.Input = geometry;
			return reverse.Output();
		}

	}

}