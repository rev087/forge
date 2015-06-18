using UnityEngine;
using Forge.Extensions;
using System.Xml;

namespace Forge {

	public static class OperatorSerializer {

		public static JSONObject Serialize(this Operator op) {

			var opJson = new JSONObject(JSONObject.Type.OBJECT);
			opJson.AddField("Type", op.GetType().ToString());
			opJson.AddField("GUID", op.GUID);

			var paramsJson = new JSONObject(JSONObject.Type.OBJECT);
			opJson.AddField("Params", paramsJson);

			foreach (IOOutlet outlet in op.Inputs) {

				// float
				if (outlet.Type == typeof(System.Single)) {
					paramsJson.AddField(outlet.Name, op.GetValue<float>(outlet));
				}

				// bool
				else if (outlet.Type == typeof(System.Boolean)) {
					paramsJson.AddField(outlet.Name, op.GetValue<bool>(outlet));
				}

				// int
				else if (outlet.Type == typeof(System.Int32)) {
					paramsJson.AddField(outlet.Name, op.GetValue<int>(outlet));
				}

				// Vector2
				else if (outlet.Type == typeof(Vector2)) {
					var vJson = new JSONObject(JSONObject.Type.ARRAY);
					var v = op.GetValue<Vector2>(outlet);
					vJson.Add(v.x, v.y);
					paramsJson.AddField(outlet.Name, vJson);
				}

				// Vector3
				else if (outlet.Type == typeof(Vector3)) {
					var vJson = new JSONObject(JSONObject.Type.ARRAY);
					var v = op.GetValue<Vector3>(outlet);
					vJson.Add(v.x, v.y, v.z);
					paramsJson.AddField(outlet.Name, vJson);
				}

				// Vector4
				else if (outlet.Type == typeof(Vector4)) {
					var vJson = new JSONObject(JSONObject.Type.ARRAY);
					var v = op.GetValue<Vector4>(outlet);
					vJson.Add(v.x, v.y, v.z, v.w);
					paramsJson.AddField(outlet.Name, vJson);
				}

				// Enum
				else if (outlet.Type.IsEnum) {
					object objValue = ((System.Reflection.FieldInfo) outlet.Member).GetValue(op);
					paramsJson.AddField(outlet.Name, objValue.ToString());
				}

				else {
					paramsJson.AddField(outlet.Name, new JSONObject(JSONObject.Type.NULL));
				}

			}

			return opJson;
		}

		public static void Deserialize(this Operator op, JSONObject opJs) {
			op.GUID = opJs["GUID"].str;

			var paramsJs = opJs["Params"];
			for (int i = 0; i < op.Inputs.Length; i++) {
				string param = op.Inputs[i].Name;

				// float
				if (op.Inputs[i].Type == typeof(System.Single)) {
					op.SetValue<float>(op.Inputs[i], paramsJs[param].n);
				}

				// bool
				else if (op.Inputs[i].Type == typeof(System.Boolean)) {
					op.SetValue<bool>(op.Inputs[i], paramsJs[param].b);
				}

				// int
				else if (op.Inputs[i].Type == typeof(System.Int32)) {
					op.SetValue<int>(op.Inputs[i], (int) paramsJs[param].n);
				}

				// Vector3
				else if (op.Inputs[i].Type == typeof(Vector3)) {
					op.SetValue<Vector3>(op.Inputs[i], new Vector3(
						paramsJs[param][0].n,
						paramsJs[param][1].n,
						paramsJs[param][2].n
					));
				}

				// Debug.LogFormat("{0} - {1}\n{2}", paramsJs[param], op.Inputs[i].Type, paramsJs.list[i]);
			}
		}

	}
	
}