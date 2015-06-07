using UnityEngine;
using UnityEditor;

namespace Forge.Editor {
	public class Node {

		public bool ShowType = true;

		public Operator Operator;

		// UI Dimensions
		public const float BaseWidth = 200f;
		public const float IOHeight = 28f;
		public const float TitleHeight = 28f;
		public const float TitleSeparator = 1f;

		// Calculated bounds
		public Rect TitleRect;
		public Rect NodeRect;
		private float _cachedScale = -1f;
		public void RecalculateBounds(float scale) {
			int ioCount = Mathf.Max(Operator.Inputs.Length, Operator.Outputs.Length);
			Vector2 pos = Operator.EditorPosition;

			TitleRect = new Rect(
				pos.x * scale,
				pos.y * scale,
				BaseWidth * scale, TitleHeight
			);

			float height = TitleHeight * scale + TitleSeparator;
			height += (ioCount * IOHeight) * scale;

			NodeRect = new Rect(
				pos.x * scale,
				pos.y * scale,
				BaseWidth * scale, height
			);

			_cachedScale = scale;
		}

		public Node(Operator op) {
			Operator = op;
		}

		public bool EventsNeedRepaint(float scale) {
			bool needsRepaint = false;

			if (scale != _cachedScale) RecalculateBounds(scale);

			var ev = Event.current;

			bool mouseInRect = NodeRect.Contains(ev.mousePosition);
			if (mouseInRect && !ShowType || !mouseInRect && ShowType) {
				ShowType = !ShowType;
				needsRepaint = true;
			}

			if (ev.type == EventType.MouseDown && ev.button == 0) {
				if (TitleRect.Contains(ev.mousePosition)) {
					GraphEditor.DragEventOwner = this;
				}
			}

			if (ev.type == EventType.MouseDrag && ev.button == 0 && GraphEditor.DragEventOwner == this) {
				Operator.EditorPosition.x += ev.delta.x;
				Operator.EditorPosition.y += ev.delta.y;
				RecalculateBounds(scale);
				needsRepaint = true;
			}

			return needsRepaint;

		}
	}
}