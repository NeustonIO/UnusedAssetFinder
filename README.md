# Unused Asset Finder

## Overview

Unused Asset Finder is a Unity package that helps you figure out which assets are unused and can safely be deleted from the project.

Unused Assets are assets that can't be reach in the asset dependency graph from the scenes in Editor Build Settings or from Resources.

## Getting started

- Open `Tools > Neuston > Unused Asset Finder`
- Click `Find Unused Assets`.
- See list of unused assets.
- Delete what you don't need.

## Configuration

Filter out assets you don't care about like this if you want:

```c#
public class UnusedAssetConfiguration : IUnusedAssetConfiguration
{
	public void FilterAssetPaths(List<string> assetPaths)
	{
		assetPaths.RemoveAll(path => path.StartsWith("Assets/Plugins/"));
	}
}
```