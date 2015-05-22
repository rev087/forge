using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Forge.Primitives {

	public class Square {

		public OrientationPlane OrientationPlane = OrientationPlane.XZ;
		public Vector2 Size = Vector2.one;
		public Vector3 Center = Vector3.zero;

		public Geometry Output() {

			Geometry geo = new Geometry();
			Vector3 normal = Vector3.zero;

			// Vertices and normals
			switch (OrientationPlane) {
				case OrientationPlane.XY:
					geo.Vertices = new Vector3 [] {
						new Vector3(Center.x + -Size.x / 2, Center.y + -Size.y / 2, Center.z),
						new Vector3(Center.x + -Size.x / 2, Center.y +  Size.y / 2, Center.z),
						new Vector3(Center.x +  Size.x / 2, Center.y +  Size.y / 2, Center.z),
						new Vector3(Center.x +  Size.x / 2, Center.y + -Size.y / 2, Center.z)
					};
					normal = new Vector3(0f, 0f, -1f);
					break;
				case OrientationPlane.XZ:
					geo.Vertices = new Vector3 [] {
						new Vector3(Center.x + -Size.x / 2, Center.y, Center.z + -Size.y / 2),
						new Vector3(Center.x + -Size.x / 2, Center.y, Center.z +  Size.y / 2),
						new Vector3(Center.x +  Size.x / 2, Center.y, Center.z +  Size.y / 2),
						new Vector3(Center.x +  Size.x / 2, Center.y, Center.z + -Size.y / 2)
					};
					normal = new Vector3(0f, 1f, 0f);
					break;
				case OrientationPlane.YZ:
					geo.Vertices = new Vector3 [] {
						new Vector3(Center.x, Center.y + -Size.x / 2, Center.z + -Size.y / 2),
						new Vector3(Center.x, Center.y +  Size.x / 2, Center.z + -Size.y / 2),
						new Vector3(Center.x, Center.y +  Size.x / 2, Center.z +  Size.y / 2),
						new Vector3(Center.x, Center.y + -Size.x / 2, Center.z +  Size.y / 2)
					};
					normal = new Vector3(1f, 0f, 0f);
					break;
			}

			geo.Normals = new Vector3 [] {normal, normal, normal, normal};

			// UV
			geo.UV = new Vector2 [] {
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0f)
			};

			// Triangles
			geo.Triangles = new int [] {
				0, 1, 2,
				2, 3, 0
			};

			return geo;
		}
		
	}

}