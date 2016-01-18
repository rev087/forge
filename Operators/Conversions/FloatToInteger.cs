using UnityEngine;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Conversions", Title = "Float to Integer")]
	public class FloatToInteger : Operator {

		[Input]
		public float Float = 0f;

		[Output]
		public int Integer {
			get {
				return (int) Float;
			}
		}
	}

}
