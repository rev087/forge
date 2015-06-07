using UnityEngine;
using System.Collections.Generic;
using Forge.Primitives;
using Forge.Filters;

namespace Forge {

	public struct IOConnection {

	}

	public class Template {

		public Dictionary<string, Operator> Operators = new Dictionary<string, Operator>();

		public Template() {

			var c = new Circle();
			c.EditorPosition = new Vector2(50f, 50f);

			var m = new Mirror();
			m.EditorPosition = new Vector2(300f, 50f);

			Operators.Add(c.Guid, c);
			Operators.Add(m.Guid, m);
		}

	}

}