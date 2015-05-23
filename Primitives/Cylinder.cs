using UnityEngine;
using Forge.Filters;

namespace Forge.Primitives {

	public class Cylinder {

		public float Radius = 0.5f;
		public float Height = 1f;
		public int Segments = 8;
		public bool CapTop = false;
		public bool CapBottom = false;
		public Vector3 Center = Vector3.zero;
		public OrientationPlane OrientationPlane = OrientationPlane.XZ;

		private Geometry _geometry;

		public Geometry Output() {

			Circle bottom = new Circle();
			bottom.Radius = Radius;
			bottom.Segments = Segments;
			bottom.OrientationPlane = OrientationPlane;

			Circle top = new Circle();
			top.Radius = Radius;
			top.Segments = Segments;
			top.OrientationPlane = OrientationPlane;

			switch (OrientationPlane) {
				case OrientationPlane.XY:
					bottom.Center = new Vector3(Center.x, Center.y, Center.z - Height/2);
					top.Center = new Vector3(Center.x, Center.y, Center.z + Height/2);
					break;
				case OrientationPlane.XZ:
					bottom.Center = new Vector3(Center.x, Center.y - Height/2, Center.z);
					top.Center = new Vector3(Center.x, Center.y + Height/2, Center.z);
					break;
				case OrientationPlane.YZ:
					bottom.Center = new Vector3(Center.x - Height/2, Center.y, Center.z);
					top.Center = new Vector3(Center.x + Height/2, Center.y, Center.z);
					break;
			}

			Bridge bridge = new Bridge(bottom.Output(), top.Output());

			Merge merge = new Merge();

			if (OrientationPlane != OrientationPlane.XZ) {
				merge.Input(Reverse.Process(bridge.Output()));
			} else {
				merge.Input(bridge.Output());
			}

			if (CapTop) {
				top.Filled = true;
				merge.Input(top.Output());
			}

			if (CapBottom) {
				bottom.Filled = true;
				merge.Input(Reverse.Process(bottom.Output()));
			}

			return merge.Output();
		}

	} // class

} // namespace