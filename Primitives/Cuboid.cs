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

			// West wall
			Square west = new Square();
			west.OrientationPlane = OrientationPlane.YZ;
			west.Center = new Vector3(Center.x - Size.x/2, Center.y, Center.z);
			west.Size = new Vector2(Size.y, Size.z);

			// East wall
			Square east = new Square();
			east.OrientationPlane = OrientationPlane.YZ;
			east.Center = new Vector3(Center.x + Size.x/2, Center.y, Center.z);
			east.Size = new Vector2(Size.y, Size.z);

			// South wall
			Square south = new Square();
			south.OrientationPlane = OrientationPlane.XY;
			south.Center = new Vector3(Center.x, Center.y, Center.z - Size.z/2);
			south.Size = new Vector2(Size.x, Size.y);

			// North wall
			Square north = new Square();
			north.OrientationPlane = OrientationPlane.XY;
			north.Center = new Vector3(Center.x, Center.y, Center.z + Size.z/2);
			north.Size = new Vector2(Size.x, Size.y);

			// Top
			Square top = new Square();
			top.Center = new Vector3(Center.x, Center.y + Size.y/2, Center.z);
			top.Size = new Vector2(Size.x, Size.z);

			// Merge all sides
			Merge merge = new Merge();
			merge.Input(Reverse.Process(bottom.Output()));
			merge.Input(Reverse.Process(west.Output()));
			merge.Input(east.Output());
			merge.Input(south.Output());
			merge.Input(Reverse.Process(north.Output()));
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