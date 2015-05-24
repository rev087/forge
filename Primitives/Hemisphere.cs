using UnityEngine;
using Forge.Filters;

namespace Forge.Primitives {

	public class Hemisphere {

		public float Radius = 0.5f;
		public Vector3 Center = Vector3.zero;
		public int Segments = 8;
		public OrientationPlane OrientationPlane = OrientationPlane.XZ;

		public Geometry Output() {

			if (Segments < 4) return new Geometry();

			Merge hemisphere = new Merge();

			int rows = Segments / 2;

			Circle prev = null;

			for (int i = 0; i < rows; i++) {
				float angle = Mathf.PI / 2 * i / (rows-1);
				float cos = Mathf.Cos(angle) * Radius;
				float sin = Mathf.Sin(angle) * Radius;

				if (i < rows - 1) {
					Circle c = new Circle();
					c.Segments = Segments;
					c.Center = new Vector3(Center.x, Center.y + sin, Center.z);
					c.Radius = cos;

					if (i > 0) {
						Bridge bridge = new Bridge(prev.Output(), c.Output());
						hemisphere.Input(bridge.Output());
					}

					prev = c;
				} else {
					Converge converge = new Converge(prev.Output());
					converge.Point = new Vector3(Center.x, Center.y + sin, Center.z);
					hemisphere.Input(converge.Output());
				}

			}

			Geometry hGeo = hemisphere.Output();

			return Fuse.Process(hemisphere.Output());
		}

	} // class

} // namespace