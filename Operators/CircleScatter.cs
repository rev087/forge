using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Forge.Operators {

	public class CircleScatter {

		public float Radius = 0.5f;
		public int Count = 10;
		public int Seed = 1;

		public Geometry Output() {

			Geometry geo = new Geometry();

			Random.seed = Seed;

			// Vertices
			geo.Vertices = new Vector3 [Count];
			geo.Normals = new Vector3 [Count];
			geo.UV = new Vector2[0];
			geo.Triangles = new int[0];

			for (int i = 0; i < Count; i++) {
				float dist = Random.Range(0f, Radius);
				float angle = Random.Range(0f, Mathf.PI * 2);
				float cos = Mathf.Cos(angle);
				float sin = Mathf.Sin(angle);
				geo.Vertices[i] = new Vector3(cos * dist, 0, sin * dist);
				geo.Normals[i] = geo.Vertices[i].normalized;
			}

			return geo;
		}
		
	}

}