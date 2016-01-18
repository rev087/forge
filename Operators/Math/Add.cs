using UnityEngine;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Math", Title = "Add Integer")]
	public class AddInteger : Operator {

		[Input]
		public int A = 0;

		[Input]
		public int B = 0;

		[Output]
		public int Output() {
			return A + B;
		}

	}

	[OperatorMetadata(Category = "Math", Title = "Add Float")]
	public class AddFloat : Operator {

		[Input]
		public float A = 0.0f;

		[Input]
		public float B = 0.0f;

		[Output]
		public float Output() {
			return A + B;
		}

	}

}