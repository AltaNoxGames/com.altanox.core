using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Nox.Core.Animation
{
	public class AnimatorControllerPostProcessor : AssetPostprocessor
	{
		public static event Action<string> AnimatorControllerSavedEvent;
		public static event Action<string, string> AnimatorControllerMovedEvent;

		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			foreach (string asset in importedAssets)
			{
				if (Path.GetExtension(asset).Equals(".controller"))
					AnimatorControllerSavedEvent?.Invoke(asset);
			}

			for (int i = 0; i < movedAssets.Length; i++)
			{
				string asset = movedAssets[i];
				if (Path.GetExtension(asset).Equals(".controller"))
					AnimatorControllerMovedEvent?.Invoke(asset,movedFromAssetPaths[i]);
			}
		}
	}
}

