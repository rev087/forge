using UnityEngine;

namespace Forge {

	[OperatorMetadata(Category = "Primitives")]
	public class Point : Operator {

		[Input] public Vector3 Position = Vector3.zero;

		public static Geometry At(Vector3 position) {
			var p = new Point();
			p.Position = position;
			return p.Output();
		}

		[Output] public Geometry Output() {
			return new Geometry() {
				Vertices = new Vector3[] { Position },
				Normals = new Vector3[] { Vector3.zero },
				Tangents = new Vector4[] { Vector4.zero },
				UV = new Vector2[0],
				Triangles = new int[0],
				Polygons = new int[0]
			};
		}

	} // class

} // namespace