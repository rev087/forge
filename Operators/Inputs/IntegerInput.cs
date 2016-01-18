using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Forge.Operators {

	[OperatorMetadata(Category = "Input", Title = "Integer", DataType = typeof(float))]
	public class IntegerInput : Parameter {

		[Input]
		public int Min = 0;

		[Input]
		public int Max = 100;

		[Input]
		public int Default = 0;

		[Input]
		public bool DisplaySlider = true;

		[Output]
		[ParameterInput]
		public int Integer = 0;

#if UNITY_EDITOR
		[ParameterGUI]
		public object InputGUI(object intValue) {
			int value = intValue == null ? Default : (int)intValue;
			if (DisplaySlider) {
				Integer = (int) EditorGUILayout.Slider(Label, value, Min, Max);
			} else {
				Integer = (int)EditorGUILayout.IntField(Label, value);
			}
			return Integer;
		}
#endif
	}

}