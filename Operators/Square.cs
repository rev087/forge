using UnityEngine;
using Forge.Operators;

namespace Forge.Operators {

	public class Square : Operator {

		[Input] public OrientationPreset Orientation = OrientationPreset.XZ;
		[Input] public Vector2 Size = Vector2.one;
		[Input] public Vector3 Center = Vector3.zero;
		[Input] public Surface Surface = Surface.None;

		[Output] public Geometry Output() {

			Geometry geo = new Geometry();

			Orientation ori = Forge.Orientation.FromPreset(Orientation);

			// Vertices
			geo.Vertices = new Vector3[] {
				ori.Vector3(- Size.x / 2, - Size.y / 2),
				ori.Vector3(- Size.x / 2, + Size.y / 2),
				ori.Vector3(+ Size.x / 2, + Size.y / 2),
				ori.Vector3(+ Size.x / 2, - Size.y / 2)
			};

			// Normals
			geo.Normals = new Vector3[] {
				ori.Vector3(0f, 0f, 1f),
				ori.Vector3(0f, 0f, 1f),
				ori.Vector3(0f, 0f, 1f),
				ori.Vector3(0f, 0f, 1f)
			};

			// Tangents
			Vector4 tangent = Vector4.zero;
			tangent[(int)ori.Horizontal] = 1f;
			tangent.w = -1f;
			geo.Tangents = new Vector4 [] {tangent, tangent, tangent, tangent};

			// Polygons
			geo.Polygons = new int[] {0, 4};

			// UV
			if (ori.Normal == Axis.Z) {
				geo.UV = new Vector2[] {
					new Vector2(1f, 0f),
					new Vector2(1f, 1f),
					new Vector2(0f, 1f),
					new Vector2(0f, 0f)
				};
			} else {
				geo.UV = new Vector2[] {
					new Vector2(0f, 0f),
					new Vector2(0f, 1f),
					new Vector2(1f, 1f),
					new Vector2(1f, 0f)
				};
			}

			// Surface
			switch (Surface) {
				case Surface.None:
					geo.Triangles = new int[0];
					break;
				case Surface.Triangulate:
					if (ori.Normal == Axis.Z) {
						geo.Triangles = new int[] {
							2, 1, 0,
							0, 3, 2
						};
					} else {
						geo.Triangles = new int[] {
							0, 1, 2,
							2, 3, 0
						};
					}
					break;
				case Surface.Converge:
					var conv = new Converge(geo);
					conv.Point = Center;
					conv.RecalculateNormals = true;
					geo = conv.Output();
					break;
			}
			
			geo.Offset(Center);

			return geo;
		}
		
	}

}