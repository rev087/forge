using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Forge.Editor.Renderers {

	public static class Connections {

		public static void DrawConnections(this Template template, Dictionary<string, Node> nodes) {
			foreach (IOConnection conn in template.Connections) {
				Node a = nodes[conn.From.Guid];
				Node b = nodes[conn.To.Guid];

				Vector2 aPoint = a.OutputOutlet(conn.Output);
				Vector2 bPoint = b.InputOutlet(conn.Input);

				float tanOffset = Vector2.Distance(aPoint, bPoint) / 4f;

				Vector2 aTan = new Vector2(aPoint.x + tanOffset, aPoint.y);
				Vector2 bTan = new Vector2(bPoint.x - tanOffset, bPoint.y);

				Handles.DrawBezier(aPoint, bPoint, aTan, bTan, Color.white, null, 2f);
			}
		}

	}

}