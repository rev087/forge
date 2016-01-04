using UnityEngine;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Values", Title = "XY to Vector2")]
	class XYZToVector2 : Operator {
		[Input]
		public float X = 0f;
		[Input]
		public float Y = 0f;

		[Output]
		public Vector2 Vector2() {
			return new Vector2(X, Y);
		}
	}

	[OperatorMetadata(Category = "Values", Title = "Vector2 to XY")]
	class Vector2ToXYZ : Operator {
		[Input]
		public Vector2 Vector = Vector2.zero;
		[Output]
		public float X { get { return Vector.x; } }
		[Output]
		public float Y { get { return Vector.y; } }
	}

}
