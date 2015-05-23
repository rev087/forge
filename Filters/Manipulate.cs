using UnityEngine;

namespace Forge.Filters {

	public class Manipulate {

		private Geometry _geometry;

		public Manipulate() {}

		public Manipulate(Geometry geometry) {
			Input(geometry);
		}

		public void Input(Geometry geometry) {
			_geometry = geometry.Copy();
		}

		public Geometry Output() {
			return _geometry;
		}

		public void Rotate(Vector3 rot) {
			Quaternion qRot = Quaternion.Euler(rot);
			for (int i = 0; i < _geometry.Vertices.Length; i++) {
				_geometry.Vertices[i] = qRot * _geometry.Vertices[i];
				_geometry.Normals[i] = qRot * _geometry.Normals[i];
			}
		}

		public void Scale(Vector3 scale) {
			for (int i = 0; i < _geometry.Vertices.Length; i++) {
				_geometry.Vertices[i] = Vector3.Scale(_geometry.Vertices[i], scale);
			}
		}

		public void Move(Vector3 pos) {
			for (int i = 0; i < _geometry.Vertices.Length; i++) {
				_geometry.Vertices[i] = _geometry.Vertices[i] + pos;
			}
		}


	}

}