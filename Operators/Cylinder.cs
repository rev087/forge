using UnityEngine;
using Forge.Operators;

namespace Forge.Operators {

	public class Cylinder {

		public float Radius = 0.5f;
		public float Height = 1f;
		public int Segments = 8;
		public bool CapTop = true;
		public bool CapBottom = true;
		public Vector3 Center = Vector3.zero;
		public OrientationPreset Orientation = OrientationPreset.XZ;

		private Geometry _geometry;

		public Geometry Output() {

			var bottom = new Circle();
			bottom.Radius = Radius;
			bottom.Segments = Segments;
			bottom.Center = new Vector3(Center.x, Center.y - Height/2, Center.z);

			var top = new Circle();
			top.Radius = Radius;
			top.Segments = Segments;
			top.Center = new Vector3(Center.x, Center.y + Height/2, Center.z);

			var scaffold = new Merge(top.Output(), bottom.Output());

			var bridge = new Bridge(scaffold.Output());

			var cylinder = new Merge(bridge.Output());

			if (CapTop) {
				top.Surface = true;
				cylinder.Input(top.Output());
			}

			if (CapBottom) {
				bottom.Surface = true;
				cylinder.Input(Reverse.Process(bottom.Output()));
			}

			var geo = cylinder.Output();
			geo.ApplyOrientation(Orientation);
			return geo;
		}

	} // class

} // namespace