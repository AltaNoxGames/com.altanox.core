using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nox.Core.Animation
{
	[ExecuteAlways]
	public class AnimationEvents : StateMachineBehaviour
	{
		[Serializable]
		public class AnimationEventParameters
		{
			public GameObject gameObject;
		}

		[Tooltip("Useful for identifying multiple AnimationEvents in a single animator")]
		public List<string> tags;

		[Tooltip("Normalized animation time")]
		[Range(0, 1f)]
		public float eventTime;
		public AnimationEventParameters parameters;


		private bool eventFlag;

		public event Action<AnimationEvents> Event;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			eventFlag = false;
			if (eventTime <= 0)
				Event?.Invoke(this);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (eventTime <= 0 || eventTime >= 1)
				return;

			if (!eventFlag && stateInfo.normalizedTime >= eventTime)
			{
				eventFlag = true;
				Event?.Invoke(this);
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (eventTime >= 1)
				Event?.Invoke(this);
		}
	}
}

