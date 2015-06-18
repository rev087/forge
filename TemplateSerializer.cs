using UnityEngine;
using System.Collections.Generic;

namespace Forge {

	public static class TemplateSerializer {

		public static string Serialize(this Template template) {

			// Template
			var tplJs = new JSONObject(JSONObject.Type.OBJECT);

			// Operators
			var opsJs = new JSONObject(JSONObject.Type.ARRAY);
			tplJs.AddField("Operators", opsJs);

			foreach (KeyValuePair<string, Operator> kvp in template.Operators) {
				opsJs.Add(kvp.Value.Serialize());
			}

			// Connections
			var connsJs = new JSONObject(JSONObject.Type.ARRAY);
			tplJs.AddField("Connections", connsJs);

			foreach (IOConnection conn in template.Connections) {
				connsJs.Add(conn.Serialize());
			}

			return tplJs.Print(true);
		}

		public static JSONObject Serialize(this IOConnection conn) {
			var connJs = new JSONObject(JSONObject.Type.OBJECT);
			connJs.AddField("From", conn.From.GUID);
			connJs.AddField("Output", conn.Output.Name);
			connJs.AddField("To", conn.To.GUID);
			connJs.AddField("Input", conn.Input.Name);
			return connJs;
		}

		public static void Deserialize(this Template template, string json) {
			template.Clear();
			var tplJs = new JSONObject(json);

			if (!tplJs.HasField("Operators") || !tplJs.HasField("Connections")) {
				Debug.LogWarning("Invalid JSON");
			} else {
				var opsJs = tplJs.GetField("Operators");
				var connsJs = tplJs.GetField("Connections");
				foreach (var opJs in opsJs.list) {
					var type = System.Type.GetType(opJs["Type"].str);
					var op = (Operator) System.Activator.CreateInstance(type);
					op.Deserialize(opJs);
					template.AddOperator(op);
				}
				foreach (var connJs in connsJs.list) {
					// Debug.Log(connJs);
				}
			}
		}

	}

}