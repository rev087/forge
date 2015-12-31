using UnityEngine;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Values", Title = "Float")]
	public class FloatValue : Operator {

		[Input][Output]
		public float Float = 0.0f;

	}

}