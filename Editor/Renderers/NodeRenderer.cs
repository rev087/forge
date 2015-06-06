using UnityEngine;
using UnityEditor;

namespace Forge.Editor.Renderers {

	public class NodeRenderer {

		public string Title = "Node";

		private Vector2 _pos = new Vector2(50f, 50f);
		private Color _txColor = new Color(0.282f, 0.294f, 0.302f);
		private Color _bgColor = new Color(0.682f, 0.714f, 0.735f);
		private Color _bgAltColor = new Color(0.612f, 0.639f, 0.661f);

		private Texture2D _bg;
		private Texture2D _bgAlt;
		private GUIStyle _titleStyle;
		private GUIStyle _inputStyle;
		private GUIStyle _outputStyle;

		// Node parameters
		private const float _Width = 200f;

		// Title parameters
		private const float _TitleHeight = 28f;
		private const int _TitleFontSize = 16;
		private const float _TitleSeparator = 1f;

		// IO Parameters
		private const float _IOHeight = 24f;
		private const float _IOMargin = 15f;
		private const int _IOFontSize = 12;

		private static OutletRenderer _outletRenderer = null;

		public NodeRenderer() {
			_titleStyle = new GUIStyle();
			_titleStyle.normal.textColor = _txColor;
			_titleStyle.alignment = TextAnchor.MiddleCenter;

			_inputStyle = new GUIStyle();
			_inputStyle.normal.textColor = _txColor;
			_inputStyle.alignment = TextAnchor.MiddleLeft;

			_outputStyle = new GUIStyle();
			_outputStyle.normal.textColor = _txColor;
			_outputStyle.alignment = TextAnchor.MiddleRight;

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
			_outputStyle.fontSize = Mathf.CeilToInt(_IOFontSize * scale);

			// Title box
			GUI.DrawTexture(new Rect(x, y, width, titleHeight), _bg);
			GUI.Label(new Rect(x, y, width, titleHeight), Title, _titleStyle);

			y += titleHeight + _TitleSeparator;

			string[] inputs = new string[] {
				"Opening", "Orientation", "Surface", "Segments", "Center", "Radius", "Start Angle", "End Angle"
			};

			string[] outputs = new string[] {
				"Geometry"
			};

			// IO
			int ioCount = Mathf.Max(inputs.Length, outputs.Length);
			for (int i = 0; i < ioCount; i++) {
				Texture2D bg = (i % 2 == 0) ? _bg : _bgAlt;
				GUI.DrawTexture(new Rect(x, y, width, ioHeight), bg);

				if (i < inputs.Length) {
					_outletRenderer.Draw(x, y + ioHeight/2, scale, false);
					GUI.Label(new Rect(x + ioMargin, y, width, ioHeight), inputs[i], _inputStyle);
				}

				if (i < outputs.Length) {
					_outletRenderer.Draw(x + width, y + ioHeight/2, scale, false);
					GUI.Label(new Rect(x, y, width - ioMargin, ioHeight), outputs[i], _outputStyle);
				}

				y += ioHeight;
			}
		}

	}

}