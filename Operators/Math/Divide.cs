using UnityEngine;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Math", Title = "Divide Integer")]
	public class DivideInteger : Operator {

		[Input] public int A = 1;
		[Input] public int B = 1;

		[Output]
		public int Output() {

			if (B == 0) {
				OperatorError = "Cannot divide by zero!";
				return A;
			} else  OperatorError = null;

			return A / B;
		}

	}

	[OperatorMetadata(Category = "Math", Title = "Divide Float")]
	public class DivideFloat : Operator {

		[Input] public float A = 1.0f;
		[Input] public float B = 1.0f;

		[Output]
		public float Output() {

			if (B == 0f) {
				OperatorError = "Cannot divide by zero!";
				return A;
			} else OperatorError = null;

			return A / B;
		}

	}

}