using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using Forge.Extensions;

namespace Forge {

	[System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Field | System.AttributeTargets.Property, AllowMultiple = false)]
	public class OutputAttribute : System.Attribute {}

	[System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Field | System.AttributeTargets.Property, AllowMultiple = false)]
	public class InputAttribute : System.Attribute {}

	[System.Serializable]
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
			set {
				_guid = value;
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
							inputs.Add(new IOOutlet(member));
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
							outputs.Add(new IOOutlet(member));
						}
					}

					_outputs = outputs.ToArray();
				}
				return _outputs;
			}
		}

		public IOOutlet GetInput(string name) {
			foreach (IOOutlet input in Inputs) {
				if (input.Name == name) return input;
			}
			throw new System.ArgumentException(System.String.Format("Operator {0} does not contain an input named {1}", Title, name));
		}

		public IOOutlet GetOutput(string name) {
			foreach (IOOutlet output in Outputs) {
				if (output.Name == name) return output;
			}
			throw new System.ArgumentException(System.String.Format("Operator {0} does not contain an output named {1}", Title, name));
		}

		public T GetValue<T>(IOOutlet outlet) {
			if (outlet.Member is PropertyInfo) {
				return (T) ((PropertyInfo)outlet.Member).GetValue(this, null);
			}
			else if (outlet.Member is FieldInfo) {
				return (T) ((FieldInfo)outlet.Member).GetValue(this);
			}
			else if (outlet.Member is MethodInfo) {
				return (T) ((MethodInfo)outlet.Member).Invoke(this, null);
			}
			else {
				throw new System.ArgumentException(System.String.Format("Operator {0} could not retrieve the value of {1}", Title, outlet.Name));
			}
		}

		public void SetValue<T>(IOOutlet outlet, T val) {
			if (outlet.Member is PropertyInfo) {
				((PropertyInfo)outlet.Member).SetValue(this, (object)val, null);
			}
			else if (outlet.Member is FieldInfo) {
				((FieldInfo)outlet.Member).SetValue(this, (object)val);
			}
			else if (outlet.Member is MethodInfo) {
				((MethodInfo)outlet.Member).Invoke(this, new object[] {(object)val});
			}
			else {
				throw new System.ArgumentException(System.String.Format("Operator {0} could not set the value of {1} to {2}", Title, outlet.Name, val));
			}
		}

		public static System.Type[] GetAvailableOperators() {
			var opTypes = new List<System.Type>();
			var allTypes = typeof(Operator).Assembly.GetTypes();
			foreach (var type in allTypes) {
				if (type.IsSubclassOf(typeof(Operator))) {
					opTypes.Add(type);
				}
			}
			return opTypes.ToArray();
		}

	}

}