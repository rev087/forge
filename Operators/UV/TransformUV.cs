using UnityEngine;

namespace Forge.Operators {

	[OperatorMetadata(Category = "UV", Title = "Transform UV")]
	public class TransformUV : Operator {

		[Input]
		public Geometry Input = Geometry.Empty;
		[Input]
		public Vector2 Position = Vector2.zero;
		[Input]
		public Vector2 Rotation = Vector2.zero;
		[Input]
		public Vector2 Scale = Vector2.one;

		[Output]
		public Geometry Output() {
			Geometry geo = Input.Copy();

			if (Position != Vector2.zero) {
				for (int i = 0; i < geo.UV.Length; i++) {
					geo.UV[i] = geo.UV[i] + Position;
				}
			}

			if (Rotation != Vector2.zero) {
				Quaternion qRot = Quaternion.Euler(Rotation);
				for (int i = 0; i < geo.UV.Length; i++) {
					geo.UV[i] = qRot * geo.UV[i];
				}
			}

			if (Scale != Vector2.one) {
				for (int i = 0; i < geo.UV.Length; i++) {
					geo.UV[i] = Vector2.Scale(geo.UV[i], Scale);
				}
			}

			return geo;
		}

	}

}