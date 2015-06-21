using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Forge.Extensions;

namespace Forge.Editor.Renderers {

	public static class Connections {

		private static GUIStyle _FloatingTextStyle = null;

		public static void DrawBezier(Vector2 inPoint, Vector2 outPoint) {
			float tanOffset = Vector2.Distance(inPoint, outPoint) / 4f;

			Vector2 inTan = new Vector2(inPoint.x + tanOffset, inPoint.y);
			Vector2 outTan = new Vector2(outPoint.x - tanOffset, outPoint.y);

			Handles.DrawBezier(inPoint, outPoint, inTan, outTan, Color.white, null, 2f);
		}

		public static void DrawConnections(this Template template) {

			if (_FloatingTextStyle == null) {
				_FloatingTextStyle = new GUIStyle();
				_FloatingTextStyle.alignment = TextAnchor.UpperRight;
				_FloatingTextStyle.normal.textColor = Color.white;
				_FloatingTextStyle.fontSize = 16;
			}

			// Connections
			foreach (IOConnection conn in template.Connections) {
				Node a = GraphEditor.GetNode(conn.From.GUID);
				Node b = GraphEditor.GetNode(conn.To.GUID);

				DrawBezier(a.OutputOutlet(conn.Output), b.InputOutlet(conn.Input));
			}

			// Open connections
			if (GraphEditor.CurrentEvent.Type == GEType.Drag) {

				Node node = GraphEditor.CurrentEvent.Node;
				IOOutlet outlet = GraphEditor.CurrentEvent.Outlet;
				Vector2 mousePos = Event.current.mousePosition;

				bool validConnection = false;

				// Dragging from Input
				if (GraphEditor.CurrentEvent.Context == GEContext.Input) {
					Vector2 inPoint = node.InputOutlet(outlet);
					DrawBezier(mousePos, inPoint);
					validConnection = true;
				}

				// Dragging from Output
				if (GraphEditor.CurrentEvent.Context == GEContext.Output) {
					Vector2 outPoint = node.OutputOutlet(outlet);
					DrawBezier(outPoint, mousePos);
					validConnection = true;
				}

				if (validConnection) {
					var point = mousePos + new Vector2(0f, 20f);
					string label = System.String.Format("{0}\n{1}", outlet.Name, outlet.DataType.TypeAlias(), _FloatingTextStyle);
					Handles.Label(point, label);
				}

			}

		}

	}

}