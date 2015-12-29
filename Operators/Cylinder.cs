using UnityEngine;
using Forge.Operators;

namespace Forge.Operators {

	[OperatorMetadata(Category = "Primitives")]
	public class Cylinder : Operator {

		[Input] public float Radius = 0.5f;
		[Input] public float Height = 1f;
		[Input] public int Segments = 8;
		[Input] public bool CapTop = true;
		[Input] public bool CapBottom = true;
		[Input] public Vector3 Center = Vector3.zero;
		[Input] public OrientationPreset Orientation = OrientationPreset.XZ;

		private Geometry _geometry;

		[Output]
		public Geometry Output() {

			var bottom = new Circle();
			bottom.Radius = Radius;
			bottom.Segments = Segments;
			bottom.Center = new Vector3(Center.x, Center.y - Height/2, Center.z);

			var top = new Circle();
			top.Radius = Radius;
			top.Segments = Segments;
			top.Center = new Vector3(Center.x, Center.y + Height/2, Center.z);

			var scaffold = new Merge(bottom.Output(), top.Output());

			var bridge = new Bridge(scaffold.Output());

			var cylinder = new Merge(bridge.Output());

			if (CapTop) {
				top.Surface = true;
				cylinder.Input.Add(top.Output());
			}

			if (CapBottom) {
				bottom.Surface = true;
				cylinder.Input.Add(Reverse.Process(bottom.Output()));
			}

			var geo = cylinder.Output();
			geo.ApplyOrientation(Orientation);
			return geo;
		}

	} // class

} // namespace