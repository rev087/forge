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

			var c2 = new Circle();
			c2.EditorPosition = new Vector2(300f, 200f);

			Operators.Add(c.Guid, c);
			Operators.Add(m.Guid, m);
			Operators.Add(c2.Guid, c2);

			var output = new IOOutlet(typeof(Geometry), "Output");
			var input = new IOOutlet(typeof(Axis), "Axis");
			Connections.Add(new IOConnection() { From=c, Output=output, To=m, Input=input });
		}

		public void Connect(Operator outOp, IOOutlet output, Operator inOp, IOOutlet input) {
			Connections.Add(new IOConnection() { From=outOp, Output=output, To=inOp, Input=input });
		}

	}

}