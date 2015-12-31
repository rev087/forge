using UnityEngine;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Math")]
	public class MultiplyInteger : Operator {

		[Input] public int A = 0;
		[Input] public int B = 0;

		[Output]
		public int Output() {
			return A * B;
		}

	}

	[OperatorMetadata(Category = "Math")]
	public class MultiplyFloat : Operator {

		[Input] public float A = 0.0f;
		[Input] public float B = 0.0f;

		[Output]
		public float Output() {
			return A * B;
		}

	}

}