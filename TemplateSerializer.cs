using UnityEngine;
using System.Collections.Generic;

namespace Forge {

	public static class TemplateSerializer {

		public static string Serialize(this Template template) {

			// Template
			var tplJs = new JSONObject(JSONObject.Type.OBJECT);

			// Operators
			var opsJs = new JSONObject(JSONObject.Type.ARRAY);
			tplJs.AddField("operators", opsJs);

			foreach (KeyValuePair<string, Operator> kvp in template.Operators) {
				opsJs.Add(kvp.Value.Serialize());
			}

			// Connections
			var connsJs = new JSONObject(JSONObject.Type.ARRAY);
			tplJs.AddField("connections", connsJs);

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

	}

}