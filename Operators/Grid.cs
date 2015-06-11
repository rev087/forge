using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Forge.Operators {

	public class Grid {

		public OrientationPreset Orientation;
		public Vector2 Size;
		public Vector2 Center;
		public Vector2 Cells = new Vector2(2, 2);

		public Geometry Output() {
			Geometry geo = new Geometry();

			// Square cell = new Square();

			return geo;
		}
		
	}

}