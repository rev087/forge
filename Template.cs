using UnityEngine;
using System.Collections.Generic;
using Forge.Operators;
using System.Xml;

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

	public class Template : ScriptableObject {

		public Dictionary<string, Operator> Operators = new Dictionary<string, Operator>();
		public List<IOConnection> Connections = new List<IOConnection>();
		public string JSON = "";

		public void LoadDemo() {

			var c = new Circle();
			c.EditorPosition = new Vector2(50f, 50f);
			AddOperator(c);

			var m = new Mirror();
			m.EditorPosition = new Vector2(300f, 50f);
			AddOperator(m);

			var co = c.GetOutput("Output");
			var mi = m.GetInput("Input");
			Connect(c, co, m, mi);

			var c2 = new Circle();
			c2.EditorPosition = new Vector2(300f, 200f);
			AddOperator(c2);

			var f = new FloatValue();
			f.EditorPosition = new Vector2(50f, 350f);
			AddOperator(f);
			
			var ff = f.GetOutput("Float");
			var c2r = c2.GetInput("Radius");
			Connect(f, ff, c2, c2r);
		}

		public void AddOperator(Operator op) {
			Operators.Add(op.GUID, op);
		}

		public void Connect(Operator outOp, IOOutlet output, Operator inOp, IOOutlet input) {
			Connections.Add(new IOConnection() { From=outOp, Output=output, To=inOp, Input=input });
		}

	}

}