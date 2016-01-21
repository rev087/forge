using UnityEngine;

namespace Forge.Editor.Renderers {

	public class OutletRenderer {

		public const int Radius = 8;
		private const int Outline = 3;
		public Vector2 Center = Vector3.zero;

		private static Color _outlineColor = new Color(0.329f, 0.329f, 0.329f);
		private static Color _mainColor = new Color(0.682f, 0.714f, 0.735f);
		private static Color _mainActiveColor = new Color(0.882f, 0.914f, 0.935f);
		private static Color _clearColor = new Color(0.282f, 0.294f, 0.302f, 0f);

		private static Texture2D _tex;
		private static Texture2D _texActive;
		private static float _cachedScale = 1.0f;

		private static Color _AntiAlias(float xSquared, float ySquared, float radSquared, float radius, Color inside, Color outside) {
			float distance = Mathf.Abs(radSquared - (xSquared + ySquared));
			float factor = (radius - distance) / radius;
			return Color.Lerp(inside, outside, factor);
		}

		public Texture2D Render(bool active, float scale) {
			float radius = Radius * scale;
			int side = (int)radius * 2;
			var tex = new Texture2D(side, side);
			tex.hideFlags = HideFlags.HideAndDontSave;
			tex.filterMode = FilterMode.Point;

			// Top right quadrant
			for (int x = 0; x < side; x++) {
				for (int y = 0; y < side; y++) {
					float xSqr = Mathf.Pow(x - radius, 2);
					float ySqr = Mathf.Pow(y - radius, 2);

					float radOutlineSqr = Mathf.Pow(radius, 2);
					float radiusOutline = radius - Outline * scale;
					float radSqr = Mathf.Pow(radiusOutline, 2);

					if (xSqr + ySqr < radSqr) {
						if (active) {
							tex.SetPixel(x, y, _AntiAlias(xSqr, ySqr, radSqr, radius, _mainActiveColor, _outlineColor));
						} else {
							tex.SetPixel(x, y, _AntiAlias(xSqr, ySqr, radSqr, radius, _mainColor, _outlineColor));
						}
					} else if (xSqr + ySqr < radOutlineSqr) {
						tex.SetPixel(x, y, _AntiAlias(xSqr, ySqr, radOutlineSqr, radiusOutline, _outlineColor, _clearColor));
					} else {
						tex.SetPixel(x, y, _clearColor);
					}
				}
			}

			tex.Apply();
			return tex;
		}

		public void Draw(float x, float y, float scale, bool active) {
			if (_texActive == null || _tex == null || _cachedScale != scale) {
				_tex = Render(false, scale);
				_texActive = Render(true, scale);
				_cachedScale = scale;
			}

			x -= Radius * scale;
			y -= Radius * scale;
			float side = Radius * 2 * scale;

			if (active) {
				GUI.DrawTexture(new Rect(x, y, side, side), _texActive);
			} else {
				GUI.DrawTexture(new Rect(x, y, side, side), _tex);
			}
		}

	}

}