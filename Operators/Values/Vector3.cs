using UnityEngine;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Values", Title = "XYZ to Vector3")]
	class XYZToVector3 : Operator {
		[Input] public float X = 0f;
		[Input] public float Y = 0f;
		[Input] public float Z = 0f;

		[Output]
		public Vector3 Vector3() {
			return new Vector3(X, Y, Z);
		}
	}

	[OperatorMetadata(Category = "Values", Title = "Vector3 to XYZ")]
	class Vector3ToXYZ : Operator {
		[Input] public Vector3 Vector = Vector3.zero;
		[Output] public float X { get { return Vector.x; } }
		[Output] public float Y { get { return Vector.y; } }
		[Output] public float Z { get { return Vector.z; } }
	}

}
