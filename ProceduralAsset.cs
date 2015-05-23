using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using Forge.EditorUtils;
#endif

namespace Forge {

	public enum OrientationPlane {XY, XZ, YZ};

	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]

	public class ProceduralAsset : MonoBehaviour {

		[HideInInspector] public Mesh Mesh = null;

		[HideInInspector] public bool DisplayVertices = false;
		[HideInInspector] public bool DisplayVertexIndex = false;
		[HideInInspector] public bool DisplayVertexNormal = false;

		[HideInInspector] public bool DisplayFaces = false;
		[HideInInspector] public bool DisplayFaceIndex = false;
		[HideInInspector] public bool DisplayFaceNormal = false;

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

			// Material
			// if (GetComponent<MeshRenderer>().sharedMaterial == null) {
			// 	Material mat = new Material(Shader.Find("Standard"));
			// 	mat.hideFlags = HideFlags.HideAndDontSave;
			// 	GetComponent<MeshRenderer>().sharedMaterial = mat;
			// }

			IsBuilt = true;
		}

		public void Save(string filename) {

		}

		#if UNITY_EDITOR
		private MeshFilter _meshFilter;
		private MeshDisplay _meshDisplay;

		void OnDrawGizmosSelected() {
			if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
			if (_meshDisplay == null) _meshDisplay = new MeshDisplay();

			_meshDisplay.DisplayVertices = DisplayVertices;
			_meshDisplay.DisplayVertexIndex = DisplayVertexIndex;
			_meshDisplay.DisplayVertexNormal = DisplayVertexNormal;
			
			_meshDisplay.DisplayFaces = DisplayFaces;
			_meshDisplay.DisplayFaceIndex = DisplayFaceIndex;
			_meshDisplay.DisplayFaceNormal = DisplayFaceNormal;
			
			_meshDisplay.DrawHandles(_meshFilter.sharedMesh, transform);
		}
		#endif

	}

}