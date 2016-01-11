using UnityEngine;

#if UNITY_EDITOR
using Forge.EditorUtils;
#endif

namespace Forge {

	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]

	[System.Serializable]
	public class ProceduralAsset : MonoBehaviour {
		
		public delegate void OnDrawGizmosHandler(GameObject go);
		public event OnDrawGizmosHandler OnDrawGizmos;

		public Template Template = null;
		[HideInInspector] public Mesh Mesh = null;

#if UNITY_EDITOR
		private MeshDisplay _meshDisplay = null;
		public MeshDisplay MeshDisplay {
			get {
				if (_meshDisplay == null) {
					_meshDisplay = (MeshDisplay) ScriptableObject.CreateInstance(typeof(MeshDisplay));
					_meshDisplay.hideFlags = HideFlags.HideAndDontSave;
				}
				return _meshDisplay;
			}
		}

		[HideInInspector] [System.NonSerialized] public Mesh GhostMesh = null;

#endif

		private System.Diagnostics.Stopwatch Stopwatch = null;
		[HideInInspector] public double LastBuildTime = 0;

		[HideInInspector] [System.NonSerialized] public Geometry Geometry = Geometry.Empty;
		[HideInInspector] [System.NonSerialized] public bool IsBuilt = false;

		public virtual Geometry Build() {
			return Geometry.Empty;
		}

		public void Generate() {
			if (Stopwatch == null) {
				Stopwatch = new System.Diagnostics.Stopwatch();
			}

#if UNITY_EDITOR
			// Reset the ghost mesh
			GhostMesh = null;
#endif

			// Statistics
#if UNITY_EDITOR
			Stopwatch.Start();
#endif

			// Build it
			if (Template != null)
				Geometry = Template.Build();
			else
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

#if UNITY_EDITOR
		public void Ghost(Geometry geo) {
			GhostMesh = new Mesh();
			GhostMesh.Clear();
			GhostMesh.vertices = geo.Vertices;
			GhostMesh.normals = geo.Normals;
			GhostMesh.tangents = geo.Tangents;
			GhostMesh.triangles = geo.Triangles;
			GhostMesh.uv = geo.UV;
		}

		void OnDrawGizmosSelected() {
			if (!IsBuilt) Generate();
			MeshDisplay.DrawHandles(this, transform);
			if (OnDrawGizmos != null) OnDrawGizmos(gameObject);
		}
#endif

	}

}