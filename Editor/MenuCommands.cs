using UnityEngine;
using UnityEditor;
using System.IO;

namespace Forge.Editor {

	public static class MenuCommands {

		[MenuItem("Assets/Create/Forge Template", false, 1)]
		static void CreateNewTemplate(MenuCommand menuCommand) {
			string path = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (path == "")
				path = "Assets";
			else if (Path.GetExtension (path) != "")
				path = path.Replace (Path.GetFileName(AssetDatabase.GetAssetPath (Selection.activeObject)), "");
			
			string fullPath = AssetDatabase.GenerateUniqueAssetPath (path + "/New Forge Template.asset");
			
			Template template = ScriptableObject.CreateInstance<Template>();

			AssetDatabase.CreateAsset (template, fullPath);
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = template;
		}

	}

}