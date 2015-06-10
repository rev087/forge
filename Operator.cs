using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace Forge {

	[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
	public class OutputAttribute : System.Attribute {}

	[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
	public class InputAttribute : System.Attribute {}

	public struct IOOutlet {
		public System.Type Type;
		public string Name;

		public IOOutlet(System.Type type, string name) {
			Type = type;
			Name = name;
		}

		public static IOOutlet None {
			get { return new IOOutlet(typeof(System.Boolean), null); }
		}

		public bool IsNone() {
			return Type == typeof(System.Boolean) && Name == null;
		}
	}

	public class Operator {

		public Vector2 EditorPosition = Vector2.zero;

		private string _guid = null;
		public string Guid {
			get {
				if (_guid == null) {
					_guid = System.Guid.NewGuid().ToString("D");
				}
				return _guid;
			}
		}

		private string _title = null;
		public string Title {
			get {
				if (_title == null) {
					_title = GetType().Name;
				}
				return _title;
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
							inputs.Add(new IOOutlet() { Name=fields[i].Name, Type=fields[i].FieldType });
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
							outputs.Add(new IOOutlet() { Name=methodInfo.Name, Type=methodInfo.ReturnType });
						}
					}

					_outputs = outputs.ToArray();
				}
				return _outputs;
			}
		}

		public static void Operators() {
			var types = typeof(Operator).Assembly.GetTypes();
			foreach (var type in types) {
				if (type.IsSubclassOf(typeof(Operator))) {
					Debug.Log(type.Name);
				}
			}
		}

	}

}