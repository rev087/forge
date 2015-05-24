using UnityEngine;

namespace Forge.Filters {

	public class Manipulate {

		public Vector3 Position = Vector3.zero;
		public Vector3 Rotation = Vector3.zero;
		public Vector3 Scale = Vector3.one;

		private Geometry _geometry;

		public Manipulate() {}

		public Manipulate(Geometry geometry) {
			Input(geometry);
		}

		public void Input(Geometry geometry) {
			_geometry = geometry.Copy();
		}

		public Geometry Output() {

			if (Position != Vector3.zero) {
				for (int i = 0; i < _geometry.Vertices.Length; i++) {
					_geometry.Vertices[i] = _geometry.Vertices[i] + Position;
				}
			}

			if (Rotation != Vector3.zero) {
				Quaternion qRot = Quaternion.Euler(Rotation);
				for (int i = 0; i < _geometry.Vertices.Length; i++) {
					_geometry.Vertices[i] = qRot * _geometry.Vertices[i];
					_geometry.Normals[i] = qRot * _geometry.Normals[i];
				}
			}

			if (Scale != Vector3.one) {
				for (int i = 0; i < _geometry.Vertices.Length; i++) {
					_geometry.Vertices[i] = Vector3.Scale(_geometry.Vertices[i], Scale);
				}
			}

			return _geometry;
		}


	}

}