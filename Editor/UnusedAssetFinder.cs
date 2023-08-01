#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace Neuston.UnusedAssetFinder
{
	public class UnusedAssetFinder
	{
		public static IEnumerable<string> FindUnusedAssets()
		{
			var scenesInBuildSettings = EditorBuildSettings.scenes
				.Where(scene => scene.enabled)
				.Select(scene => scene.path)
				.ToList();

			var assetDependencyGraph = new AssetDependencyGraph();

			var allAssetPaths = AssetDatabase.GetAllAssetPaths().ToList();
			allAssetPaths.Sort();
			FilterAssetPaths(allAssetPaths);

			for (var i = 0; i < allAssetPaths.Count; i++)
			{
				string assetPath = allAssetPaths[i];

				EditorUtility.DisplayProgressBar("Asset Tools", "Finding unused assets...", (float)i / allAssetPaths.Count);

				var absoluteAssetPath = Application.dataPath + assetPath.Substring("Assets".Length);
				if (Directory.Exists(absoluteAssetPath))
				{
					continue;
				}

				// "Used" by Resources?
				if (assetPath.Contains("/Resources/"))
				{
					continue;
				}

				// "Used" by Editor Build Settings?
				if (assetPath.EndsWith(".unity") && scenesInBuildSettings.Contains(assetPath))
				{
					continue;
				}

				// Used by other assets?
				var dependants = assetDependencyGraph.GetAllAssetsThatDependOn(assetPath);
				if (dependants.Any())
				{
					continue;
				}

				// We now conclude that this is an unused asset.
				yield return assetPath;
			}

			EditorUtility.ClearProgressBar();
		}

		static void FilterAssetPaths(List<string> assetPaths)
		{
			assetPaths.RemoveAll(path => path.StartsWith("ProjectSettings/"));
			assetPaths.RemoveAll(path => path.StartsWith("Library/"));
			assetPaths.RemoveAll(path => path.StartsWith("Packages/"));
			assetPaths.RemoveAll(path => path.StartsWith("Assets/link.xml"));
			assetPaths.RemoveAll(path => path.StartsWith("Assets/StreamingAssets/"));
			assetPaths.RemoveAll(path => path.EndsWith(".cs"));
			assetPaths.RemoveAll(path => path.EndsWith(".shader"));
			assetPaths.RemoveAll(path => path.EndsWith(".hlsl"));
			assetPaths.RemoveAll(path => path.EndsWith(".cginc"));
			assetPaths.RemoveAll(path => path.EndsWith(".asmdef"));
			assetPaths.RemoveAll(path => path.EndsWith(".preset"));

			// Run the project-specific filter.
			UnusedAssetConfigurationSingleton.Instance.FilterAssetPaths(assetPaths);
		}
	}
}
