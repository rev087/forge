using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Forge.Editor.Renderers {

	public class GridRenderer {

		private static Texture2D _gridTex = null;
		private float _cachedScale = 1f;

		private const float _TileSize = 120f;
		private const float _GridSize = 10f;
		
		// Generates a single tile of the grid texture
		public Texture2D Render(float scale) {
			int tileSize = Mathf.RoundToInt(_TileSize * scale);
			Vector2 step = new Vector2(tileSize/10, tileSize/10);

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
					else if (x % step.x == 0 && y % step.y == 0)
						gridTex.SetPixel(x, y, lightIntersection);

					// Finer grid color
					else if (x % step.x == 0 || y % step.y == 0)
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
			if (_gridTex == null || _cachedScale != scale) {
				_gridTex = Render(scale);
			}
			
			float yOffset = scrollPoint.y % _gridTex.height;
			float yStart = scrollPoint.y - yOffset;
			float yEnd = scrollPoint.y + canvas.height + yOffset;
			
			float xOffset = scrollPoint.x % _gridTex.width;
			float xStart = scrollPoint.x - xOffset;
			float xEnd = scrollPoint.x + canvas.width + xOffset;
			
			for (float x = xStart; x < xEnd; x += _gridTex.width) {
				for (float y = yStart; y < yEnd; y += _gridTex.height) {
					GUI.DrawTexture(new Rect(x, y, _gridTex.width, _gridTex.height), _gridTex);
				}
			}
		}

	}
	
}