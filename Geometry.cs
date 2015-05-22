using UnityEngine;

namespace Forge {

	public struct Geometry {

		public Vector3[] Vertices;
		public Vector3[] Normals;
		public Vector2[] UV;
		public int[] Triangles;

		public Geometry(Vector3[] verts, Vector3[] normals, Vector2[] uv, int[] tris) {
			Vertices = verts;
			Normals = normals;
			UV = uv;
			Triangles = tris;
		}

	}

}