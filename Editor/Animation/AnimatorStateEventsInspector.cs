using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nox.Core.Animation
{
	[CustomEditor(typeof(AnimationEvents))]
	public class AnimatorStateEventsInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
		}
	}
}
