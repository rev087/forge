using UnityEngine;

namespace Forge {

	/*
	Parameter values are serialized with the ProceduralAsset instance. Because the generic type `object`
	cant be serialized by Unity, we use this struct instead.
	Possible optimization: replace string _serializedValue by multiple value fields for different types
	ex. FloatValue, IntegerValue etc.
	*/

	public enum ParameterType { Float, Integer, String, Boolean, Vector2, Vector3, Vector4, Color, Invalid }

	[System.Serializable]
	public struct ParameterValue {
		public string GUID;
		[SerializeField]
		private string _serializedValue;
		public ParameterType Type;
		
		public ParameterValue (string guid, object value) {

			GUID = guid;
			_serializedValue = value.ToString();

			if (value is System.Int32)			Type = ParameterType.Integer;
			else if (value is System.Single)	Type = ParameterType.Float;
			else if (value is System.String)	Type = ParameterType.String;
			else if (value is System.Boolean)	Type = ParameterType.Boolean;
			else if (value is Vector2)			Type = ParameterType.Vector2;
			else if (value is Vector3)			Type = ParameterType.Vector3;
			else if (value is Vector4)			Type = ParameterType.Vector4;
			else if (value is Color)			Type = ParameterType.Color;
			else {
				Type = ParameterType.Invalid;
				Debug.LogWarningFormat("ParameterValue warning: type {0} is not supported", value.GetType());
			}

		}

		public object Value {
			get {
				switch (Type) {
					case ParameterType.Integer:
						int intValue;
						if (System.Int32.TryParse(_serializedValue, out intValue)) return intValue;
						break;
					case ParameterType.Float:
						float floatvalue;
						if (System.Single.TryParse(_serializedValue, out floatvalue)) return floatvalue;
						break;
					case ParameterType.String:
						return _serializedValue;
					case ParameterType.Boolean:
						bool boolValue;
						if (System.Boolean.TryParse(_serializedValue, out boolValue)) return boolValue;
						break;
				}
				Debug.LogWarningFormat("ParameterValue warning: type {0} is not supported", Type);
				return null;
			}
			set {
				_serializedValue = value.ToString();
			}
		}
	}

}
