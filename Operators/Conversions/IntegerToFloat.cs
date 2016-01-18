using UnityEngine;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Conversions", Title = "Integer to Float")]
	public class IntegerToFloat : Operator {

		[Input]
		public int Integer = 0;

		[Output]
		public float Float {
			get {
				return Integer;
			}
		}
	}

}
