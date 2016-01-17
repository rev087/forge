﻿using UnityEngine;

namespace Forge {

	public enum ParameterType { Float, Integer, String, Boolean, Vector2, Vector3, Vector4, Color }

	[System.Serializable]
	public struct ParameterValue {
		public string GUID;
		[SerializeField]
		private string _serializedValue;
		public ParameterType Type;

		public object Value {
			get {
				switch (Type) {
					case ParameterType.Float:
						float floatvalue;
						if (System.Single.TryParse(_serializedValue, out floatvalue)) return floatvalue;
						break;
					case ParameterType.Integer:
						int intValue;
						if (System.Int32.TryParse(_serializedValue, out intValue)) return intValue;
						break;
					case ParameterType.String:
						return _serializedValue;
					case ParameterType.Boolean:
						bool boolValue;
						if (System.Boolean.TryParse(_serializedValue, out boolValue)) return boolValue;
						break;
				}
				Debug.LogWarningFormat("ParameterValue warning: type {0} not yet handled");
				return null;
			}
			set {
				_serializedValue = value.ToString();
			}
		}
	}

}