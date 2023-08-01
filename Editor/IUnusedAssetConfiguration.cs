#nullable enable

using System;
using System.Collections.Generic;
using UnityEditor;

// ReSharper disable UnusedMember.Global
// ReSharper disable once IdentifierTypo

namespace Neuston.UnusedAssetFinder
{
	public interface IUnusedAssetConfiguration
	{
		public void FilterAssetPaths(List<string> assetPaths);
	}

	class UnusedAssetConfigurationSingleton
	{
		static IUnusedAssetConfiguration? instance;

		public static IUnusedAssetConfiguration Instance
		{
			get
			{
				if (instance != null)
				{
					return instance;
				}

				var types = TypeCache.GetTypesDerivedFrom<IUnusedAssetConfiguration>();

				if (types.Count == 0)
				{
					throw new UnusedAssetConfigurationException("No IUnusedAssetConfiguration found. Create a concrete implementation of IUnusedAssetConfiguration in your project.");
				}

				if (types.Count > 1)
				{
					throw new UnusedAssetConfigurationException("More than one IUnusedAssetConfiguration found. Create only one concrete implementation of IUnusedAssetConfiguration in your project.");
				}

				var type = types[0];

				instance = (IUnusedAssetConfiguration)Activator.CreateInstance(type);

				return instance;
			}
		}
	}

	public class UnusedAssetConfigurationException : Exception
	{
		public UnusedAssetConfigurationException()
		{
		}

		public UnusedAssetConfigurationException(string message) : base(message)
		{
		}

		public UnusedAssetConfigurationException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
