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
		[HideInInspector] public Mesh GhostMesh = null;
		[HideInInspector] public Geometry Geometry = Geometry.Empty;
		[HideInInspector] public MeshDisplay MeshDisplay;

		private System.Diagnostics.Stopwatch Stopwatch = null;
		[HideInInspector] public double BuildMilliseconds = 0;

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

			// Reset the ghost mesh
			GhostMesh = null;

			// Statistics
			Stopwatch.Start();
			Geometry = Build();
			BuildMilliseconds = Stopwatch.Elapsed.TotalMilliseconds;
			Stopwatch.Reset();
			VertexCount = (Geometry.Vertices != null) ? Geometry.Vertices.Length : 0;
			TriangleCount = (Geometry.Triangles != null) ? Geometry.Triangles.Length / 3 : 0;

			// Mesh
			Mesh = (Mesh == null) ? new Mesh() : Mesh;
			Mesh.Clear();
			Mesh.vertices = Geometry.Vertices;
			Mesh.normals = Geometry.Normals;
			Mesh.triangles = Geometry.Triangles;
			Mesh.uv = Geometry.UV;

			GetComponent<MeshFilter>().sharedMesh = Mesh;

			IsBuilt = true;
		}

		public void Ghost(Geometry geo) {
			GhostMesh = new Mesh();
			GhostMesh.Clear();
			GhostMesh.vertices = geo.Vertices;
			GhostMesh.normals = geo.Normals;
			GhostMesh.triangles = geo.Triangles;
			GhostMesh.uv = geo.UV;
		}

		#if UNITY_EDITOR
		private MeshFilter _meshFilter;

		void OnDrawGizmosSelected() {
			if (MeshDisplay == null) MeshDisplay = (MeshDisplay) ScriptableObject.CreateInstance(typeof(MeshDisplay));

			MeshDisplay.DrawHandles(this, transform);
		}
		#endif

	}

}