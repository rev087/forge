using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using Forge.Extensions;

namespace Forge {

	[System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Field | System.AttributeTargets.Property, AllowMultiple = false)]
	public class OutputAttribute : System.Attribute {}

	[System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Field | System.AttributeTargets.Property, AllowMultiple = false)]
	public class InputAttribute : System.Attribute {}

	[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
	public class ParameterGUIAttribute : System.Attribute { }

	[System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Field | System.AttributeTargets.Property, AllowMultiple = false)]
	public class ParameterInputAttribute : System.Attribute { }

	[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
	public class OperatorMetadataAttribute : System.Attribute {
		public string Title;
		public string Category;
		public string Description;
		public System.Type DataType;
	}

	[System.Serializable]
	public class Operator {

		public Vector2 EditorPosition = Vector2.zero;
		public bool IsGeometryOutput = false;

		public string OperatorError = null;

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

		private OperatorMetadataAttribute _metadata = null;
		public OperatorMetadataAttribute Metadata {
			get {
				if (_metadata != null) return _metadata;
				System.Type opType = this.GetType();
				var meta = System.Attribute.GetCustomAttribute(opType, typeof(OperatorMetadataAttribute)) as OperatorMetadataAttribute;
				if (meta != null) {
					_metadata = meta;
					if (_metadata.Title == null) {
						_metadata.Title = opType.Name;
					}
				} else {
					_metadata = new OperatorMetadataAttribute() { Title = opType.Name };
				}
				return _metadata;
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
							inputs.Add(new IOOutlet(member, IOOutletType.Input));
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
							outputs.Add(new IOOutlet(member, IOOutletType.Output));
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
			throw new System.ArgumentException(System.String.Format("Operator {0} does not contain an input named {1}", Metadata.Title, name));
		}

		public IOOutlet GetOutput(string name) {
			foreach (IOOutlet output in Outputs) {
				if (output.Name == name) return output;
			}
			throw new System.ArgumentException(System.String.Format("Operator {0} does not contain an output named {1}", Metadata.Title, name));
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
				throw new System.ArgumentException(System.String.Format("Operator {0} could not retrieve the value of {1}", Metadata.Title, outlet.Name));
			}
		}

		public object GetValue(IOOutlet outlet) {
			if (outlet.Member is PropertyInfo) {
				return (object) ((PropertyInfo)outlet.Member).GetValue(this, null);
			}
			else if (outlet.Member is FieldInfo) {
				return (object) ((FieldInfo)outlet.Member).GetValue(this);
			}
			else if (outlet.Member is MethodInfo) {
				return (object) ((MethodInfo)outlet.Member).Invoke(this, null);
			}
			else {
				throw new System.ArgumentException(System.String.Format("Operator {0} could not retrieve the value of {1}", Metadata.Title, outlet.Name));
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
				throw new System.ArgumentException(System.String.Format("Operator {0} could not set the value of {1} to {2}", Metadata.Title, outlet.Name, val));
			}
		}

		public void SetValue(IOOutlet outlet, object val) {
			
			// Properties
			if (outlet.Member is PropertyInfo) {
				((PropertyInfo)outlet.Member).SetValue(this, val, null);
			}

			// Fields
			else if (outlet.Member is FieldInfo) {

				// Multi inputs
				if (outlet.DataType.IsCollection()) {
					var methodInfo = outlet.DataType.GetMethod("Add");
					var list = GetValue(outlet);
					methodInfo.Invoke(list, new object[] {val});
				}

				// Single inputs
				else {
					((FieldInfo)outlet.Member).SetValue(this, val);
				}
			}

			// Methods
			else if (outlet.Member is MethodInfo) {
				((MethodInfo)outlet.Member).Invoke(this, new object[] {val});
			}
			else {
				throw new System.ArgumentException(System.String.Format("Operator {0} could not set the value of {1} to {2}", Metadata.Title, outlet.Name, val));
			}
		}

		public static System.Type[] GetAvailableOperators() {
			var opTypes = new List<System.Type>();
			var allTypes = typeof(Operator).Assembly.GetTypes();
			foreach (var type in allTypes) {
				if (type.IsSubclassOf(typeof(Operator)) && type != typeof(Parameter)) {
					opTypes.Add(type);
				}
			}
			return opTypes.ToArray();
		}

		// Override if operator features gizmos
		public virtual void OnDrawGizmos(GameObject go) {}

	}

}