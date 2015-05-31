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

			for (int i = 0; i < _geometry.Vertices.Length; i++) {
				Vector3 v = _geometry.Vertices[i];

				switch (Axis) {
					case Axis.X:
						_geometry.Vertices[i] = new Vector3(-v.x, v.y, v.z);
						break;
					case Axis.Y:
						_geometry.Vertices[i] = new Vector3(v.x, -v.y, v.z);
						break;
					case Axis.Z:
						_geometry.Vertices[i] = new Vector3(v.x, v.y, -v.z);
						break;
				}
			}
			
			return Reverse.Process(_geometry);
		}

		public static Geometry Process(Geometry geometry) {
			Mirror mirror = new Mirror();
			mirror.Input(geometry);
			return mirror.Output();
		}

	} // class

} // namespace