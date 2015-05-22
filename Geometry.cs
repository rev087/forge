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

		public Geometry Copy() {
			Geometry geometry = new Geometry();

			Vector3[] vertices = new Vector3[Vertices.Length];
			System.Array.Copy(Vertices, vertices, Vertices.Length);
			geometry.Vertices = vertices;

			Vector3[] normals = new Vector3[Normals.Length];
			System.Array.Copy(Normals, normals, Normals.Length);
			geometry.Normals = normals;

			Vector2[] uv = new Vector2[UV.Length];
			System.Array.Copy(UV, uv, UV.Length);
			geometry.UV = uv;

			int[] triangles = new int[Triangles.Length];
			System.Array.Copy(Triangles, triangles, Triangles.Length);
			geometry.Triangles = triangles;

			return geometry;
		}

	}

}