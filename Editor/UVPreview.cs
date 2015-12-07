using UnityEngine;
using UnityEditor;
using Forge.Editor.Renderers;
using System.Collections.Generic;

namespace Forge.Editor {

	public enum UEType { None, Unresolved, Click, Drag }
	public enum UEContext { None, Background, Vector }

	public class UVPReview : EditorWindow {

		public Vector2 ScrollPoint = Vector2.zero;
		public float Zoom = 1f;
		public Rect Canvas;

		public bool DisplayVertices = true;
		public bool DisplayVertexIndices = false;
		public bool DisplayEdges = true;
		public bool DisplayFaces = false;
		public int UVSet = 0;
		public int TexInput = 0;

		private static IconLoader IconLoader = null;
		private const float CanvasMargin = 20f;
		private const float CanvasMarginTop = 40f;
		private static Color BGColor = new Color(0.282f, 0.294f, 0.302f);
		private static Texture2D BGTex = null;
		private static GUIStyle FaceStyle = null;
		private static GUIStyle VertexStyle = null;
		private static GUIStyle ShadowStyle = null;

		private bool _isDragging = false;

		[MenuItem("Window/Forge/UV Preview")]
		public static void ShowEditor() {
			UVPReview editor = (UVPReview)EditorWindow.GetWindow(typeof(UVPReview));
			editor.titleContent = new GUIContent("Forge UV");
			editor.Show();
		}

		public static void OnTemplateChange(Template template) {
			//Geometry geo = asset.Geometry;
			var go = Selection.activeObject as GameObject;
			if (go != null) {
				var asset = go.GetComponent<ProceduralAsset>();
				if (asset != null) {
					Debug.Log(asset.Geometry);
				}
			}
		}

		public void OnSelectionChange() {
			Repaint();
		}

		public void OnEnable() {
			OnSelectionChange();
			Canvas = new Rect(0, 0, position.width * 2, position.height * 2);
			wantsMouseMove = true;
		}

		private GUIStyle MakeStyle(Color textColor, float offset) {
			var style = new GUIStyle();
			style.normal.textColor = textColor;
			style.fontSize = 14;
			style.contentOffset = new Vector2(offset, offset);
			style.alignment = TextAnchor.UpperCenter;
			style.fixedHeight = 20f;
			style.fixedWidth = 100f;
			return style;
		}

		private void DrawLabel(float x, float y, string label, GUIStyle style) {
			GUI.Label(new Rect(x - 50f, y + 2f, 0f, 0f), label, ShadowStyle);
			GUI.Label(new Rect(x - 50f, y + 2f, 0f, 0f), label, style);
		}

