using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using Forge.Extensions;

namespace Forge {

	[System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Field | System.AttributeTargets.Property, AllowMultiple = false)]
	public class OutputAttribute : System.Attribute {}

	[System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Field | System.AttributeTargets.Property, AllowMultiple = false)]
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

		public static bool CanConnect(IOOutlet output, IOOutlet input) {
			return output.Type == input.Type;
		}
	}

	public class Operator {

		public Vector2 EditorPosition = Vector2.zero;

		private string _guid = null;
		public string GUID {
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

					var inputs = new List<IOOutlet>();

					MemberInfo[] members = type.GetMembers();
					foreach (MemberInfo member in members) {
						if (System.Attribute.IsDefined(member, typeof(InputAttribute))) {
							inputs.Add(new IOOutlet() { Name=member.Name, Type=member.OutletType() });
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

					var outputs = new List<IOOutlet>();

					MemberInfo[] members = type.GetMembers();
					foreach (MemberInfo member in members) {
						if (System.Attribute.IsDefined(member, typeof(OutputAttribute))) {
							outputs.Add(new IOOutlet() { Name=member.Name, Type=member.OutletType() });
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