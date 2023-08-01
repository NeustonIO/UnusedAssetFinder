# Unused Asset Finder

## Overview

Unused Asset Finder is a Unity package that helps you figure out which assets are unused and can safely be deleted from the project.

## Getting started

- Open `Tools > Neuston > Unused Asset Finder`
- Click `Find Unused Assets`.
- See list of unused assets.
- Delete what you don't need.

## Notes

Assets in `Resources` folders are considered used and therefore excluded.

Filter out more assets you want to exclude like this: 

```c#
public class UnusedAssetConfiguration : IUnusedAssetConfiguration
{
	public void FilterAssetPaths(List<string> assetPaths)
	{
		assetPaths.RemoveAll(path => path.StartsWith("Assets/Plugins/"));
	}
}
```