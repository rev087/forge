using UnityEngine;
using System.Collections.Generic;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Geometry")]
	public class Mirror : Operator {

		[Input] public Geometry Input = Geometry.Empty;
		[Input] public Axis Axis = Axis.X;
		[Input] public bool Reverse = true;

		public Mirror(){}

		public Mirror(Geometry geometry) {
			Input = geometry;
		}

		public Geometry InputGeometry {
			get { return Input; }
			set { Input = ((Geometry)value).Copy(); }
		}

		[Output]
		public Geometry Output() {

			Geometry geo = Input.Copy();

			for (int i = 0; i < Input.Vertices.Length; i++) {
				Vector3 v = Input.Vertices[i];

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

				for (int p = 0; p < Input.Polygons.Length; p+=2) {
					int start = Input.Polygons[p];
					int length = Input.Polygons[p+1];

					geo.Polygons[p] = Input.Vertices.Length-(start+length);
					geo.Polygons[p+1] = length;
				}

				int total = Input.Vertices.Length - 1;
				for (int t = 0; t < Input.Triangles.Length; t+=3) {
					int a = Input.Triangles[t];
					int b = Input.Triangles[t+1];
					int c = Input.Triangles[t+2];

					geo.Triangles[t  ] = total-c;
					geo.Triangles[t+1] = total-b;
					geo.Triangles[t+2] = total-a;
				}
			}

			return geo;
		}

		public static Geometry Process(Geometry geometry) {
			var mirror = new Mirror(geometry);
			return mirror.Output();
		}

	} // class

} // namespace