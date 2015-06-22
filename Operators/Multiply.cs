using UnityEngine;

namespace Forge.Operators {

	public class Multiply : Operator {

		[Input] public float A = 0.0f;
		[Input] public float B = 0.0f;

		[Output]
		public float Output() {
			return A * B;
		}

	}

}