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
			Geometry geo = _geometry.Copy();

			if (Position != Vector3.zero) {
				for (int i = 0; i < geo.Vertices.Length; i++) {
					geo.Vertices[i] = geo.Vertices[i] + Position;
				}
			}

			if (Rotation != Vector3.zero) {
				Quaternion qRot = Quaternion.Euler(Rotation);
				for (int i = 0; i < geo.Vertices.Length; i++) {
					geo.Vertices[i] = qRot * geo.Vertices[i];
					if (geo.Normals != null && i < geo.Normals.Length) {
						geo.Normals[i] = qRot * geo.Normals[i];
					}
				}
			}

			if (Scale != Vector3.one) {
				for (int i = 0; i < geo.Vertices.Length; i++) {
					geo.Vertices[i] = Vector3.Scale(geo.Vertices[i], Scale);
				}
			}

			return geo;
		}

	}

}