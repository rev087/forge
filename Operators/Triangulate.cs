using UnityEngine;
using System.Collections.Generic;
using Forge.Extensions;

namespace Forge.Operators {

	public class Triangulate : Operator {

		[Input] public bool RecomputeNormals = true;
		public int MaxIterations = -1; // Used to interactivelly visualize the algorithm
		public string Error = null;

		public Triangulate() {}

		public Triangulate(Geometry geometry) {
			Input(geometry);
		}

		private Geometry _geometry;

		[Input] public void Input(Geometry geometry) {
			_geometry = geometry.Copy();
		}

		[Output] public Geometry Output() {

			int vertexCount = _geometry.Vertices.Length;
			if (MaxIterations < 0) MaxIterations = vertexCount;

			Vector3 axisVariance = _geometry.AxisVariance();

			// 3D shape
			if (!Geometry.IsCoplanar(axisVariance)) {
				Error = "Triangulate.Output error: input vertices are not coplanar";
				return _geometry;
			}

			// 1D and 0D shapes
			var inv = Geometry.InvariantAxis(axisVariance);
			if (inv < 0) {
				Error = "Triangulate.Output error: input vertices are not a 2D polygon";
				return _geometry;
			}
			
			var candidates = new List<int>(_geometry.Vertices.Length.ToRange());
			var triangles = new List<int>();

			int iterations = 0;
			while (candidates.Count > 3) {

				iterations++;
				if (iterations > MaxIterations) break;
				bool earFound = false;

				for (int c = 0; c < candidates.Count; c++) {

					int iPrev = candidates[c == 0 ? candidates.Count-1 : c-1];
					int i     = candidates[c];
					int iNext = candidates[c < candidates.Count-1 ? c+1 : 0];

					// Convex
					if (!IsReflex(_geometry.Vertices, iPrev, i, iNext, inv)) {

						// Has no points inside
						if (!TriangleOverlapsVertices(_geometry.Vertices, iPrev, i, iNext, inv)) {
							earFound = true;
							candidates.RemoveAt(c);
							triangles.AddRange(new int[] {iPrev, i, iNext});
							break;
						}

					}

				} // for

				if (!earFound) {
					break;
				}
			}

			// Last 3 vertices
			if (candidates.Count == 3 && iterations < MaxIterations) {
				if (!IsReflex(_geometry.Vertices, candidates[0], candidates[1], candidates[2], inv)) {
					if (!TriangleOverlapsVertices(_geometry.Vertices, candidates[0], candidates[1], candidates[2], inv)) {
						triangles.AddRange(new int[] {candidates[0], candidates[1], candidates[2]});
					}
				}
			}

			_geometry.Triangles = triangles.ToArray();

			if (RecomputeNormals) {
				_geometry.RecalculateNormals();
			}

			return _geometry;
		} // Output

		private bool IsReflex(Vector3[] vertices, int i0, int i, int i1, int invariantAxis) {
			Vector3 v0 = vertices[i0];
			Vector3 v  = vertices[i];
			Vector3 v1 = vertices[i1];

			Vector3 cross = Vector3.Cross(v0-v, v1-v);
			return cross[invariantAxis] >= 0;
		}

		private bool TriangleOverlapsVertices(Vector3[] vertices, int a, int b, int c, int invariantAxis) {
			for (int f = 0; f < vertices.Length; f++) {

				if (f.In(a, b, c)) continue;

				Vector3 c1 = Vector3.Cross(vertices[b]-vertices[a], vertices[f]-vertices[a]);
				if (c1[invariantAxis] < 0) continue;

				Vector3 c2 = Vector3.Cross(vertices[c]-vertices[b], vertices[f]-vertices[b]);
				if (c2[invariantAxis] < 0) continue;

				Vector3 c3 = Vector3.Cross(vertices[a]-vertices[c], vertices[f]-vertices[c]);
				if (c3[invariantAxis] < 0) continue;

				return true;
			}
			return false;
		}

		public static Geometry Process(Geometry geometry) {
			var triangulate = new Triangulate(geometry);
			return triangulate.Output();
		}

	} // class

} // namespace