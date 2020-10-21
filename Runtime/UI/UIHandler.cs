using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nox.Core
{
	public class UIHandler : MonoBehaviour
	{
		private void Awake()
		{
			screenStack = new Stack<UILayer>();
			availableScreens = new Dictionary<Type, UILayer>();
			UILayer[] screens = GetComponentsInChildren<UILayer>(true);
			for (int i = 0; i < screens.Length; i++)
			{
				Type screenType = screens[i].GetType();
				availableScreens.Add(screenType, screens[i]);
				screens[i].gameObject.SetActive(false);
				screens[i].handler = this;
			}

			foreach (UILayer s in availableScreens.Values)
				s.OnScreenStart();
		}

		private Dictionary<Type, UILayer> availableScreens;
		private Stack<UILayer> screenStack;

		public Type GetCurrentScreenType()
		{
			return screenStack.Count > 0 ? screenStack.Peek().GetType() : null;
		}

		public void Show(Type screenType, float upDelay = 0)
		{
			UILayer newScreen = availableScreens[screenType];
			//Check if the stack is not empty
			if (screenStack.Count != 0 && newScreen.hidePrevious)
				TakeScreenDown(screenStack.Peek());
			screenStack.Push(newScreen);
			StartCoroutine(DelayedUp(newScreen, upDelay));
		}

		public void Show<T>(float upDelay = 0) where T : UILayer
		{
			Type screenType = typeof(T);
			Show(screenType, upDelay);
		}

		public void ImmediateShow(Type screenType)
		{
			UILayer newScreen = availableScreens[screenType];
			if (screenStack.Count != 0 && newScreen.hidePrevious)
				TakeScreenDown(screenStack.Peek());
			screenStack.Push(newScreen);
			PutScreenUp(newScreen);
		}

		public void ImmediateShow<T>() where T : UILayer
		{
			Type screenType = typeof(T);
			ImmediateShow(screenType);
		}

		public void Hide(float downDelay = 0, float upDelay = 0)
		{
			StartCoroutine(DelayedDown(screenStack.Pop(), downDelay));
			if (screenStack.Count == 0) return;
			StartCoroutine(DelayedUp(screenStack.Peek(), upDelay));
		}

		public void ImmediateHide()
		{
			TakeScreenDown(screenStack.Pop());
			if (screenStack.Count == 0) return;
			PutScreenUp(screenStack.Peek());
		}

		public bool ScreenIsUp(Type screenType)
		{
			return availableScreens[screenType].gameObject.activeSelf;
		}

		public void Clear()
		{
			int count = screenStack.Count;
			for (int i = 0; i < count; i++)
				TakeScreenDown(screenStack.Pop());
		}

		private void TakeScreenDown(UILayer screen)
		{
			screen.gameObject.SetActive(false);
			screen.OnScreenDown();
		}

		private void PutScreenUp(UILayer screen)
		{
			screen.gameObject.SetActive(true);
			screen.OnScreenUp();
		}

		private IEnumerator DelayedUp(UILayer screen, float delay)
		{
			yield return new WaitForSeconds(delay);
			PutScreenUp(screen);
		}

		private IEnumerator DelayedDown(UILayer screen, float delay)
		{
			yield return new WaitForSeconds(delay);
			TakeScreenDown(screen);
		}
	}
}