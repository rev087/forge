using UnityEngine;
using System.Collections.Generic;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Logic")]
	public class Switch : Operator {

		[Input] public bool BooleanValue = false;

		[Input] public Geometry TrueGeo;
		[Input] public Geometry FalseGeo;

		public Switch() {}

		public Switch(Geometry trueGeo, Geometry falseGeo) {
			TrueGeo = trueGeo;
			FalseGeo = falseGeo;
		}

		[Output]
		public Geometry Output() {
			if (BooleanValue) {
				return TrueGeo;
			} else {
				return FalseGeo;
			}
		}

	}

}