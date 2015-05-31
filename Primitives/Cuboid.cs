using UnityEngine;
using Forge.Filters;

namespace Forge.Primitives {

	public class Cuboid {

		public Vector3 Size = Vector3.one;
		public Vector3 Center = Vector3.zero;

		public Geometry Output() {
			// Top
			var top = new Square();
			top.Center = new Vector3(Center.x, Center.y + Size.y/2, Center.z);
			top.Size = new Vector2(Size.x, Size.z);
			top.Surface = Surface.Triangulate;

			// bottom
			var bottom = new Mirror(top.Output());
			bottom.Axis = Axis.Y;

			// Right wall
			var right = new Square();
			right.Orientation = OrientationPreset.ZY;
			right.Center = new Vector3(Center.x + Size.x/2, Center.y, Center.z);
			right.Size = new Vector2(Size.z, Size.y);
			right.Surface = Surface.Triangulate;

			// Left wall
			var left = new Mirror(right.Output());
			left.Axis = Axis.X;

			// Front wall
			var front = new Square();
			front.Orientation = OrientationPreset.XY;
			front.Center = new Vector3(Center.x, Center.y, Center.z + Size.z/2);
			front.Size = new Vector2(Size.x, Size.y);
			front.Surface = Surface.Triangulate;

			// Back wall
			var back = new Mirror(front.Output());
			back.Axis = Axis.Z;

			// Merge all sides
			Merge merge = new Merge();
			merge.Input(bottom.Output());
			merge.Input(right.Output());
			merge.Input(left.Output());
			merge.Input(front.Output());
			merge.Input(back.Output());
			merge.Input(top.Output());

			return merge.Output();
		}

	} // class

} // namespace