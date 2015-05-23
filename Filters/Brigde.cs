using UnityEngine;
using System.Collections.Generic;

namespace Forge.Filters {
	
	public class Bridge {

		private Geometry _a;
		private Geometry _b;

		public Bridge() {}

		public Bridge(Geometry a, Geometry b) {
			InputA(a);
			InputB(b);
		}

		public void InputA(Geometry geometry) {
			_a = geometry.Copy();
		}

		public void InputB(Geometry geometry) {
			_b = geometry.Copy();
		}

		public Geometry Output() {
		
			int vertexCount = Mathf.Max(_a.Vertices.Length, _b.Vertices.Length);

			Geometry geometry = new Geometry();
			geometry.Vertices = new Vector3[vertexCount * 2];
			geometry.Normals = new Vector3[vertexCount * 2];
			geometry.UV = new Vector2[vertexCount * 2];
			geometry.Triangles = new int[vertexCount * 6];

			// Vertices and triangles
			for (int i = 0; i < vertexCount; i++) {
				geometry.Vertices[i] = _a.Vertices[i];
				geometry.Vertices[vertexCount + i] = _b.Vertices[i];

				// First Triangle
				geometry.Triangles[i*6  ] = i;
				geometry.Triangles[i*6+1] = i + vertexCount;
				geometry.Triangles[i*6+2] = i + 1;

				// Second Triangle
				geometry.Triangles[i*6+3] = i;
				geometry.Triangles[i*6+4] = i + vertexCount - 1;
				geometry.Triangles[i*6+5] = i + vertexCount;
			}

			// Normals
			for (int i = 0; i < vertexCount; i++) {
				// Normal for a[i]
				geometry.Normals[i] = CalculateNormal(geometry, i);

				// Normal for b[i]
				geometry.Normals[i + vertexCount] = CalculateNormal(geometry, i + vertexCount);
			}

			return geometry;
		}

		private Vector3 CalculateNormal(Geometry geometry, int vertexIndex) {

			HashSet<Vector3> normalSet = new HashSet<Vector3>();

			int[] faces = FacesSharingPoint(geometry, vertexIndex);
			for (int i = 0; i < faces.Length; i++) {
				normalSet.Add(NormalForFace(geometry, faces[i]));
			}

			Vector3 sum = Vector3.zero;
			foreach (Vector3 normal in normalSet) {
				sum += normal;
			}

			return sum / normalSet.Count;
		}

		private int[] FacesSharingPoint(Geometry geometry, int vertexIndex) {
			List<int> faces = new List<int>();

			for (int i = 0; i < geometry.Triangles.Length; i++) {
				if (geometry.Triangles[i] == vertexIndex) {
					faces.Add(Mathf.FloorToInt(i / 3));
				}
			}

			return faces.ToArray();
		}

		private Vector3 NormalForFace(Geometry geometry, int faceIndex) {
			Vector3 a = geometry.Vertices[geometry.Triangles[faceIndex * 3]];
			Vector3 b = geometry.Vertices[geometry.Triangles[faceIndex * 3 + 1]];
			Vector3 c = geometry.Vertices[geometry.Triangles[faceIndex * 3 + 2]];
			Vector3 normal = Vector3.Cross(b-a, c-a).normalized;
			return normal;
		}

	} // class

} // namespace