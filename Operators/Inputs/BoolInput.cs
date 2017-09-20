using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Forge.Operators {

	[OperatorMetadata(Category = "Input", Title = "Boolean", DataType = typeof(float))]
	public class BoolInput : Parameter {

		[Input]
		public bool Default = false;

		[Output]
		[ParameterInput]
		public bool Value = false;

#if UNITY_EDITOR
		[ParameterGUI]
		public object InputGUI(object intValue) {
			Value = (bool) EditorGUILayout.Toggle(Label, Value);
			return Value;
		}
#endif
	}

}