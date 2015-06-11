using UnityEngine;

namespace Forge.Operators {

	public class FloatValue : Operator {

		public float FloatVal = 0.0f;

		[Output]
		public float Value() {
			return FloatVal;
		}

	}

}