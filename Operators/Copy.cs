using UnityEngine;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Geometry")]
	public class Copy : Operator {

		[Input] public int Copies = 1;
		[Input] public Vector3 Position = Vector3.zero;
		[Input] public Vector3 Rotation = Vector3.zero;
		[Input] public Vector3 Scale = Vector3.one;

		private Geometry _geometry;

		public Copy() {}

		public Copy(Geometry geometry) {
			Input(geometry);
		}

		[Input] public void Input(Geometry geometry) {
			_geometry = geometry.Copy();
		}

		[Output]
		public Geometry Output() {

			Vector3 scaleDelta = Vector3.one - Scale;

			Vector3 scale = Scale;
			Vector3 position = Position;
			Vector3 rotation = Rotation;

			var merge = new Merge();

			for (int i = 0; i < Copies; i++) {
				var manipulate = new Manipulate(_geometry);
				if (i > 0) {
					manipulate.Rotation = rotation;
					manipulate.Position = position;
					manipulate.Scale = scale;
				}

				merge.Input.Add(manipulate.Output());

				if (i > 0) {
					position += Position;
					rotation += Rotation;
					scale += scaleDelta;
				}
			}

			return merge.Output();
		}

	}

}