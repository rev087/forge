using UnityEngine;
using Forge.Extensions;
using System.Xml;

namespace Forge {

	public static class OperatorSerializer {

		public static JSONObject Serialize(this Operator op) {

			var opJs = new JSONObject(JSONObject.Type.OBJECT);
			opJs.AddField("Type", op.GetType().ToString());
			opJs.AddField("GUID", op.GUID);

			var posJs = new JSONObject(JSONObject.Type.ARRAY);
			posJs.Add(op.EditorPosition.x, op.EditorPosition.y);
			opJs.AddField("EditorPosition", posJs);

			opJs.AddField("IsGeometryOutput", new JSONObject(op.IsGeometryOutput));

			var paramsJs = new JSONObject(JSONObject.Type.OBJECT);
			opJs.AddField("Params", paramsJs);

			foreach (IOOutlet outlet in op.Inputs) {

				// float
				if (outlet.DataType == typeof(System.Single)) {
					paramsJs.AddField(outlet.Name, op.GetValue<float>(outlet));
				}

				// bool
				else if (outlet.DataType == typeof(System.Boolean)) {
					paramsJs.AddField(outlet.Name, op.GetValue<bool>(outlet));
				}

				// int
				else if (outlet.DataType == typeof(System.Int32)) {
					paramsJs.AddField(outlet.Name, op.GetValue<int>(outlet));
				}

				// string
				else if (outlet.DataType == typeof(System.String)) {
					paramsJs.AddField(outlet.Name, op.GetValue<string>(outlet));
				}

				// Vector2
				else if (outlet.DataType == typeof(Vector2)) {
					var vJs = new JSONObject(JSONObject.Type.ARRAY);
					var v = op.GetValue<Vector2>(outlet);
					vJs.Add(v.x, v.y);
					paramsJs.AddField(outlet.Name, vJs);
				}

				// Vector3
				else if (outlet.DataType == typeof(Vector3)) {
					var vJs = new JSONObject(JSONObject.Type.ARRAY);
					var v = op.GetValue<Vector3>(outlet);
					vJs.Add(v.x, v.y, v.z);
					paramsJs.AddField(outlet.Name, vJs);
				}

				// Vector4
				else if (outlet.DataType == typeof(Vector4)) {
					var vJs = new JSONObject(JSONObject.Type.ARRAY);
					var v = op.GetValue<Vector4>(outlet);
					vJs.Add(v.x, v.y, v.z, v.w);
					paramsJs.AddField(outlet.Name, vJs);
				}

				// Enum
				else if (outlet.DataType.IsEnum) {
					object objValue = ((System.Reflection.FieldInfo) outlet.Member).GetValue(op);
					paramsJs.AddField(outlet.Name, objValue.ToString());
				}

				else {
					paramsJs.AddField(outlet.Name, new JSONObject(JSONObject.Type.NULL));
				}

			}

			return opJs;
		}

		public static void Deserialize(this Operator op, JSONObject opJs) {
			op.GUID = opJs["GUID"].str;
			op.EditorPosition = new Vector2(opJs["EditorPosition"][0].n, opJs["EditorPosition"][1].n);
			op.IsGeometryOutput = opJs["IsGeometryOutput"].b;

			var paramsJs = opJs["Params"];
			for (int i = 0; i < op.Inputs.Length; i++) {
				string param = op.Inputs[i].Name;

				// float
				if (op.Inputs[i].DataType == typeof(System.Single)) {
					op.SetValue<float>(op.Inputs[i], paramsJs[param].n);
				}

				// bool
				else if (op.Inputs[i].DataType == typeof(System.Boolean)) {
					op.SetValue<bool>(op.Inputs[i], paramsJs[param].b);
				}

				// int
				else if (op.Inputs[i].DataType == typeof(System.Int32)) {
					op.SetValue<int>(op.Inputs[i], (int) paramsJs[param].n);
				}

				// string
				else if (op.Inputs[i].DataType == typeof(System.String)) {
					op.SetValue<string>(op.Inputs[i], (string) paramsJs[param].str);
				}

				// Vector2
				else if (op.Inputs[i].DataType == typeof(Vector2)) {
					op.SetValue<Vector2>(op.Inputs[i], new Vector2(
						paramsJs[param][0].n,
						paramsJs[param][1].n
					));
				}

				// Vector3
				else if (op.Inputs[i].DataType == typeof(Vector3)) {
					op.SetValue<Vector3>(op.Inputs[i], new Vector3(
						paramsJs[param][0].n,
						paramsJs[param][1].n,
						paramsJs[param][2].n
					));
				}

				// Enum
				else if (op.Inputs[i].DataType.IsEnum) {
					object val = System.Enum.Parse(op.Inputs[i].DataType, paramsJs[param].str);
					op.SetValue(op.Inputs[i], val);
				}

			}
		}

	}
	
}