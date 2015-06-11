using UnityEngine;
using System.Collections.Generic;

namespace Forge.Operators {

	public class Mirror : Operator {

		public Axis Axis = Axis.X;
		public bool Reverse = true;

		private Geometry _geometry;

		public Mirror(){}

		public Mirror(Geometry geometry) {
			Input(geometry);
		}

		public void Input(Geometry geometry) {
			_geometry = geometry.Copy();
		}

		public Geometry InputGeometry {
			get { return _geometry; }
			set { _geometry = ((Geometry)value).Copy(); }
		}

		[Output]
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
			
			if (Reverse) {
				System.Array.Reverse(geo.Vertices);
				System.Array.Reverse(geo.Normals);
				System.Array.Reverse(geo.Tangents);
				System.Array.Reverse(geo.UV);

				for (int p = 0; p < _geometry.Polygons.Length; p+=2) {
					int start = _geometry.Polygons[p];
					int length = _geometry.Polygons[p+1];

					geo.Polygons[p] = _geometry.Vertices.Length-(start+length);
					geo.Polygons[p+1] = length;
				}

				int total = _geometry.Vertices.Length - 1;
				for (int t = 0; t < _geometry.Triangles.Length; t+=3) {
					int a = _geometry.Triangles[t];
					int b = _geometry.Triangles[t+1];
					int c = _geometry.Triangles[t+2];

					geo.Triangles[t  ] = total-c;
					geo.Triangles[t+1] = total-b;
					geo.Triangles[t+2] = total-a;
				}
			}

			return geo;
		}

		public static Geometry Process(Geometry geometry) {
			Mirror mirror = new Mirror();
			mirror.Input(geometry);
			return mirror.Output();
		}

	} // class

} // namespace