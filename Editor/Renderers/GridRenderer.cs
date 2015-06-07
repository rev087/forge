using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Forge.Editor.Renderers {

	public class GridRenderer {

		private static Texture2D _GridTex = null;
		private static float _CachedScale = 1f;

		private const float _TileSize = 120f;
		public const float StepSize = 10f;
		
		// Generates a single tile of the grid texture
		public Texture2D Render(float scale) {
			int tileSize = Mathf.RoundToInt(_TileSize * scale);
			int step = Mathf.RoundToInt((_TileSize * scale) / StepSize);

			Texture2D gridTex = new Texture2D(tileSize, tileSize);
			gridTex.hideFlags = HideFlags.DontSave;
			
			Color bg = new Color(0.282f, 0.294f, 0.302f);

			Color dark = Color.Lerp(bg, Color.black, 0.15f);
			Color darkIntersection = Color.Lerp(bg, Color.black, 0.2f);

			Color light = Color.Lerp(bg, Color.black, 0.05f);
			Color lightIntersection = Color.Lerp(bg, Color.black, 0.1f);
			
			for (int x = 0; x < tileSize; x ++) {

				for (int y = 0; y < tileSize; y ++) {
					
					// Left Top edge, dark intersection color
					if (x == 0 && y == 0)
						gridTex.SetPixel(x, y, darkIntersection);

					// Left and Top edges, dark color
					else if (x == 0 || y == 0)
						gridTex.SetPixel(x, y, dark);

					// Finer grid intersection color
					else if (x % step == 0 && y % step == 0)
						gridTex.SetPixel(x, y, lightIntersection);

					// Finer grid color
					else if (x % step == 0 || y % step == 0)
						gridTex.SetPixel(x, y, light);

					// Background
					else
						gridTex.SetPixel(x, y, bg);
				}

			}
			
			gridTex.Apply();
			return gridTex;
		}
		
		public void Draw(Vector2 scrollPoint, float scale, Rect canvas) {

			if (_GridTex == null || _CachedScale != scale) {
				_GridTex = Render(scale);
				_CachedScale = scale;
			}
			
			float yOffset = scrollPoint.y % _GridTex.height;
			float yStart = scrollPoint.y - yOffset;
			float yEnd = scrollPoint.y + canvas.height + yOffset;
			
			float xOffset = scrollPoint.x % _GridTex.width;
			float xStart = scrollPoint.x - xOffset;
			float xEnd = scrollPoint.x + canvas.width + xOffset;
			
			for (float x = xStart; x < xEnd; x += _GridTex.width) {
				for (float y = yStart; y < yEnd; y += _GridTex.height) {
					GUI.DrawTexture(new Rect(x, y, _GridTex.width, _GridTex.height), _GridTex);
				}
			}
		}

	}
	
}