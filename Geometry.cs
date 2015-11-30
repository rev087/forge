using UnityEngine;
using Forge.Extensions;
using System.Collections.Generic;

namespace Forge {

	public enum Surface {None, Triangulate, Converge};

	public struct Geometry {

		public Vector3[] Vertices;
		public Vector3[] Normals;
		public Vector4[] Tangents;
		public Vector2[] UV;
		public int[] Triangles;
		public int[] Polygons;

		public const float TAU = Mathf.PI * 2;

		public static Geometry Empty {
			get {
				return new Geometry() {
					Vertices = new Vector3[0],
					Normals = new Vector3[0],
					Tangents = new Vector4[0],
					UV = new Vector2[0],
					Triangles = new int[0],
					Polygons = new int[0]
				};
			}
		}

		public Geometry(int vertexCount, int faceCount=0, int polyCount=0) {
			Vertices = new Vector3[vertexCount];
			Normals = new Vector3[vertexCount];
			Tangents = new Vector4[vertexCount];
			UV = new Vector2[vertexCount];
			Triangles = new int[faceCount];
			Polygons = new int[polyCount];
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

			if (Tangents != null) {
				Vector4[] tangents = new Vector4[Tangents.Length];
				System.Array.Copy(Tangents, tangents, Tangents.Length);
				geometry.Tangents = tangents;
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

			if (Polygons != null) {
				int[] polygons = new int[Polygons.Length];
				System.Array.Copy(Polygons, polygons, Polygons.Length);
				geometry.Polygons = polygons;
			}

			return geometry;
		}

		public void RecalculateNormals() {
			Normals = new Vector3[Vertices.Length];

			for (int i = 0; i < Vertices.Length; i++) {
				Normals[i] = CalculateNormal(i);
			}
		}

		public void Offset(Vector3 offset) {
			if (offset.magnitude != 0f) {
				for (int i = 0; i < Vertices.Length; i++) {
					Vertices[i] = Vertices[i] + offset;
				}
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

			return (sum / normalSet.Count).normalized;
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

		public Vector3 AxisVariance() {
			if (Vertices.Length == 0) return Vector3.zero;
			Vector3 delta = Vector3.zero;
			Vector3 v0 = Vector3.zero;
			for (int i = 0; i < Vertices.Length; i++) {
				if (i > 0) {
					delta += (v0 - Vertices[i]).Abs();
				}
				v0 = Vertices[i];
			}
			return delta;
		}

		public static bool IsCoplanar(Vector3 axisVariance) {
			Vector3 v = axisVariance;
			return v.x == 0f || v.y == 0f || v.z == 0f;
		}
		public bool IsCoplanar() { return IsCoplanar(AxisVariance()); }

		public static int InvariantAxis(Vector3 axisVariance) {
			Vector3 v = axisVariance;
			if (v.x == 0f && v.y != 0f && v.z != 0f) return 0;
			if (v.x != 0f && v.y == 0f && v.z != 0f) return 1;
			if (v.x != 0f && v.y != 0f && v.z == 0f) return 2;
			return -1;
		}
		public int InvariantAxis() { return InvariantAxis(AxisVariance()); }

		public static Vector2 PlanarCoordinates(int invariantAxis, Vector3 point) {
			switch (invariantAxis) {
				case 0: return new Vector2(point.y, point.z);
				case 1: return new Vector2(point.x, point.z);
				case 2: return new Vector2(point.x, point.y);
				default: return Vector2.zero;
			}
		}
		public Vector2 PlanarCoordinates(Vector3 point) {
			return PlanarCoordinates(InvariantAxis(AxisVariance()), point);
		}

		// Minimum bounding box value in the axis
		public float Min(Axis axis) {
			float min = Mathf.Infinity;

			for (int i = 0; i < Vertices.Length; i++) {
				if (Vertices[i][(int)axis] < min) min = Vertices[i][(int)axis];
			}

			return min;
		}

		// Maximum bounding box value in the axis
		public float Max(Axis axis) {
			float max = -Mathf.Infinity;

			for (int i = 0; i < Vertices.Length; i++) {
				if (Vertices[i][(int)axis] > max) max = Vertices[i][(int)axis];
			}

			return max;
		}

		// Span of the bounding box in the axis
		public float Span(Axis axis) {
			return Max(axis) - Min(axis);
		}

		public override string ToString() {
			return System.String.Format("vert:{0}, nor:{1}, tan:{2} uv:{3}, tri:{4} poly:{5}",
				Vertices != null ? Vertices.Length.ToString() : "-",
				Normals != null ? Normals.Length.ToString() : "-",
				Tangents != null ? Tangents.Length.ToString() : "-",
				UV != null ? UV.Length.ToString() : "-",
				Triangles != null ? (Triangles.Length / 3).ToString() : "-",
				Polygons != null ? (Polygons.Length / 2).ToString() : "-"
			);
		}

	}

}