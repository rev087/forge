using UnityEngine;
using Forge.Filters;

namespace Forge.Primitives {

	public class Cuboid {

		public bool FuseVertices = false;
		public Vector3 Size = Vector3.one;
		public Vector3 Center = Vector3.zero;

		public Geometry Output() {
			// Bottom
			Square bottom = new Square();
			bottom.Center = new Vector3(Center.x, Center.y - Size.y/2, Center.z);
			bottom.Size = new Vector2(Size.x, Size.z);

			// Right wall
			Square right = new Square();
			right.OrientationPlane = OrientationPlane.YZ;
			right.Center = new Vector3(Center.x - Size.x/2, Center.y, Center.z);
			right.Size = new Vector2(Size.y, Size.z);

			// Left wall
			Square left = new Square();
			left.OrientationPlane = OrientationPlane.YZ;
			left.Center = new Vector3(Center.x + Size.x/2, Center.y, Center.z);
			left.Size = new Vector2(Size.y, Size.z);

			// Back wall
			Square back = new Square();
			back.OrientationPlane = OrientationPlane.XY;
			back.Center = new Vector3(Center.x, Center.y, Center.z - Size.z/2);
			back.Size = new Vector2(Size.x, Size.y);

			// Front wall
			Square front = new Square();
			front.OrientationPlane = OrientationPlane.XY;
			front.Center = new Vector3(Center.x, Center.y, Center.z + Size.z/2);
			front.Size = new Vector2(Size.x, Size.y);

			// Top
			Square top = new Square();
			top.Center = new Vector3(Center.x, Center.y + Size.y/2, Center.z);
			top.Size = new Vector2(Size.x, Size.z);

			// Merge all sides
			Merge merge = new Merge();
			merge.Input(Reverse.Process(bottom.Output()));
			merge.Input(Reverse.Process(right.Output()));
			merge.Input(left.Output());
			merge.Input(Reverse.Process(back.Output()));
			merge.Input(front.Output());
			merge.Input(top.Output());

			// Fuse vertices
			if (FuseVertices) {
				Fuse fuse = new Fuse();
				fuse.Input(merge.Output());
				return fuse.Output();
			} else {
				return merge.Output();
			}
		}

	} // class

} // namespace