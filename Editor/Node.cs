using UnityEngine;
using UnityEditor;
using Forge.Editor.Renderers;

namespace Forge.Editor {
	public class Node {

		public bool ShowType = false;
		public bool Selected = true;

		public Operator Operator;

		// UI Dimensions
		public const float BaseWidth = 200f;
		public const float IOHeight = 28f;
		public const float TitleHeight = 28f;
		public const float TitleSeparator = 1f;

		// State
		public int InputHover = -1;
		public int OutputHover = -1;

		// Calculated bounds
		public Rect NodeRect;
		public Rect InputsRect;
		public Rect OutputsRect;
		private float _cachedScale = -1f;

		public void RecalculateBounds(float scale) {
			int ioCount = Mathf.Max(Operator.Inputs.Length, Operator.Outputs.Length);
			Vector2 pos = Operator.EditorPosition;

			float titleHeight = TitleHeight * scale + TitleSeparator;
			float bodyHeight = (ioCount * IOHeight) * scale;
			float nodeHeight = titleHeight + bodyHeight;

			// NodeRect represents the clickable area of the node, excluding outlets
			NodeRect = new Rect(
				(pos.x + OutletRenderer.Radius) * scale,
				pos.y * scale,
				(BaseWidth - OutletRenderer.Radius * 2) * scale,
				nodeHeight
			);

			InputsRect = new Rect(
				(pos.x - OutletRenderer.Radius) * scale,
				titleHeight + pos.y * scale,
				(OutletRenderer.Radius * 2) * scale,
				bodyHeight
			);

			OutputsRect = new Rect(
				(pos.x + BaseWidth - OutletRenderer.Radius) * scale,
				titleHeight + pos.y * scale,
				(OutletRenderer.Radius * 2) * scale,
				bodyHeight
			);

			_cachedScale = scale;
		}

		public Node(Operator op) {
			Operator = op;
		}

