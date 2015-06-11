using UnityEngine;
using Forge.Operators;

namespace Forge {

	public enum OrientationPreset {XY, XZ, ZY};
	public enum Axis {X=0, Y=1, Z=2};

	public static class OrientationUtil {

		private struct Orientation {
			public Axis Horizontal;
			public Axis Vertical;
			public Axis Normal;

			public static Orientation Preset(OrientationPreset preset) {
				switch (preset) {
					case OrientationPreset.XY:
						return new Orientation() { Horizontal = Axis.X, Vertical = Axis.Y, Normal = Axis.Z };
					case OrientationPreset.ZY:
						return new Orientation() { Horizontal = Axis.Z, Vertical = Axis.Y, Normal = Axis.X };
					default:
						return new Orientation() { Horizontal = Axis.X, Vertical = Axis.Z, Normal = Axis.Y };
				}
			}

			public override string ToString() {
				return System.String.Format("Horizontal:{0}, Vertical:{1}, Normal:{2}", Horizontal, Vertical, Normal);
			}
		}

		public static void ApplyOrientation(this Geometry geo, OrientationPreset preset) {

			if (preset == OrientationPreset.XZ) return;

			Orientation o = Orientation.Preset(preset);

			for (int i = 0; i < geo.Vertices.Length; i++) {

				Vector3 vertex = Vector3.zero;
				vertex[(int)o.Horizontal] = geo.Vertices[i].x;
				vertex[(int)o.Vertical] = geo.Vertices[i].z;
				vertex[(int)o.Normal] = geo.Vertices[i].y;
				geo.Vertices[i] = vertex;

				if (i < geo.Normals.Length) {
					Vector3 normal = Vector3.zero;
					normal[(int)o.Horizontal] = geo.Normals[i].x;
					normal[(int)o.Vertical] = geo.Normals[i].z;
					normal[(int)o.Normal] = geo.Normals[i].y;
					geo.Normals[i] = normal;
				}

			}

			if (o.Normal == Axis.Z) {
				for (int t = 0; t < geo.Triangles.Length; t += 3) {
					int a = geo.Triangles[t  ];
					int b = geo.Triangles[t+1];
					int c = geo.Triangles[t+2];
					geo.Triangles[t+2] = a;
					geo.Triangles[t+1] = b;
					geo.Triangles[t  ] = c;
				}
			}

		}
	}

} // namespace