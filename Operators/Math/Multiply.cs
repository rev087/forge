using UnityEngine;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Math", Title = "Multiply Integer")]
	public class MultiplyInteger : Operator {

		[Input] public int A = 1;
		[Input] public int B = 1;

		[Output]
		public int Output() {
			return A * B;
		}

	}

	[OperatorMetadata(Category = "Math", Title = "Multiply Float")]
	public class MultiplyFloat : Operator {

		[Input] public float A = 1.0f;
		[Input] public float B = 1.0f;

		[Output]
		public float Output() {
			return A * B;
		}

	}

}