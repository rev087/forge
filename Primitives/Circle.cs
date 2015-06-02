using UnityEngine;
using Forge.Filters;

namespace Forge.Primitives {

	public class Circle {

		public OrientationPreset Orientation = OrientationPreset.XZ;
		public Surface Surface = Surface.None;
		public int Segments = 12;
		public Vector3 Center = Vector3.zero;
		public float Radius = .5f;
		public float StartAngle = 0;
		public float EndAngle = 360;

		public Geometry Output() {
			float angleDelta = Mathf.Abs(EndAngle - StartAngle);
			int vertexCount = Segments + (angleDelta == 360 ? 0 : 1);

			Geometry geo = new Geometry(vertexCount);

			for (int i = 0; i < vertexCount; i++) {
				int s = vertexCount - (angleDelta == 360 ? 0 : 1);
				float angle = StartAngle + ((EndAngle-StartAngle) * i / s);
				float h = Mathf.Cos(angle * Mathf.Deg2Rad) * Radius;
				float v = Mathf.Sin(angle * Mathf.Deg2Rad) * Radius;
				geo.Vertices[vertexCount - i - 1] = new Vector3(h, 0f, v);
				geo.Normals[vertexCount - i - 1] = new Vector3(h, 0f, v).normalized;
			}

			switch (Surface) {
				case Surface.None:
					geo.Triangles = new int[0];
					break; 
				case Surface.Triangulate:
					var triangulate = new Triangulate(geo);
					geo = triangulate.Output();
					break;
				case Surface.Converge:
					var converge = new Converge(geo);
					converge.Point = Vector3.zero;
					converge.RecalculateNormals = false;

					var sel = new ExtractFaces(converge.Output());
					sel.Indexes = new int[] {Segments};
					sel.Invert = true;
					sel.RecalculateNormals = false;
					geo = sel.Output();

					break;
			}

			geo.ApplyOrientation(Orientation);
			geo.Offset(Center);

			return geo;

		} // Output

	} // class

} // namespace