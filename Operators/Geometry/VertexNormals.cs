using UnityEngine;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Geometry", Title = "Vertex Normals")]
	class VertexNormals : Operator {

		[Input]
		public Geometry Input = Geometry.Empty;

		[Input]
		public Vector3 Normal = Vector3.zero;

		[Input]
		public Vector4 Tangent = Vector4.zero;

		[Output]
		public Geometry Output() {
			Geometry geo = Input.Copy();

			Vector3[] normals = new Vector3[geo.Vertices.Length];
			Vector4[] tangents = new Vector4[geo.Vertices.Length];

			for (int i = 0; i < geo.Vertices.Length; i++) {
				normals[i] = Normal;
				tangents[i] = Tangent;
			}

			geo.Normals = normals;
			geo.Tangents = tangents;

			return geo;
		}
	}
}
