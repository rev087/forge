using UnityEngine;

namespace Forge {

	public class Point {

		public Vector3 Position = Vector3.zero;

		public Point() {}

		public Point(float x, float y, float z) {
			Position = new Vector3(x, y, z);
		}

		public Point(Vector3 position) {
			Position = position;
		}

		public static Geometry At(Vector3 position) {
			var p = new Point();
			p.Position = position;
			return p.Output();
		}

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