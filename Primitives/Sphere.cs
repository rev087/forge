using UnityEngine;
using Forge.Filters;

namespace Forge.Primitives {

	public class Sphere {

		public float Radius = 0.5f;
		public Vector3 Center = Vector3.zero;
		public int Segments = 8;
		public OrientationPlane OrientationPlane = OrientationPlane.XZ;

		public Geometry Output() {
			Hemisphere hemisphere = new Hemisphere();
			hemisphere.Center = Center;
			hemisphere.Segments = Segments;
			hemisphere.OrientationPlane = OrientationPlane;
			var top = hemisphere.Output();

			Mirror bottom = new Mirror(top);
			bottom.Axis = Axis.Y;

			Merge sphere = new Merge(top, bottom.Output());

			return Fuse.Process(sphere.Output());
		}

	} // class

} // namespace