using UnityEngine;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Values")]
	public class IntegerValue : Operator {

		[Input][Output]
		public int Integer = 0;

	}

}