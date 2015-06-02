using UnityEngine;
using System.Collections.Generic;

namespace Forge.Filters {

	public class Mirror {

		public Axis Axis = Axis.X;

		private Geometry _geometry;

		public Mirror(){}

		public Mirror(Geometry geometry) {
			Input(geometry);
		}

		public void Input(Geometry geometry) {
			_geometry = geometry.Copy();
		}

		public Geometry Output() {

			Geometry geo = _geometry.Copy();

			for (int i = 0; i < _geometry.Vertices.Length; i++) {
				Vector3 v = _geometry.Vertices[i];

				switch (Axis) {
					case Axis.X:
						geo.Vertices[i] = new Vector3(-v.x, v.y, v.z);
						break;
					case Axis.Y:
						geo.Vertices[i] = new Vector3(v.x, -v.y, v.z);
						break;
					case Axis.Z:
						geo.Vertices[i] = new Vector3(v.x, v.y, -v.z);
						break;
				}
			}
			
			System.Array.Reverse(geo.Vertices);
			System.Array.Reverse(geo.Normals);
			System.Array.Reverse(geo.Tangents);
			System.Array.Reverse(geo.UV);
			return geo;
		}

		public static Geometry Process(Geometry geometry) {
			Mirror mirror = new Mirror();
			mirror.Input(geometry);
			return mirror.Output();
		}

	} // class

} // namespace