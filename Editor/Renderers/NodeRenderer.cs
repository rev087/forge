using UnityEngine;
using UnityEditor;
using Forge;
using Forge.Primitives;
using Forge.Filters;

namespace Forge.Editor.Renderers {

	// Draws the UI for the Node class
	public static class NodeRenderer {

		private static Color _txColor = new Color(0.182f, 0.194f, 0.202f);
		private static Color _txAltColor = new Color(0.332f, 0.344f, 0.352f);
		private static Color _bgColor = new Color(0.682f, 0.714f, 0.735f);
		private static Color _bgAltColor = new Color(0.612f, 0.639f, 0.661f);

		private static Texture2D _bg;
		private static Texture2D _bgAlt;
		private static GUIStyle _titleStyle;
		private static GUIStyle _inputStyle;
		private static GUIStyle _inputTypeStyle;
		private static GUIStyle _outputStyle;
		private static GUIStyle _outputTypeStyle;

		// Title parameters
		private const int _TitleFontSize = 16;

		// IO Parameters
		private const float _IOMargin = 15f;
		private const int _IOFontSize = 12;
		private const int _IOAltFontSize = 10;

		private static OutletRenderer _outletRenderer = null;

		private static bool _HasInitializedStyles = false;
		public static void InitializeStyles() {
			_titleStyle = new GUIStyle();
			_titleStyle.normal.textColor = _txColor;
			_titleStyle.alignment = TextAnchor.MiddleCenter;

			_inputStyle = new GUIStyle();
			_inputStyle.normal.textColor = _txColor;
			_inputStyle.alignment = TextAnchor.MiddleLeft;

			_inputTypeStyle = new GUIStyle();
			_inputTypeStyle.normal.textColor = _txAltColor;
			_inputTypeStyle.alignment = TextAnchor.MiddleLeft;

			_outputStyle = new GUIStyle();
			_outputStyle.normal.textColor = _txColor;
			_outputStyle.alignment = TextAnchor.MiddleRight;

			_outputTypeStyle = new GUIStyle();
			_outputTypeStyle.normal.textColor = _txAltColor;
			_outputTypeStyle.alignment = TextAnchor.MiddleRight;

			_bg = new Texture2D(1, 1);
			_bg.hideFlags = HideFlags.HideAndDontSave;
			_bg.SetPixel(0, 0, _bgColor);
			_bg.Apply();

			_bgAlt = new Texture2D(1, 1);
			_bgAlt.hideFlags = HideFlags.HideAndDontSave;
			_bgAlt.SetPixel(0, 0, _bgAltColor);
			_bgAlt.Apply();

			// Outlets
			_outletRenderer = new OutletRenderer();
		}

		public static void Draw(this Node node, float scale) {

			if (!_HasInitializedStyles) InitializeStyles();

			var op = node.Operator;

			float x = op.EditorPosition.x * scale, y = op.EditorPosition.y * scale;
			float width = Node.BaseWidth * scale;
			float titleHeight = Node.TitleHeight * scale;
			float ioHeight = Node.IOHeight * scale;
			float ioMargin = _IOMargin * scale;
			_titleStyle.fontSize = Mathf.CeilToInt(_TitleFontSize * scale);
			_inputStyle.fontSize = Mathf.CeilToInt(_IOFontSize * scale);
			_inputTypeStyle.fontSize = Mathf.CeilToInt(_IOAltFontSize * scale);
			_outputStyle.fontSize = Mathf.CeilToInt(_IOFontSize * scale);
			_outputTypeStyle.fontSize = Mathf.CeilToInt(_IOAltFontSize * scale);

			// Title box
			GUI.DrawTexture(new Rect(x, y, width, titleHeight), _bg);
			GUI.Label(new Rect(x, y, width, titleHeight), op.Title, _titleStyle);

			y += titleHeight + Node.TitleSeparator;

			// IO
			int ioCount = Mathf.Max(op.Inputs.Length, op.Outputs.Length);
			for (int i = 0; i < ioCount; i++) {
				Texture2D bg = (i % 2 == 0) ? _bg : _bgAlt;
				GUI.DrawTexture(new Rect(x, y, width, ioHeight), bg);

				if (i < op.Inputs.Length) {
					_outletRenderer.Draw(x, y + ioHeight/2, scale, node.InputHover == i);
					if (node.ShowType) {
						GUI.Label(new Rect(x + ioMargin, y, width, ioHeight/2), op.Inputs[i].Name, _inputStyle);
						GUI.Label(new Rect(x + ioMargin, y+ioHeight/2, width, ioHeight/2), TypeAlias(op.Inputs[i].Type), _inputTypeStyle);
					} else {
						GUI.Label(new Rect(x + ioMargin, y, width, ioHeight), op.Inputs[i].Name, _inputStyle);
					}
				}

				if (i < op.Outputs.Length) {
					_outletRenderer.Draw(x + width, y + ioHeight/2, scale, node.OutputHover == i);
					if (node.ShowType) {
						GUI.Label(new Rect(x, y, width - ioMargin, ioHeight/2), op.Outputs[i].Name, _outputStyle);
						GUI.Label(new Rect(x, y+ioHeight/2, width - ioMargin, ioHeight/2), TypeAlias(op.Outputs[i].Type), _outputTypeStyle);
					} else {
						GUI.Label(new Rect(x, y, width - ioMargin, ioHeight), op.Outputs[i].Name, _outputStyle);
					}
				}

				y += ioHeight;
			}
		}

		public static string TypeAlias(System.Type type) {
			// See https://msdn.microsoft.com/en-us/library/ya5y69ds

			if (type == typeof(System.Boolean)) return "bool";
			else if (type == typeof(System.String)) return "string";
			else if (type == typeof(System.Int32)) return "int";			
			else if (type == typeof(System.Single)) return "float";
			else if (type == typeof(System.Double)) return "double";

			else if (type == typeof(System.Byte)) return "byte";
			else if (type == typeof(System.SByte)) return "sbyte";
			else if (type == typeof(System.Char)) return "char";
			else if (type == typeof(System.Decimal)) return "decimal";
			else if (type == typeof(System.UInt32)) return "uint";
			else if (type == typeof(System.Int64)) return "long";
			else if (type == typeof(System.UInt64)) return "ulong";
			else if (type == typeof(System.Int16)) return "short";
			else if (type == typeof(System.UInt16)) return "ushort";

			return type.Name;
		}

	}

}