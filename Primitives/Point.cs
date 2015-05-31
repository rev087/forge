using UnityEngine;

namespace Forge {

	public class Point {

		public Vector3 Position = Vector3.zero;

		public Geometry Output() {
			return new Geometry() {
				Vertices = new Vector3[] { Position },
				Normals = new Vector3[] { Vector3.zero },
				UV = new Vector2[0],
				Triangles = new int[0]
			};
		}

	} // class

} // namespace