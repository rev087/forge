using UnityEngine;
using UnityEditor;
using Forge;
using Forge.Primitives;
using Forge.Filters;

namespace Forge.Editor.Renderers {

	public class NodeRenderer {

		private Vector2 _pos = new Vector2(50f, 50f);
		private Color _txColor = new Color(0.182f, 0.194f, 0.202f);
		private Color _txAltColor = new Color(0.332f, 0.344f, 0.352f);
		private Color _bgColor = new Color(0.682f, 0.714f, 0.735f);
		private Color _bgAltColor = new Color(0.612f, 0.639f, 0.661f);

		private Texture2D _bg;
		private Texture2D _bgAlt;
		private GUIStyle _titleStyle;
		private GUIStyle _inputStyle;
		private GUIStyle _inputTypeStyle;
		private GUIStyle _outputStyle;
		private GUIStyle _outputTypeStyle;

		// Interaction
		public bool ShowType = true;
		public Node Node = (Node) new Cuboid();

		// Node parameters
		private const float _Width = 200f;

		// Title parameters
		private const float _TitleHeight = 28f;
		private const int _TitleFontSize = 16;
		private const float _TitleSeparator = 1f;

		// IO Parameters
		private const float _IOHeight = 28f;
		private const float _IOMargin = 15f;
		private const int _IOFontSize = 12;
		private const int _IOAltFontSize = 10;

		private static OutletRenderer _outletRenderer = null;

		public NodeRenderer() {
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

		public void Draw(Vector2 scrollPoint, float scale) {

			float x = _pos.x * scale, y = _pos.y * scale;
			float width = _Width * scale;
			float titleHeight = _TitleHeight * scale;
			float ioHeight = _IOHeight * scale;
			float ioMargin = _IOMargin * scale;
			_titleStyle.fontSize = Mathf.CeilToInt(_TitleFontSize * scale);
			_inputStyle.fontSize = Mathf.CeilToInt(_IOFontSize * scale);
			_inputTypeStyle.fontSize = Mathf.CeilToInt(_IOAltFontSize * scale);
			_outputStyle.fontSize = Mathf.CeilToInt(_IOFontSize * scale);
			_outputTypeStyle.fontSize = Mathf.CeilToInt(_IOAltFontSize * scale);

			// Title box
			GUI.DrawTexture(new Rect(x, y, width, titleHeight), _bg);
			GUI.Label(new Rect(x, y, width, titleHeight), Node.NodeName, _titleStyle);

			y += titleHeight + _TitleSeparator;

			// IO
			int ioCount = Mathf.Max(Node.Inputs.Length, Node.Outputs.Length);
			for (int i = 0; i < ioCount; i++) {
				Texture2D bg = (i % 2 == 0) ? _bg : _bgAlt;
				GUI.DrawTexture(new Rect(x, y, width, ioHeight), bg);

				if (i < Node.Inputs.Length) {
					_outletRenderer.Draw(x, y + ioHeight/2, scale, false);
					if (ShowType) {
						GUI.Label(new Rect(x + ioMargin, y, width, ioHeight/2), Node.Inputs[i].Name, _inputStyle);
						GUI.Label(new Rect(x + ioMargin, y+ioHeight/2, width, ioHeight/2), Node.Inputs[i].Type, _inputTypeStyle);
					} else {
						GUI.Label(new Rect(x + ioMargin, y, width, ioHeight), Node.Inputs[i].Name, _inputStyle);
					}
				}

				if (i < Node.Outputs.Length) {
					_outletRenderer.Draw(x + width, y + ioHeight/2, scale, false);
					if (ShowType) {
						GUI.Label(new Rect(x, y, width - ioMargin, ioHeight/2), Node.Outputs[i].Name, _outputStyle);
						GUI.Label(new Rect(x, y+ioHeight/2, width - ioMargin, ioHeight/2), Node.Outputs[i].Type, _outputTypeStyle);
					} else {
						GUI.Label(new Rect(x, y, width - ioMargin, ioHeight), Node.Outputs[i].Name, _outputStyle);
					}
				}

				y += ioHeight;
			}
		}

	}

}