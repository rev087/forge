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
			return _geometry;
		}

#if UNITY_EDITOR
		private static Color PreviewFillColor = new Color(0f, 1f, 1f, .15f);
		private static Color PreviewLineColor = new Color(0f, 1f, 1f, 1f);

		private void DrawPlane(Vector3 origin, Axis u, Axis v, Axis w) {
			DrawPlane(origin, (int)u, (int)v, (int)w);
		}

		private void DrawPlane(Vector3 origin, int u, int v, int w) {

			// Bounds
			float uSpan = _geometry.Span((Axis)u), vSpan = _geometry.Span((Axis)v), wSpan = _geometry.Span((Axis)w);
			float uHalf = uSpan / 2, vHalf = vSpan / 2, wHalf = wSpan / 2;

			// Preview geometry points
			Vector3[] plane = new Vector3[4];
			for (int i = 0; i < 4; i++) {
				plane[i] = new Vector3(origin.x, origin.y, origin.z);
			}
			plane[0][u] -= uHalf; plane[0][v] -= vHalf;
			plane[1][u] -= uHalf; plane[1][v] += vHalf;
			plane[2][u] += uHalf; plane[2][v] += vHalf;
			plane[3][u] += uHalf; plane[3][v] -= vHalf;

			Vector3 front = Vector3.zero;
			front[w] -= wHalf;

			Vector3 back = Vector3.zero;
			back[w] += wHalf;

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