		public bool EventsNeedRepaint(float scale, GraphEditor graphEditor) {
			bool needsRepaint = false, showType = false;
			int inputHover = -1, outputHover = -1;

			if (scale != _cachedScale) RecalculateBounds(scale);

			Event ev = Event.current;

			bool mouseInNode = NodeRect.Contains(ev.mousePosition);
			bool mouseInInputs = InputsRect.Contains(ev.mousePosition);
			bool mouseInOutputs = OutputsRect.Contains(ev.mousePosition);

			// MouseOver node
			if (mouseInNode || mouseInInputs || mouseInOutputs) {
				showType = true;
			}

			// MouseOver inputs
			int inputIndex = Mathf.FloorToInt((ev.mousePosition.y - InputsRect.y) / IOHeight);
			if (mouseInInputs) {
				inputHover = inputIndex;
			}

			// MouseOver outputs
			int outputIndex = Mathf.FloorToInt((ev.mousePosition.y - OutputsRect.y) / IOHeight);
			if (mouseInOutputs) {
				outputHover = outputIndex;
			}

			// Left mouse button actions
			if (ev.button == 0) {

				// MouseDown outlets
				if (ev.type == EventType.MouseDown) {

					// Input
					if (mouseInInputs && inputIndex < Operator.Inputs.Length) {
						GraphEditor.CurrentEvent = new GraphEvent(GEType.Unresolved, GEContext.Input, this, Operator.Inputs[inputIndex]);
					}

					// Output
					if (mouseInOutputs && outputIndex < Operator.Outputs.Length) {
						GraphEditor.CurrentEvent = new GraphEvent(GEType.Unresolved, GEContext.Output, this, Operator.Outputs[outputIndex]);
					}
				}

				// MouseUp outlets
				if (ev.type == EventType.MouseUp) {

					// Releasing Output on Input
					if (mouseInInputs && GraphEditor.CurrentEvent.Type == GEType.Drag && GraphEditor.CurrentEvent.Context == GEContext.Output) {
						if (IOOutlet.CanConnect(GraphEditor.CurrentEvent.Outlet, Operator.Inputs[inputIndex])) {
							GraphEditor.Template.Connect(
								GraphEditor.CurrentEvent.Node.Operator, GraphEditor.CurrentEvent.Outlet,
								Operator, Operator.Inputs[inputIndex]
							);
						}
						GraphEditor.CurrentEvent.Empty();
						needsRepaint = true;
					}

					// Releasing Input on Output
					if (mouseInOutputs && GraphEditor.CurrentEvent.Type == GEType.Drag && GraphEditor.CurrentEvent.Context == GEContext.Input) {
						if (IOOutlet.CanConnect(Operator.Outputs[inputIndex], GraphEditor.CurrentEvent.Outlet)) {
							GraphEditor.Template.Connect(
								Operator, Operator.Outputs[inputIndex],
								GraphEditor.CurrentEvent.Node.Operator, GraphEditor.CurrentEvent.Outlet
							);
						}
						GraphEditor.CurrentEvent.Empty();
						needsRepaint = true;
					}
				}
				
				// Mouse cursor over node
				if (NodeRect.Contains(ev.mousePosition)) {

					// Mouse Down
					if (ev.type == EventType.MouseDown) {
						GraphEditor.CurrentEvent = new GraphEvent(GEType.Unresolved, GEContext.Node, this, IOOutlet.None);
					}

					// Mouse Up
					if (ev.type == EventType.MouseUp && GraphEditor.CurrentEvent.Type == GEType.Unresolved) {
						if (!GraphEditor.Selection.Contains(this)) {
							if (Event.current.modifiers == EventModifiers.Shift) {
								GraphEditor.Selection.Add(this);
							} else {
								GraphEditor.Selection.Clear();
								GraphEditor.Selection.Add(this);
							}
						}
						needsRepaint = true;
					}
					else if (GraphEditor.CurrentEvent.IsNodeDrag(this)) {
						GraphEditor.Template.Serialize();
					}

				}

				// MouseDrag
				if (ev.type == EventType.MouseDrag) {

					// Drag node
					if (GraphEditor.CurrentEvent.IsNodeDrag(this)) {
						Operator.EditorPosition.x += ev.delta.x / scale;
						Operator.EditorPosition.y += ev.delta.y / scale;
						RecalculateBounds(scale);
						GraphEditor.CurrentEvent.Type = GEType.Drag;
						needsRepaint = true;
					}

					// Drag input
					else if (GraphEditor.CurrentEvent.CanDragOutlet(this, GEContext.Input)) {
						GraphEditor.CurrentEvent = new GraphEvent(GEType.Drag, GEContext.Input, this, GraphEditor.CurrentEvent.Outlet);
						needsRepaint = true;
					}

					// Drag output
					else if (GraphEditor.CurrentEvent.CanDragOutlet(this, GEContext.Output)) {
						GraphEditor.CurrentEvent = new GraphEvent(GEType.Drag, GEContext.Output, this, GraphEditor.CurrentEvent.Outlet);
						needsRepaint = true;
					}

				}
			}

			if (showType != ShowType || inputHover != InputHover || outputHover != OutputHover) {
				ShowType = showType;
				InputHover = inputHover;
				OutputHover = outputHover;
				needsRepaint = true;
			}

			return needsRepaint;

		}

		public Vector2 OutputOutlet(IOOutlet target) {
			for (int i = 0; i < Operator.Outputs.Length; i++) {
				if (Operator.Outputs[i].Name == target.Name) {
					float height = TitleHeight * _cachedScale + TitleSeparator;
					height += (i * IOHeight) * _cachedScale;
					height += (IOHeight * _cachedScale) / 2;
					return new Vector2(
						NodeRect.x + NodeRect.width,
						NodeRect.y + height
					);
				}
			}
			return new Vector2(NodeRect.x, NodeRect.y);
		}

		public Vector2 InputOutlet(IOOutlet target) {
			for (int i = 0; i < Operator.Inputs.Length; i++) {
				if (Operator.Inputs[i].Name == target.Name) {
					float height = TitleHeight * _cachedScale + TitleSeparator;
					height += (i * IOHeight) * _cachedScale;
					height += (IOHeight * _cachedScale) / 2;

					return new Vector2(
						NodeRect.x,
						NodeRect.y + height
					);
				}
			}
			return new Vector2(NodeRect.x, NodeRect.y);
		}

	}
}