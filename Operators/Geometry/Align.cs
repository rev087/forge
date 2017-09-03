using UnityEngine;

namespace Forge.Operators{

	[OperatorMetadata(Category = "Geometry")]
	public class Align : Operator {

		public enum AlignmentType { Min, Max, Center }

		[Input]
		public Geometry Input = Geometry.Empty;

		[Input]
		public Axis Axis = Axis.X;

		[Input]
		public AlignmentType AlignTo = AlignmentType.Min;

		[Input]
		public float Value = 0f;

		[Output]
		public Geometry Output() {
			var transf = new TransformGeometry();
			transf.Input(Input);
			transf.Position = Vector3.zero;

			switch (AlignTo) {
				case AlignmentType.Min:
					transf.Position[(int)Axis] = Value - Input.Min(Axis);
					break;
				case AlignmentType.Max:
					transf.Position[(int)Axis] = - (Input.Max(Axis) - Value);
					break;
				case AlignmentType.Center:
					transf.Position[(int)Axis] = Value - (Input.Min(Axis) + Input.Span(Axis) / 2f);
					break;
			}

			return transf.Output();
		}

	}

}
