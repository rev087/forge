using UnityEngine;
using System.Collections.Generic;
using Forge.Operators;

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
			AddOperator(c);

			var m = new Mirror();
			m.EditorPosition = new Vector2(300f, 50f);
			AddOperator(m);

			var c2 = new Circle();
			c2.EditorPosition = new Vector2(300f, 200f);
			AddOperator(c2);

			var f = new FloatValue();
			f.EditorPosition = new Vector2(50f, 350f);
			AddOperator(f);

			var output = new IOOutlet(typeof(Geometry), "Output");
			var input = new IOOutlet(typeof(Axis), "Axis");
			Connections.Add(new IOConnection() { From=c, Output=output, To=m, Input=input });
		}

		public void AddOperator(Operator op) {
			Operators.Add(op.Guid, op);
		}

		public void Connect(Operator outOp, IOOutlet output, Operator inOp, IOOutlet input) {
			Connections.Add(new IOConnection() { From=outOp, Output=output, To=inOp, Input=input });
		}

	}

}