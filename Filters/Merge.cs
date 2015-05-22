using UnityEngine;
using System.Collections.Generic;

namespace Forge.Filters {

	public class Merge {

		private List<Geometry> _geometries = null;

		private int VertexCount {
			get {
				if (_geometries == null) return 0;
				int count = 0;
				foreach (Geometry geo in _geometries) {
					count += geo.Vertices.Length;
				}
				return count;
			}
		}

		private int FaceCount {
			get {
				if (_geometries == null) return 0;
				int count = 0;
				foreach (Geometry geo in _geometries) {
					count += geo.Triangles.Length;
				}
				return count;
			}
		}

		public void Input(Geometry geometry) {
			if (_geometries == null) {
				_geometries = new List<Geometry>();
			}
			_geometries.Add(geometry.Copy());
		}

		public Geometry Output() {

			Geometry result = new Geometry();

			int vertexCount = VertexCount;
			result.Vertices = new Vector3 [vertexCount];
			result.Normals = new Vector3 [vertexCount];
			result.UV = new Vector2 [vertexCount];
			result.Triangles = new int [FaceCount];

			int vCount = 0;
			int tCount = 0;
			foreach (Geometry geo in _geometries) {

				// Vertices, Normals and UV
				for (int v = 0; v < geo.Vertices.Length; v++) {
					result.Vertices[vCount + v] = geo.Vertices[v];					
					result.Normals[vCount + v] = geo.Normals[v];					
					result.UV[vCount + v] = geo.UV[v];					
				}

				// Faces
				for (int f = 0; f < geo.Triangles.Length; f++) {
					result.Triangles[tCount + f] = geo.Triangles[f] + vCount;
				}

				tCount += geo.Triangles.Length;
				vCount += geo.Vertices.Length;
			}

			return result;
		}

		public static Geometry Process(params Geometry[] geometries) {
			Merge merge = new Merge();
			for (int i = 0; i < geometries.Length; i++) {
				merge.Input(geometries[i]);
			}
			return merge.Output();
		}

	}

}