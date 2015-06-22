using UnityEngine;
using Forge.Operators;

namespace Forge.Operators {

	public class Cuboid : Operator {

		[Input] public Vector3 Size = Vector3.one;
		[Input] public Vector3 Center = Vector3.zero;

		[Output]
		public Geometry Output() {
			// Top
			var top = new Square();
			top.Center = new Vector3(Center.x, Center.y + Size.y/2, Center.z);
			top.Size = new Vector2(Size.x, Size.z);
			top.Surface = Surface.Triangulate;

			// Bottom
			var bottom = new Manipulate(top.Output());
			bottom.Position = new Vector3(0f, -Size.y, 0f);

			// Right wall
			var right = new Square();
			right.Orientation = OrientationPreset.ZY;
			right.Center = new Vector3(Center.x + Size.x/2, Center.y, Center.z);
			right.Size = new Vector2(Size.z, Size.y);
			right.Surface = Surface.Triangulate;

			// Left wall
			var left = new Manipulate(right.Output());
			left.Position = new Vector3(-Size.x, 0f, 0f);

			// Front wall
			var front = new Square();
			front.Orientation = OrientationPreset.XY;
			front.Center = new Vector3(Center.x, Center.y, Center.z + Size.z/2);
			front.Size = new Vector2(Size.x, Size.y);
			front.Surface = Surface.Triangulate;

			// Back wall
			var back = new Manipulate(front.Output());
			back.Position = new Vector3(0f, 0f, -Size.z);

			// Merge all sides
			Merge merge = new Merge();
			merge.Input.Add(Reverse.Process(bottom.Output()));
			merge.Input.Add(right.Output());
			merge.Input.Add(Reverse.Process(left.Output()));
			merge.Input.Add(front.Output());
			merge.Input.Add(Reverse.Process(back.Output()));
			merge.Input.Add(top.Output());

			return merge.Output();
		}

	} // class

} // namespace