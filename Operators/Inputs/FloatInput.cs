using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Forge.Operators {

	[OperatorMetadata(Category = "Input", Title = "Float", DataType = typeof(float))]
	public class FloatInput : Parameter {

		[Input]
		public new string Label = "Float";

		[Input]
		public float Min = 0f;

		[Input]
		public float Max = 100f;

		[Input]
		public float Default = 0f;

		[Input]
		public bool DisplaySlider = true;

		[Output]
		[ParameterInput]
		public float Float = 0.0f;

#if UNITY_EDITOR
		[ParameterGUI]
		public object InputGUI(object floatValue) {
			Float = EditorGUILayout.Slider(Label, floatValue == null ? Default : (float) floatValue, Min, Max);
			return Float;
		}
#endif
	}

}