using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Forge.Operators {

	class PlanarProjection : Operator {
		private Geometry _geometry;

		[Input] public void Input(Geometry geometry) {
			_geometry = geometry.Copy();
		}
		[Input] public Axis Axis = Axis.X;

		[Output] public Geometry Output() {

			Geometry output = _geometry.Copy();

			int u = 0, v = 0;

			switch (Axis) {
				case Axis.X:
					u = (int)Axis.Z;
					v = (int)Axis.Y;
					break;
				case Axis.Y:
					u = (int)Axis.X;
					v = (int)Axis.Z;
					break;
				case Axis.Z:
					u = (int)Axis.X;
					v = (int)Axis.Y;
					break;
			}

			float uSpan = _geometry.Span((Axis) u), uHalf = uSpan / 2;
			float vSpan = _geometry.Span((Axis) v), vHalf = vSpan / 2;

			for (int i = 0; i < _geometry.Vertices.Length; i++) {
				Vector3 vert = _geometry.Vertices[i];
				output.UV[i] = new Vector2(
					(vert[u] + uHalf) / uSpan,
					(vert[v] + vHalf) / vSpan
					
				);
			}

			return output;
		}

#if UNITY_EDITOR
		private static Color PreviewFillColor = new Color(0f, 1f, 1f, .2f);
		private static Color PreviewLineColor = new Color(0f, 1f, 1f, 1f);

		private void DrawPlane(Vector3 origin, Axis u, Axis v, Axis w) {
			DrawPlane(origin, (int)u, (int)v, (int)w);
		}

		private void DrawPlane(Vector3 origin, int u, int v, int w) {

			// Bounds
			float uMin = _geometry.Min((Axis)u), uMax = _geometry.Max((Axis)u);
			float vMin = _geometry.Min((Axis)v), vMax = _geometry.Max((Axis)v);

			// Preview geometry points
			Vector3[] plane = new Vector3[4];
			for (int i = 0; i < 4; i++) {
				plane[i] = new Vector3(origin.x, origin.y, origin.z);
			}
			plane[0][u] += uMin; plane[0][v] += vMin;
			plane[1][u] += uMin; plane[1][v] += vMax;
			plane[2][u] += uMax; plane[2][v] += vMax;
			plane[3][u] += uMax; plane[3][v] += vMin;

			Vector3 front = Vector3.zero;
			front[w] += _geometry.Min((Axis)w);

			Vector3 back = Vector3.zero;
			back[w] += _geometry.Max((Axis)w);

			Handles.color = PreviewFillColor;
			Handles.DrawAAConvexPolygon(new Vector3[] { plane[0] + front, plane[1] + front, plane[2] + front, plane[3] + front, plane[0] + front });
			Handles.DrawAAConvexPolygon(new Vector3[] { plane[0] + back, plane[1] + back, plane[2] + back, plane[3] + back, plane[0] + back });

			Handles.color = PreviewLineColor;
			Handles.DrawAAPolyLine(new Vector3[] { plane[0] + front, plane[1] + front, plane[2] + front, plane[3] + front, plane[0] + front });
			Handles.DrawAAPolyLine(new Vector3[] { plane[0] + back, plane[1] + back, plane[2] + back, plane[3] + back, plane[0] + back });

			for (int i = 0; i < 4; i++) {
				Handles.DrawDottedLine(plane[i] + back, plane[i] + front, 2f);
			}
		}

		public override void OnDrawGizmos() {
			GameObject go = Selection.activeObject as GameObject;
			Vector3 pos = go.transform.position;

			switch (Axis) {
				case Axis.X:
					DrawPlane(pos, Axis.Y, Axis.Z, Axis.X);
					break;
				case Axis.Y:
					DrawPlane(pos, Axis.X, Axis.Z, Axis.Y);
					break;
				case Axis.Z:
					DrawPlane(pos, Axis.X, Axis.Y, Axis.Z);
					break;
			}
		}
	}
#endif

}
