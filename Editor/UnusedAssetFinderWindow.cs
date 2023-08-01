#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// ReSharper disable once IdentifierTypo

namespace Neuston.UnusedAssetFinder
{
	public class UnusedAssetFinderWindow : EditorWindow
	{
		List<UnusedAsset> unusedAssets = new List<UnusedAsset>();
		Vector2 scrollPosition;
		int expectedProjectChanges;

		class UnusedAsset
		{
			public string AssetPath { get; }
			public bool IsChecked { set; get; }

			public UnusedAsset(string assetPath)
			{
				AssetPath = assetPath;
			}
		}

		[MenuItem("Tools/Neuston/Unused Asset Finder")]
		public static void FindReferences()
		{
			var window = GetWindow<UnusedAssetFinderWindow>();
			window.Start();
		}

		void Start()
		{
			titleContent.text = "Unused Asset Finder";
		}

		void OnGUI()
		{
			var wordWrapStyle = new GUIStyle(EditorStyles.textArea)
			{
				wordWrap = true
			};

			GUILayout.Label("Unused Asset Finder scans through the project and finds assets that are not referenced by the scenes in the Editor Build Settings or from Resources.", wordWrapStyle);

			if (GUILayout.Button("Find Unused Assets", GUILayout.Width(120)))
			{
				FindUnusedAssets();
			}

			GUILayout.Space(16);

			if (unusedAssets.Any())
			{
				DrawUnusedAssets();
			}
		}

		void DrawUnusedAssets()
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);

			foreach (var unusedAsset in unusedAssets)
			{
				GUILayout.BeginHorizontal();

				var height = GUILayout.Height(18);

				// Checkbox
				unusedAsset.IsChecked = GUILayout.Toggle(unusedAsset.IsChecked, string.Empty, GUILayout.Width(16), height);

				// Object
				var assetPath = unusedAsset.AssetPath;
				var type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
				var guiContent = EditorGUIUtility.ObjectContent(null, type);
				string fileName = Path.GetFileName(assetPath);
				guiContent.text = fileName;
				guiContent.tooltip = fileName;
				var before = GUI.skin.button.alignment;
				GUI.skin.button.alignment = TextAnchor.MiddleLeft;
				if (GUILayout.Button(guiContent, GUILayout.Width(240), height))
				{
					EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(assetPath));
				}
				GUI.skin.button.alignment = before;

				// Path
				GUILayout.Label(assetPath, height);

				GUILayout.EndHorizontal();
			}

			if (GUILayout.Button("Delete Selected", GUILayout.Width(200)))
			{
				var assetsToDelete = unusedAssets.Where(a => a.IsChecked).Select(a => a.AssetPath).ToList();
				foreach (string assetPath in assetsToDelete)
				{
					expectedProjectChanges++;
					AssetDatabase.DeleteAsset(assetPath);
					RemoveDeletedAssetFromState(assetPath);
					Debug.Log($"Deleted {assetPath}");
				}
			}

			GUILayout.EndScrollView();
		}

		void RemoveDeletedAssetFromState(string deletedAssetPath)
		{
			unusedAssets.RemoveAll(a => a.AssetPath == deletedAssetPath);
		}

		void FindUnusedAssets()
		{
			unusedAssets = UnusedAssetFinder.FindUnusedAssets().Select(p => new UnusedAsset(p)).ToList();
		}

		void OnProjectChange()
		{
			// After deleting files from this tool, we predict one project change for each deleted asset.
			if (expectedProjectChanges > 0)
			{
				expectedProjectChanges--;
			}
			else
			{
				// If we get a project change that we didn't expect (by our own delete op) we clear the state.
				ClearState();
			}
		}

		void OnDestroy()
		{
			// If we close the window we better clear the state.
			ClearState();
		}

		void ClearState()
		{
			unusedAssets.Clear();
			expectedProjectChanges = 0;
		}
	}
}
