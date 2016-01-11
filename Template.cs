using UnityEngine;
using System.Collections.Generic;
using Forge.Operators;
using Forge.Extensions;

namespace Forge {

	[System.Serializable]
	public class Template : ScriptableObject {

		public delegate void ChangedHandler(Template template);
		public event ChangedHandler Changed;

		public Dictionary<string, Operator> Operators = new Dictionary<string, Operator>();
		public List<IOConnection> Connections = new List<IOConnection>();
		public string JSON = "";

		private Dictionary<IOConnection, int> _connectionPriorities;

		private void CommitChanges() {
			TemplateSerializer.Serialize(this);
			if (Changed != null) Changed(this);
		}

		void OnEnable() {
			TemplateSerializer.Deserialize(this);
		}

		public void AddOperator(Operator op) {
			Operators.Add(op.GUID, op);
			CommitChanges();
		}

		public void RemoveOperator(Operator op) {
			for (int i = Connections.Count-1; i >= 0; i--) {
				if (Connections[i].From.GUID == op.GUID || Connections[i].To.GUID == op.GUID) {
					Connections.Remove(Connections[i]);
				}
			}
			Operators.Remove(op.GUID);
			CommitChanges();
		}

		public Operator OperatorWithGUID(string GUID) {
			foreach (var kvp in Operators) {
				if (kvp.Value.GUID == GUID) return kvp.Value;
			}
			return null;
		}

		public void Connect(Operator outOp, IOOutlet output, Operator inOp, IOOutlet input) {
			Connections.Add(new IOConnection() { From=outOp, Output=output, To=inOp, Input=input });
			CommitChanges();
		}

		public void Disconnect(IOConnection conn) {
			Connections.Remove(conn);
			CommitChanges();
		}

		public void MoveConnection(IOConnection conn, int newIndex) {
			Connections.Remove(conn);
			Connections.Insert(newIndex, conn);
			CommitChanges();
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

		private int GetPriority(IOConnection conn) {
			if (_connectionPriorities.ContainsKey(conn)) {
				return _connectionPriorities[conn];
			} else {
				var priority = 0;
				foreach (var input in conn.From.Inputs) {
					IOConnection[] conns = ConnectionsTo(conn.From, input);
					foreach (var prior in conns) {
						priority++;
						priority += GetPriority(prior);
					}
				}
				_connectionPriorities.Add(conn, priority);
				return priority;
			}
		}

		private int CompareConnectionPriority(IOConnection a, IOConnection b) {
			int priorityA = _connectionPriorities[a];
			int priorityB = _connectionPriorities[b];

			if (priorityA < priorityB) return -1;
			else if (priorityA > priorityB) return 1;
			else return 0;
		}

		public virtual Geometry Build() {

			//TemplateSerializer.Deserialize(this);

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

			// Sort the connections by build priority
			List<IOConnection> buildOrder = new List<IOConnection>();
			_connectionPriorities = new Dictionary<IOConnection, int>();
			foreach (var conn in Connections) {
				GetPriority(conn);
				buildOrder.Add(conn);
			}
			buildOrder.Sort(CompareConnectionPriority);

			var geoOutputOp = GetGeometryOutput();
			if (geoOutputOp != null) {

				// Connections
				foreach (IOConnection conn in buildOrder) {
					object val = conn.From.GetValue(conn.Output);
					conn.To.SetValue(conn.Input, val);
				}

				// For now, we just retrieve the Output with the Geometry type in the
				// Operator marked as IsGeometryOutput. In the future, the template will
				// have specialized output nodes instead.
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