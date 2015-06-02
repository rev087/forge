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
		[HideInInspector] public double LastBuildTime = 0;

		[HideInInspector] [System.NonSerialized] public Mesh GhostMesh = null;
		[HideInInspector] [System.NonSerialized] public Geometry Geometry = Geometry.Empty;
		[HideInInspector] [System.NonSerialized] public bool IsBuilt = false;

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
			#if UNITY_EDITOR
			Stopwatch.Start();
			#endif

			// Build it
			Geometry = Build();

			// Statistics
			#if UNITY_EDITOR
			LastBuildTime = Stopwatch.Elapsed.TotalMilliseconds;
			Stopwatch.Reset();
			#endif

			// Mesh
			Mesh = (Mesh == null) ? new Mesh() : Mesh;
			Mesh.Clear();
			Mesh.vertices = Geometry.Vertices;
			Mesh.normals = Geometry.Normals;
			Mesh.tangents = Geometry.Tangents;
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
			GhostMesh.tangents = geo.Tangents;
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