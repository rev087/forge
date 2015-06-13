using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Forge.Editor.Renderers {

	public class SelectionRenderer {

		public const float Margin = 2f;
		public const float Thickness = 2f;
		private static Texture2D _Texture = null;

		public static void DrawNodeSelection(Rect rect, float scale) {
			if (_Texture == null) _Texture = Texture2D.whiteTexture;

			float outletRadius = OutletRenderer.Radius * scale;

			// Left
			GUI.DrawTexture(new Rect(
				rect.x - Margin - Thickness - outletRadius,
				rect.y - Margin - Thickness,
				Thickness,
				rect.height + (Margin + Thickness) * 2
			), _Texture);

			// Top
			GUI.DrawTexture(new Rect(
				rect.x - Margin - outletRadius,
				rect.y - Margin - Thickness,
				rect.width + Margin*2 + outletRadius*2,
				Thickness
			), _Texture);

			// Right
			GUI.DrawTexture(new Rect(
				rect.x + rect.width + outletRadius + Margin,
				rect.y - Margin - Thickness,
				Thickness,
				rect.height + (Margin + Thickness) * 2
			), _Texture);

			// Bottom
			GUI.DrawTexture(new Rect(
				rect.x - Margin - outletRadius,
				rect.y + rect.height + Margin,
				rect.width + Margin*2 + outletRadius*2,
				Thickness
			), _Texture);

		} // Draw

	}

}