using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace Forge {

	[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
	public class OutputAttribute : System.Attribute {}

	public struct IOOutlet {
		public string Type;
		public string Name;
	}

	public class Node {

		private static string TypeAlias(System.Type type) {
			// See https://msdn.microsoft.com/en-us/library/ya5y69ds

			if (type == typeof(System.Boolean)) return "bool";
			else if (type == typeof(System.String)) return "string";
			else if (type == typeof(System.Int32)) return "int";			
			else if (type == typeof(System.Single)) return "float";
			else if (type == typeof(System.Double)) return "double";

			else if (type == typeof(System.Byte)) return "byte";
			else if (type == typeof(System.SByte)) return "sbyte";
			else if (type == typeof(System.Char)) return "char";
			else if (type == typeof(System.Decimal)) return "decimal";
			else if (type == typeof(System.UInt32)) return "uint";
			else if (type == typeof(System.Int64)) return "long";
			else if (type == typeof(System.UInt64)) return "ulong";
			else if (type == typeof(System.Int16)) return "short";
			else if (type == typeof(System.UInt16)) return "ushort";

			return type.Name;
		}

		public Vector2 EditorPosition = Vector2.zero;

		private string _nodeName = null;
		public string NodeName {
			get {
				if (_nodeName == null) {
					_nodeName = GetType().Name;
				}
				return _nodeName;
			}
		}

		private IOOutlet[] _inputs = null;
		public IOOutlet[] Inputs {
			get {
				if (_inputs == null) {
					System.Type type = GetType();
					FieldInfo[] fields = type.GetFields();

					var inputs = new List<IOOutlet>();

					for (int i = 0; i < fields.Length; i++) {
						if (fields[i].DeclaringType == type) {
							inputs.Add(new IOOutlet() { Name=fields[i].Name, Type=TypeAlias(fields[i].FieldType) });
						}
					}

					_inputs = inputs.ToArray();
				}
				return _inputs;
			}
		}

		private IOOutlet[] _outputs = null;
		public IOOutlet[] Outputs {
			get{
				if (_outputs == null) {
					System.Type type = GetType();
					MethodInfo[] methods = type.GetMethods();

					var outputs = new List<IOOutlet>();

					foreach (MethodInfo methodInfo in methods) {
						var isOutput = System.Attribute.IsDefined(methodInfo, typeof(OutputAttribute));
						if (isOutput) {
							outputs.Add(new IOOutlet() { Name=methodInfo.Name, Type=TypeAlias(methodInfo.ReturnType) });
						}
					}

					_outputs = outputs.ToArray();
				}
				return _outputs;
			}
		}

		public void Nodes() {
			var types = GetType().Assembly.GetTypes();
			foreach (var type in types) {
				if (type.IsSubclassOf(typeof(Node))) {
					Debug.Log(type.Name);
				}
			}
		}

	}

}