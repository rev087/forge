using UnityEngine;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Values", Title = "Bounding Box")]
	class Bounds : Operator {

		[Input]
		public Geometry Input = Geometry.Empty;

		[Input]
		public Axis Axis = Axis.X;

		[Output]
		public float Min { get { return Input.Min(Axis); } }

		[Output]
		public float Max { get { return Input.Max(Axis); } }

		[Output]
		public float Span { get { return Input.Span(Axis); } }
	}
}
