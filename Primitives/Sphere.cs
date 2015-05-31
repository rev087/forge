using UnityEngine;
using Forge.Filters;

namespace Forge.Primitives {

	public class Sphere {

		public float Radius = 0.5f;
		public Vector3 Center = Vector3.zero;
		public int Segments = 8;
		public OrientationPreset Orientation;

		public Geometry Output() {

			if (Segments < 3) {
				Debug.LogError("Sphere error: Spheres must have at least 3 segments");
			}

			Merge sphere = new Merge();

			Geometry prevCircle = Geometry.Empty;

			Vector3 northCap = Vector3.zero;

			for (int i = 0; i < Segments; i++) {
				float angle = 90 + (180 * i / (Segments - 1));
				float sin = Mathf.Sin(angle * Mathf.Deg2Rad) * Radius;

				if (i == 0) {
					northCap = new Vector3(0f, sin, 0f);
				}

				if (i > 0 && i < Segments - 1) {
					float cos = Mathf.Cos(angle * Mathf.Deg2Rad) * Radius;

					var c = new Circle();
					c.Segments = Segments;
					c.Center = new Vector3(0f, sin, 0f);
					c.Radius = cos;
					Geometry circle = c.Output();

					if (i == 1) {
						var converge = new Converge(circle);
						converge.RecalculateNormals = false;
						converge.Point = northCap;
						sphere.Input(converge.Output());
					} else {
						var bridge = new Bridge(circle, prevCircle);
						bridge.RecalculateNormals = false;
						sphere.Input(bridge.Output());
					}

					prevCircle = circle;
				}

				if (i == Segments - 1) {
					var converge = new Converge(prevCircle);
					converge.RecalculateNormals = true;
					converge.Point = new Vector3(0f, sin, 0f);
					sphere.Input(Reverse.Process(converge.Output()));
				}

			}

			return sphere.Output();
		}

	} // class

} // namespace