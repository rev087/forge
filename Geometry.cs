using UnityEngine;
using System.Collections.Generic;

namespace Forge {

	public enum OrientationPlane {XY, XZ, YZ};

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

			if (Vertices != null) {
				Vector3[] vertices = new Vector3[Vertices.Length];
				System.Array.Copy(Vertices, vertices, Vertices.Length);
				geometry.Vertices = vertices;
			}

			if (Normals != null) {
				Vector3[] normals = new Vector3[Normals.Length];
				System.Array.Copy(Normals, normals, Normals.Length);
				geometry.Normals = normals;
			}

			if (UV != null) {
				Vector2[] uv = new Vector2[UV.Length];
				System.Array.Copy(UV, uv, UV.Length);
				geometry.UV = uv;
			}

			if (Triangles != null) {
				int[] triangles = new int[Triangles.Length];
				System.Array.Copy(Triangles, triangles, Triangles.Length);
				geometry.Triangles = triangles;
			}

			return geometry;
		}

		public void CalculateNormals() {
			Normals = new Vector3[Vertices.Length];

			for (int i = 0; i < Vertices.Length; i++) {
				Normals[i] = CalculateNormal(i);
			}
		}

		public Vector3 CalculateNormal(int vertexIndex) {
			HashSet<Vector3> normalSet = new HashSet<Vector3>();

			int[] faces = FacesSharingVertex(vertexIndex);
			for (int i = 0; i < faces.Length; i++) {
				normalSet.Add(FaceNormal(faces[i]));
			}

			Vector3 sum = Vector3.zero;
			foreach (Vector3 normal in normalSet) {
				sum += normal;
			}

			return sum / normalSet.Count;
		}

		public int[] FacesSharingVertex(int vertexIndex) {
			if (Triangles == null) return new int[0];

			List<int> faces = new List<int>();

			for (int i = 0; i < Triangles.Length; i++) {
				if (Triangles[i] == vertexIndex) {
					faces.Add(Mathf.FloorToInt(i / 3));
				}
			}

			return faces.ToArray();
		}

		public Vector3 FaceNormal(int faceIndex) {
			Vector3 a = Vertices[Triangles[faceIndex * 3]];
			Vector3 b = Vertices[Triangles[faceIndex * 3 + 1]];
			Vector3 c = Vertices[Triangles[faceIndex * 3 + 2]];
			Vector3 normal = Vector3.Cross(b-a, c-a).normalized;
			return normal;
		}

		public override string ToString() {
			return System.String.Format("vert:{0}, nor:{1}, uv:{2}, tri:{3}",
				Vertices != null ? Vertices.Length.ToString() : "-",
				Normals != null ? Normals.Length.ToString() : "-",
				UV != null ? UV.Length.ToString() : "-",
				Triangles != null ? (Triangles.Length / 3).ToString() : "-"
			);
		}

	}

}