using UnityEngine;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Geometry", Title = "Recalculate Normals")]
	class RecalculateNormals : Operator {

		private Geometry _geometry = Geometry.Empty;
		[Input]
		public void Input (Geometry input) {
			_geometry = input;
		}

		[Output]
		public Geometry Output() {
			Geometry geo = _geometry.Copy();
			geo.RecalculateNormals();
			return geo;
		}
	}

}
