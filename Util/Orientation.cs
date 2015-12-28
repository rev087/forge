using UnityEngine;
using Forge.Operators;

namespace Forge {

	public enum OrientationPreset {XY, XZ, ZY};
	public enum Axis {X=0, Y=1, Z=2};


	public struct Orientation {
		public Axis Horizontal;
		public Axis Vertical;
		public Axis Normal;

		public static Orientation FromPreset(OrientationPreset preset) {
			switch (preset) {
				case OrientationPreset.XY:
					return new Orientation() { Horizontal = Axis.X, Vertical = Axis.Y, Normal = Axis.Z };
				case OrientationPreset.ZY:
					return new Orientation() { Horizontal = Axis.Z, Vertical = Axis.Y, Normal = Axis.X };
				default:
					return new Orientation() { Horizontal = Axis.X, Vertical = Axis.Z, Normal = Axis.Y };
			}
		}

		public Vector3 Vector3(float horizontal, float vertical, float normal = 0f) {
			Vector3 v = new Vector3();
			v[(int)Horizontal] = horizontal;
			v[(int)Vertical] = vertical;
			v[(int)Normal] = normal;
			return v;
		}

		public override string ToString() {
			return System.String.Format("Horizontal:{0}, Vertical:{1}, Normal:{2}", Horizontal, Vertical, Normal);
		}
	}

	public static class OrientationUtil {

		public static void ApplyOrientation(this Geometry geo, OrientationPreset preset) {

			if (preset == OrientationPreset.XZ) return;

			Orientation o = Orientation.FromPreset(preset);

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

				if (o.Vertical != Axis.Y && i < geo.UV.Length) {
					geo.UV[i].x = 1 - geo.UV[i].x;
				}

			}

			if (o.Normal == Axis.Z) {
				for (int t = 0; t < geo.Triangles.Length; t += 3) {
					// b remains the same
					int a = geo.Triangles[t];
					int c = geo.Triangles[t + 2];
					geo.Triangles[t + 2] = a;
					geo.Triangles[t] = c;
				}
			}

		}
	}

} // namespace