		void OnGUI() {
			// Initialize objects
			if (IconLoader == null) IconLoader = new IconLoader();
			if (BGTex == null) {
				BGTex = new Texture2D(1, 1);
				BGTex.hideFlags = HideFlags.HideAndDontSave;
				BGTex.SetPixel(0, 0, BGColor);
				BGTex.Apply();
			}
			if (VertexStyle == null) {
				VertexStyle = MakeStyle(Color.cyan, 0f);
				FaceStyle = MakeStyle(Color.red, 0f);
				ShadowStyle = MakeStyle(Color.black, 1f);
			}

			// Total canvas
			Canvas = new Rect(0f, 0f, position.width * 4 * Zoom, position.height * 4 * Zoom);

			// Scroll view
			Rect scrollViewRect = new Rect(0, 0, position.width, position.height);
			ScrollPoint = GUI.BeginScrollView(scrollViewRect, ScrollPoint, Canvas);
			bool needsRepaint = false;

			// Zoom with the mouse wheel
			if (Event.current.type == EventType.ScrollWheel) {
				Zoom += -Event.current.delta.y / 50;
				if (Zoom < 0.25f) Zoom = 0.25f;
				if (Zoom > 1f) Zoom = 1f;
				needsRepaint = true;
				Event.current.Use();
			}

			// Draw background
			GUI.DrawTexture(new Rect(ScrollPoint.x, ScrollPoint.y, scrollViewRect.width, scrollViewRect.height), BGTex);

			// Texture Inputs
			GameObject go = Selection.activeObject as GameObject;
			List<string> texInputsList = new List<string>();
			texInputsList.Add("-");
			Texture texPreview = null;
			if (go != null) {
				Renderer renderer = go.GetComponent<MeshRenderer>();
				if (renderer != null) {
					Material mat = renderer.sharedMaterial;
					for (int p = 0; p < ShaderUtil.GetPropertyCount(mat.shader); p++) {
						if (ShaderUtil.GetPropertyType(mat.shader, p) == ShaderUtil.ShaderPropertyType.TexEnv) {
							if (TexInput > 0 && TexInput == texInputsList.Count) {
								texPreview = mat.GetTexture(ShaderUtil.GetPropertyName(mat.shader, p));
							}
							texInputsList.Add(ShaderUtil.GetPropertyDescription(mat.shader, p));
						}
					}
				}
			}

			// Draw UV Canvas
			Rect uvCanvas = new Rect(CanvasMargin, CanvasMarginTop, position.width * 4 * Zoom - CanvasMargin * 2, position.height * 4 * Zoom - (CanvasMarginTop + CanvasMargin));
			if (texPreview) {
				GUI.DrawTexture(uvCanvas, texPreview);
			} else {
				Handles.color = new Color(0.1f, 0.1f, 0.1f);
				Handles.DrawPolyLine(new Vector3[] {
					new Vector3(uvCanvas.x, uvCanvas.y),
					new Vector3(uvCanvas.x + uvCanvas.width, uvCanvas.y),
					new Vector3(uvCanvas.x + uvCanvas.width, uvCanvas.y + uvCanvas.height),
					new Vector3(uvCanvas.x, uvCanvas.height + uvCanvas.y),
					new Vector3(uvCanvas.x, uvCanvas.y)
				});
			}

			// UI
			UVSet = EditorGUI.Popup(new Rect(10f, 12f, 100f, 30f), UVSet, new string[] { "uv0", "uv1" });
			DisplayVertices = GUI.Toggle(new Rect(120f, 10f, 30f, 20f), DisplayVertices, IconLoader.Icons["vertex"], "button");
			DisplayVertexIndices = GUI.Toggle(new Rect(155f, 10f, 30f, 20f), DisplayVertexIndices, IconLoader.Icons["vertexIndex"], "button");
			DisplayEdges = GUI.Toggle(new Rect(190f, 10f, 30f, 20f), DisplayEdges, IconLoader.Icons["face"], "button");
			DisplayFaces = GUI.Toggle(new Rect(225f, 10f, 30f, 20f), DisplayFaces, IconLoader.Icons["faceIndex"], "button");
			TexInput = EditorGUI.Popup(new Rect(260f, 12f, 100f, 30f), TexInput, texInputsList.ToArray());

			// Mesh data
			if (go != null) {
				var meshFilter = go.GetComponent<MeshFilter>();
				if (meshFilter != null) {

					Mesh mesh = meshFilter.sharedMesh;
					Vector2[] uvSet = UVSet == 0 ? mesh.uv : mesh.uv2;

					// Triangles
					for (int i = 0; i < mesh.triangles.Length; i += 3) {

						if (uvSet.Length == 0) {
							break;
						}

						Vector3[] verts = new Vector3[3];
						for (int n = 0; n < 3; n++) {
							verts[n] = new Vector3(
								uvCanvas.x + uvSet[mesh.triangles[i + n]].x * uvCanvas.width,
								uvCanvas.y + (1 - uvSet[mesh.triangles[i + n]].y) * uvCanvas.height,
								0f);
						}

						// Draw edges
						if (DisplayEdges) {
							Handles.color = Color.white;
							Handles.DrawPolyLine(new Vector3[] { verts[0], verts[1], verts[2], verts[0] });
						}

						// Draw face indices
						if (DisplayFaces) {
							Handles.color = Color.red;
							Vector3 mid = (verts[0] + verts[1] + verts[2]) / 3;
							mid.y -= 10f;
							DrawLabel(mid.x, mid.y, (i / 3).ToString(), FaceStyle);
						}
					}

					// Vertices
					for (int i = 0; i < uvSet.Length; i++) {
						Vector2 uv = uvSet[i];
						Vector3 point = new Vector3(uvCanvas.width * uv.x + uvCanvas.x, uvCanvas.height * (1 - uv.y) + uvCanvas.y, 0);

						// Draw vertex indices
						if (DisplayVertexIndices) {
							DrawLabel(point.x, point.y, i.ToString(), VertexStyle);
						}

						// Draw vertices
						if (DisplayVertices) {
							float thickness = 2f;
							Handles.color = Color.cyan;
							Handles.DrawSolidRectangleWithOutline(new Vector3[] {
								new Vector3(point.x - thickness, point.y - thickness, 0),
								new Vector3(point.x - thickness, point.y + thickness, 0),
								new Vector3(point.x + thickness, point.y + thickness, 0),
								new Vector3(point.x + thickness, point.y - thickness, 0)
							}, Color.white, Color.white);
						}
					}

				}
			}

			// Handle left mouse button events
			if (Event.current.button == 0) {

				// MouseDown
				if (Event.current.type == EventType.MouseDown && !_isDragging) {
					_isDragging = true;
				}

				// MouseDrag
				if (Event.current.type == EventType.MouseDrag && _isDragging) {
					if (Event.current.delta.magnitude > 0) {
						ScrollPoint.x += -Event.current.delta.x;
						ScrollPoint.y += -Event.current.delta.y;
						needsRepaint = true;
					}
				}

				// MouseUp
				if (Event.current.type == EventType.MouseUp) {
					_isDragging = false;
				}

			} // Left mouse down/drag/up

			// Right Click
			if (Event.current.button == 1 && Event.current.type == EventType.MouseUp) {
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent("WIP"), false, MenuAction, null);
				menu.ShowAsContext();
			}


			if (needsRepaint) {
				Repaint();
			}

			GUI.EndScrollView();

		} // OnGUI

		public void MenuAction(object userData) {
			Debug.Log("UVEditor MenuAction");
		}

	}

} //namespace