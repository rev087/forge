using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using Forge.EditorUtils;
#endif

namespace Forge {

	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]

	public class ProceduralAsset : MonoBehaviour {

		[HideInInspector] public Mesh Mesh = null;
		[HideInInspector] public MeshDisplay MeshDisplay;

		private System.Diagnostics.Stopwatch Stopwatch = null;
		[HideInInspector] public long BuildMilliseconds = 0;

		[HideInInspector] public int VertexCount = 0;
		[HideInInspector] public int TriangleCount = 0;

		[HideInInspector] public bool IsBuilt = false;

		public virtual Geometry Build() {
			return new Geometry();
		}

		public void Generate() {
			if (Stopwatch == null) {
				Stopwatch = new System.Diagnostics.Stopwatch();
			}

			// Statistics
			Stopwatch.Start();
			Geometry geo = Build();
			BuildMilliseconds = Stopwatch.ElapsedMilliseconds;
			Stopwatch.Reset();
			VertexCount = (geo.Vertices != null) ? geo.Vertices.Length : 0;
			TriangleCount = (geo.Triangles != null) ? geo.Triangles.Length / 3 : 0;

			// Mesh
			Mesh = (Mesh == null) ? new Mesh() : Mesh;
			Mesh.Clear();
			Mesh.vertices = geo.Vertices;
			Mesh.normals = geo.Normals;
			Mesh.triangles = geo.Triangles;
			Mesh.uv = geo.UV;

			GetComponent<MeshFilter>().sharedMesh = Mesh;

			// Standard shader
			Shader shader = Shader.Find("Standard");
			shader.hideFlags = HideFlags.HideAndDontSave;

			IsBuilt = true;
		}

		public void Save(string filename) {

		}

		#if UNITY_EDITOR
		private MeshFilter _meshFilter;

		void OnDrawGizmosSelected() {
			if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
			if (MeshDisplay == null) MeshDisplay = (MeshDisplay) ScriptableObject.CreateInstance(typeof(MeshDisplay));

			MeshDisplay.DrawHandles(_meshFilter.sharedMesh, transform);
		}
		#endif

	}

}