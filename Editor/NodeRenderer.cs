using UnityEngine;
using UnityEditor;

namespace Forge.Editor {

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

		private const float _Width = 200f;
		private const float _TitleHeight = 28f;
		private const float _IOHeight = 24f;
		private const float _SeparationHeight = 2f;
		private const int _TitleFontSize = 16;
		private const int _IOFontSize = 12;
		private const float _IOMargin = 20f;

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

			_bg = new Texture2D(200, 50);
			_bg.hideFlags = HideFlags.HideAndDontSave;
			for (int x = 0; x < 200; x++) {
				for (int y = 0; y < 50; y++) {
					_bg.SetPixel(x, y, _bgColor);
				}
			}
			_bg.Apply();

			_bgAlt = new Texture2D(1, 1);
			_bgAlt.hideFlags = HideFlags.HideAndDontSave;
			_bgAlt.SetPixel(0, 0, _bgAltColor);
			_bgAlt.Apply();
		}

		public void Draw(Vector2 scrollPoint, float zoom) {

			float x = _pos.x * zoom, y = _pos.y * zoom;
			float width = _Width * zoom;
			float titleHeight = _TitleHeight * zoom;
			float ioHeight = _IOHeight * zoom;
			float ioMargin = _IOMargin * zoom;
			_titleStyle.fontSize = Mathf.CeilToInt(_TitleFontSize * zoom);
			_inputStyle.fontSize = Mathf.CeilToInt(_IOFontSize * zoom);
			_outputStyle.fontSize = Mathf.CeilToInt(_IOFontSize * zoom);

			// Title box
			GUI.DrawTexture(new Rect(x, y, width, titleHeight), _bg);
			GUI.Label(new Rect(x, y, width, titleHeight), Title, _titleStyle);

			y += titleHeight + _SeparationHeight;

			string[] inputs = new string[] {
				"Opening", "Orientation", "Surface", "Segments", "Center", "Radius", "Start Angle", "End Angle"
			};

			string[] outputs = new string[] {
				"Geometry"
			};

			// Params
			int ioCount = Mathf.Max(inputs.Length, outputs.Length);
			for (int i = 0; i < ioCount; i++) {
				Texture2D bg = (i % 2 == 0) ? _bg : _bgAlt;
				GUI.DrawTexture(new Rect(x, y, width, ioHeight), bg);

				if (i < inputs.Length)
					GUI.Label(new Rect(x + ioMargin, y, width, ioHeight), inputs[i], _inputStyle);
				if (i < outputs.Length)
					GUI.Label(new Rect(x, y, width - ioMargin, ioHeight), outputs[i], _outputStyle);

				y += ioHeight;
			}
		}

	}

}