using UnityEngine;
using Forge.Extensions;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Forge.Operators {

	[OperatorMetadata(Category = "Geometry", Title = "Split At")]
	class SplitAt : Operator {

		private Geometry _geometry;
		private int[] _vertexIndices = new int[0];

		[Input] public string vertexList {
			get {
				string[] vertexTokens = new string[_vertexIndices.Length];
				for (int i = 0; i < vertexTokens.Length; i++) {
					vertexTokens[i] = _vertexIndices[i].ToString();
				}
				return string.Join(",", vertexTokens);
			}
			set {
				string[] vertexTokens = value.Split(',', ';');
				List<int> vertexIndices = new List<int>();

				for (int i = 0; i < vertexTokens.Length; i++) {
					int vertexIndex = -1;
					if (int.TryParse(vertexTokens[i].Trim(' '), out vertexIndex)) {
						vertexIndices.Add(vertexIndex);
					}
				}

				_vertexIndices = vertexIndices.ToArray();
			}
		}

		[Input] public void Input(Geometry geometry) {
			_geometry = geometry.Copy();
		}

		private int[] VertexIndices() {
			string[] vertexTokens = vertexList.Split(',', ';');
			List<int> vertexIndices = new List<int>();

			for (int i = 0; i < vertexTokens.Length; i++) {
				int vertexIndex = -1;
				if (int.TryParse(vertexTokens[i].Trim(' '), out vertexIndex)) {
					vertexIndices.Add(vertexIndex);
				}
			}

			return vertexIndices.ToArray();
		}

		[Output] public Geometry Output() {

			int[] indices = VertexIndices();

			List<Vector3> vertices = new List<Vector3>();
			vertices.AddRange(_geometry.Vertices);

			List<Vector3> normals = new List<Vector3>();
			normals.AddRange(_geometry.Normals);

			List<Vector4> tangents = new List<Vector4>();
			tangents.AddRange(_geometry.Tangents);

			List<Vector2> uvs= new List<Vector2>();
			uvs.AddRange(_geometry.UV);

			Geometry output = _geometry.Copy();

			for (int v = 0; v < _geometry.Triangles.Length; v+=3) {
				int p0 = _geometry.Triangles[v],
					p1 = _geometry.Triangles[v + 1],
					p2 = _geometry.Triangles[v + 2];

				if (indices.Contains(p0)) {
					vertices.Add(_geometry.Vertices[p0]);
					normals.Add(_geometry.Normals[p0]);
					tangents.Add(_geometry.Tangents[p0]);
					uvs.Add(_geometry.UV[p0]);
					output.Triangles[v] = vertices.Count - 1;
				}
				if (indices.Contains(p1)) {
					vertices.Add(_geometry.Vertices[p1]);
					normals.Add(_geometry.Normals[p1]);
					tangents.Add(_geometry.Tangents[p1]);
					uvs.Add(_geometry.UV[p1]);
					output.Triangles[v + 1] = vertices.Count - 1;
				}
				if (indices.Contains(p2)) {
					vertices.Add(_geometry.Vertices[p2]);
					normals.Add(_geometry.Normals[p2]);
					tangents.Add(_geometry.Tangents[p2]);
					uvs.Add(_geometry.UV[p2]);
					output.Triangles[v + 2] = vertices.Count - 1;
				}
			}

			if (vertices.Count > _geometry.Vertices.Length) {
				output.Vertices = vertices.ToArray();
				output.Normals = normals.ToArray();
				output.Tangents = tangents.ToArray();
				output.UV = uvs.ToArray();
			}

			return output;
		}

#if UNITY_EDITOR
		public override void OnDrawGizmos(GameObject go) {
			Vector3 camPos = SceneView.lastActiveSceneView.camera.transform.position;
			Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
			//Vector3[] vertices = new Vector3[_vertexIndices.Length];
			Handles.color = Color.red;
			for (int i = 0; i < _vertexIndices.Length; i++) {
				Vector3 vertex = mesh.vertices[_vertexIndices[i]];
				float camDist = Vector3.Distance(vertex, camPos);
				Handles.DotHandleCap(GUIUtility.GetControlID(FocusType.Passive), vertex, Quaternion.identity, camDist / 150, EventType.Ignore);
				//vertices[i] = vertex;
			}
			//Handles.DrawPolyLine(vertices);

		}
#endif

	}
}
