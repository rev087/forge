using UnityEngine;
using UnityEditor;
using Forge;
using Forge.Operators;
using Forge.Extensions;

namespace Forge.Editor.Renderers {

	// Draws the UI for the Node class
	public static class NodeRenderer {

		private static Color _TXTColor = new Color(0.182f, 0.194f, 0.202f);
		private static Color _TXTAltColor = new Color(0.332f, 0.344f, 0.352f);
		private static Color _BGColor = new Color(0.682f, 0.714f, 0.735f, 0.85f);
		private static Color _BGAltColor = new Color(0.612f, 0.639f, 0.661f, 0.85f);

		private static Texture2D _BGTex = null;
		private static Texture2D _BGAltTex = null;
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
			_titleStyle.normal.textColor = _TXTColor;
			_titleStyle.alignment = TextAnchor.MiddleCenter;

			_inputStyle = new GUIStyle();
			_inputStyle.normal.textColor = _TXTColor;
			_inputStyle.alignment = TextAnchor.MiddleLeft;

			_inputTypeStyle = new GUIStyle();
			_inputTypeStyle.normal.textColor = _TXTAltColor;
			_inputTypeStyle.alignment = TextAnchor.MiddleLeft;

			_outputStyle = new GUIStyle();
			_outputStyle.normal.textColor = _TXTColor;
			_outputStyle.alignment = TextAnchor.MiddleRight;

			_outputTypeStyle = new GUIStyle();
			_outputTypeStyle.normal.textColor = _TXTAltColor;
			_outputTypeStyle.alignment = TextAnchor.MiddleRight;

			if (_BGTex == null) {
				_BGTex = new Texture2D(1, 1);
				_BGTex.hideFlags = HideFlags.HideAndDontSave;
				_BGTex.SetPixel(0, 0, _BGColor);
				_BGTex.Apply();
			}

			if (_BGAltTex == null) {
				_BGAltTex = new Texture2D(1, 1);
				_BGAltTex.hideFlags = HideFlags.HideAndDontSave;
				_BGAltTex.SetPixel(0, 0, _BGAltColor);
				_BGAltTex.Apply();
			}

			// Outlets
			_outletRenderer = new OutletRenderer();
		}

		public static void Draw(this Node node, float scale) {

			if (GraphEditor.Selection.Contains(node)) {
				SelectionRenderer.DrawNodeSelection(node.NodeRect, scale);
			}

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
			string title = op is Parameter ? ((Parameter) op).Label : op.Metadata.Title;
			if (op.IsGeometryOutput) title += " *";
			GUI.DrawTexture(new Rect(x, y, width, titleHeight), _BGTex);
			GUI.Label(new Rect(x, y, width, titleHeight), title, _titleStyle);

			y += titleHeight + Node.TitleSeparator;

			// IO
			int ioCount = Mathf.Max(op is Parameter ? 0 : op.Inputs.Length, op.Outputs.Length);
			for (int i = 0; i < ioCount; i++) {
				Texture2D bg = (i % 2 == 0) ? _BGTex : _BGAltTex;
				GUI.DrawTexture(new Rect(x, y, width, ioHeight), bg);

				if (i < op.Inputs.Length && !(op is Parameter)) {
					_outletRenderer.Draw(x, y + ioHeight/2, scale, node.InputHover == i);
					if (node.ShowType) {
						GUI.Label(new Rect(x + ioMargin, y, width, ioHeight/2), op.Inputs[i].GetNiceName(), _inputStyle);
						GUI.Label(new Rect(x + ioMargin, y+ioHeight/2, width, ioHeight/2), op.Inputs[i].DataType.TypeAlias(), _inputTypeStyle);
					} else {
						GUI.Label(new Rect(x + ioMargin, y, width, ioHeight), op.Inputs[i].GetNiceName(), _inputStyle);
					}
				}

				if (i < op.Outputs.Length) {
					_outletRenderer.Draw(x + width, y + ioHeight/2, scale, node.OutputHover == i);
					if (node.ShowType) {
						GUI.Label(new Rect(x, y, width - ioMargin, ioHeight/2), op.Outputs[i].GetNiceName(), _outputStyle);
						GUI.Label(new Rect(x, y+ioHeight/2, width - ioMargin, ioHeight/2), op.Outputs[i].DataType.TypeAlias(), _outputTypeStyle);
					} else {
						GUI.Label(new Rect(x, y, width - ioMargin, ioHeight), op.Outputs[i].GetNiceName(), _outputStyle);
					}
				}

				y += ioHeight;
			}
		}

		public static string GetNiceName(this IOOutlet outlet) {
			return ObjectNames.NicifyVariableName(outlet.Member.Name);
		}

	}

}