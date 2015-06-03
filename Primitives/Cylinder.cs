using UnityEngine;
using Forge.Filters;

namespace Forge.Primitives {

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

			Merge merge = new Merge();
			merge.Input(top.Output());
			merge.Input(Reverse.Process(bottom.Output()));
			
			Bridge bridge = new Bridge(merge.Output());


			merge.Input(bridge.Output());

			if (CapTop) {
				top.Surface = true;
				merge.Input(top.Output());
			}

			if (CapBottom) {
				bottom.Surface = true;
				merge.Input(Reverse.Process(bottom.Output()));
			}

			Geometry geo = merge.Output();

			geo.ApplyOrientation(Orientation);

			return geo;
		}

	} // class

} // namespace