using UnityEngine;
using System.Collections.Generic;
using Forge.Operators;
using Forge.Extensions;

namespace Forge {

	[System.Serializable]
	public class Template : ScriptableObject {

		public Dictionary<string, Operator> Operators = new Dictionary<string, Operator>();
		public List<IOConnection> Connections = new List<IOConnection>();
		public string JSON = "";

		void OnEnable() {
			TemplateSerializer.Deserialize(this);
		}

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

		public void AddOperator(Operator op, bool serialize = true) {
			Operators.Add(op.GUID, op);
			if (serialize) {
				TemplateSerializer.Serialize(this);
			}
		}

		public void RemoveOperator(Operator op, bool serialize = true) {
			for (int i = 0; i < Connections.Count; i++) {
				if (Connections[i].From.GUID == op.GUID || Connections[i].To.GUID == op.GUID) {
					Connections.Remove(Connections[i]);
				}
			}
			Operators.Remove(op.GUID);

			if (serialize) {
				TemplateSerializer.Serialize(this);
			}
		}

		public Operator OperatorWithGUID(string GUID) {
			foreach (var kvp in Operators) {
				if (kvp.Value.GUID == GUID) return kvp.Value;
			}
			return null;
		}

		public void Connect(Operator outOp, IOOutlet output, Operator inOp, IOOutlet input) {
			Connections.Add(new IOConnection() { From=outOp, Output=output, To=inOp, Input=input });
			TemplateSerializer.Serialize(this);
		}

		public void Disconnect(IOConnection conn) {
			Connections.Remove(conn);
			TemplateSerializer.Serialize(this);
		}

		public IOConnection[] ConnectionsTo(Operator toOp, IOOutlet input) {
			var conns = new List<IOConnection>();
			foreach (IOConnection conn in Connections) {
				if (conn.To.GUID == toOp.GUID && conn.Input == input) {
					conns.Add(conn);
				}
			}
			return conns.ToArray();
		}

		public void Clear() {
			Operators.Clear();
			Connections.Clear();
		}

		// Retrieves the first Operator with IsGeometryOutput = true
		public Operator GetGeometryOutput() {
			foreach (var kvp in Operators) {
				if (kvp.Value.IsGeometryOutput) {
					return kvp.Value;
				}
			}
			return null;
		}

		public virtual Geometry Build() {

			// Reset multi imputs
			foreach (var kvp in Operators) {
				foreach (IOOutlet input in kvp.Value.Inputs) {
					if (input.DataType.IsCollection()) {
						var clearMethod = input.DataType.GetMethod("Clear");
						if (clearMethod != null) {
							object collection = kvp.Value.GetValue(input);
							clearMethod.Invoke(collection, null);
						}
					}
				}
			}

			var geoOutputOp = GetGeometryOutput();
			if (geoOutputOp != null) {

				// Connections
				foreach (IOConnection conn in Connections) {
					object val = conn.From.GetValue(conn.Output);
					conn.To.SetValue(conn.Input, val);
				}

				// For now, we just retrieve the Output with the Geometry type in the
				// Operator marked as IsGeometryOutput.
				foreach (IOOutlet output in geoOutputOp.Outputs) {
					if (output.DataType == typeof(Geometry)) {
						return geoOutputOp.GetValue<Geometry>(output);
					}
				}

			}
			return Geometry.Empty;
		}

	}

}