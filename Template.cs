using UnityEngine;
using System.Collections.Generic;
using Forge.Primitives;
using Forge.Filters;

namespace Forge {

	public struct IOConnection {
		public Operator From;
		public IOOutlet Output;
		public Operator To;
		public IOOutlet Input;

		public IOConnection(Operator a, IOOutlet o, Operator b, IOOutlet i) {
			From = a;
			Output = o;
			To = b;
			Input = i;
		}
	}

	public class Template {

		public Dictionary<string, Operator> Operators = new Dictionary<string, Operator>();
		public List<IOConnection> Connections = new List<IOConnection>();

		public Template() {

			var c = new Circle();
			c.EditorPosition = new Vector2(50f, 50f);

			var m = new Mirror();
			m.EditorPosition = new Vector2(300f, 50f);

			Operators.Add(c.Guid, c);
			Operators.Add(m.Guid, m);

			var output = new IOOutlet(typeof(Geometry), "Output");
			var input = new IOOutlet(typeof(Axis), "Axis");

			var io1 = new IOConnection() { From=c, Output=output, To=m, Input=input };
			Connections.Add(io1);
		}

	}

}