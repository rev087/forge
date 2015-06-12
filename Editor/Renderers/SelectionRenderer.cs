using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Forge.Editor.Renderers {

	public class SelectionRenderer {

		private const float _Margin = 2f;
		private const float _Thickness = 1f;

		public static void Draw(Rect rect) {
			rect.width += (_Margin + _Thickness) * 2;
			rect.height += (_Margin + _Thickness) * 2;
			rect.x -= _Margin + _Thickness;
			rect.y -= _Margin + _Thickness;

			var a = new Vector2(rect.x, rect.y);
			var b = new Vector2(rect.x + rect.width, rect.y);
			var c = new Vector2(rect.x + rect.width, rect.y + rect.height);
			var d = new Vector2(rect.x, rect.y + rect.height);

			Handles.color = Color.white;
			Handles.DrawPolyLine(a, b, c, d, a);
		}

	}

}