using UnityEngine;
using System.Reflection;

namespace Forge {

	public class Parameter : Operator {

		[Input]
		public string Label = "Parameter";

		private MemberInfo _parameterInput = null;
		public IOOutlet ParameterInput {
			get {
				if (_parameterInput == null) {
					System.Type type = GetType();

					MemberInfo[] members = type.GetMembers();
					foreach (MemberInfo member in members) {
						if (System.Attribute.IsDefined(member, typeof(ParameterInputAttribute))) {
							_parameterInput = member;
							break;
						}
					}
					if (_parameterInput == null) {
						Debug.LogErrorFormat("Operator.ParameterInput error: Operator {0} is not an Input Operator", GetType().Name);
					}
				}
				return new IOOutlet(_parameterInput, IOOutletType.Input);
			}
		}

		private MethodInfo _parameterGUI = null;
		public MethodInfo ParameterGUI {
			get {
				if (_parameterGUI == null) {
					System.Type type = GetType();

					MemberInfo[] members = type.GetMembers();
					foreach (MemberInfo member in members) {
						if (System.Attribute.IsDefined(member, typeof(ParameterGUIAttribute)) && member is MethodInfo) {
							_parameterGUI = (MethodInfo)member;
							break;
						}
						Debug.LogErrorFormat("Operator.ParameterGUI error: Operator {0} does not have a method with the ParameterGUI attribute", GetType().Name);
					}
				}
				return _parameterGUI;
			}
		}

	}

}