using UnityEditor;
using UnityEngine;

namespace Nox.Core
{
	public class ALogSettingsWindow : EditorWindow
	{
		[MenuItem("Window/Nox/ALog Settings")]
		private static void ShowWindow()
		{
			ALogSettingsWindow window = GetWindow<ALogSettingsWindow>();

			window.titleContent = new GUIContent("ALog Settings");

			window.Show();
		}
	}
}