using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;


namespace Nox.Core.Animation
{
	public class AnimatorEventsWindow : EditorWindow
	{
		private Texture acIcon;

		private AnimatorController currentController;

		//Content Elements
		private Label controllerNameLabel;
		private Image controllerIcon;
		private VisualElement statesContainer;

		[MenuItem("Window/Nox/Animation/Animator Events")]
		private static void ShowWindow()
		{
			AnimatorEventsWindow window = GetWindow<AnimatorEventsWindow>();

			window.titleContent = new GUIContent("Animator Events");
			window.Show();
		}

		private void OnEnable()
		{
			SetupVisualElements();
			CheckSelection();

			Selection.selectionChanged += CheckSelection;
			AnimatorControllerPostProcessor.AnimatorControllerSavedEvent += OnAnimatorControllerSaved;
			AnimatorControllerPostProcessor.AnimatorControllerMovedEvent += OnAnimatorControllerMoved;
		}

		private void OnDisable()
		{
			Selection.selectionChanged -= CheckSelection;
			AnimatorControllerPostProcessor.AnimatorControllerSavedEvent -= OnAnimatorControllerSaved;
			AnimatorControllerPostProcessor.AnimatorControllerMovedEvent -= OnAnimatorControllerMoved;
		}

		private void SetupVisualElements()
		{
			var root = rootVisualElement;
			
			var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.altanox.core/Editor/Animation/AnimatorEventsWindowStyles.uss");
			if (styleSheet == null)
				styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/com.altanox.core/Editor/Animation/AnimatorEventsWindowStyles.uss");
			if (styleSheet != null)
				root.styleSheets.Add(styleSheet);


			var guiContent = EditorGUIUtility.ObjectContent(null, typeof(AnimatorController));
			acIcon = guiContent.image;

			controllerIcon = new Image() { image = acIcon };
			controllerIcon.AddToClassList("Icon");
			controllerIcon.AddToClassList("Hide");

			controllerNameLabel = new Label() { text = "" };
			controllerNameLabel.AddToClassList("Header");

			statesContainer = new VisualElement();

			root.Add(controllerIcon);
			root.Add(controllerNameLabel);
			root.Add(statesContainer);
		}

		private void CheckSelection()
		{
			var controller = Selection.activeObject as AnimatorController;

			if(controller != null) //Selected controller in project 
			{
				var cPath = AssetDatabase.GetAssetPath(controller);
				var ccPath = AssetDatabase.GetAssetPath(currentController);
				if(!cPath.Equals(ccPath))
					SetContents(controller);
			}
			else if(Selection.activeTransform != null) //Selected controller in hierarchy
			{
				var animator = Selection.activeTransform.GetComponent<Animator>();
				if (animator != null && animator.runtimeAnimatorController != null)
				{
					controller = animator.runtimeAnimatorController as AnimatorController;

					if (currentController == null)
						SetContents(controller);
					else
					{
						var ccPath = AssetDatabase.GetAssetPath(currentController);
						var cPath = AssetDatabase.GetAssetPath(controller);
						if (!cPath.Equals(ccPath))
							SetContents(controller);
					}
				}
			}
		}

		//Update window contents if the currently inspected animator controller was saved
		private void OnAnimatorControllerSaved(string controller)
		{
			if(currentController != null && controller.Equals(AssetDatabase.GetAssetPath(currentController)))
				SetContents(currentController);
		}

		//Check if the controller was renamed in the editor to update header
		private void OnAnimatorControllerMoved(string to, string from)
		{
			if (currentController == null)
				return;

			var currentPath = AssetDatabase.GetAssetPath(currentController);
			var fileName = Path.GetFileNameWithoutExtension(currentPath);
			if(!fileName.Equals(currentController.name))
			{
				currentController.name = fileName;
				SetHeader(fileName);
			}
		}

		private void SetContents(AnimatorController controller)
		{
			currentController = controller;

			statesContainer.Clear();
			if (currentController == null)
			{
				controllerIcon.AddToClassList("Hide");
				controllerNameLabel.text = "";
			}
			else
			{
				controllerIcon.RemoveFromClassList("Hide");
				controllerNameLabel.text = currentController.name;
				var states = currentController.layers[0].stateMachine.states;
				foreach (var state in states)
				{
					statesContainer.Add(new Label(state.state.name));
				}
			}
		}

		private void AddEventsToState(AnimatorState state)
		{
			bool hasEvents = false;
			foreach(var behaviour in state.behaviours)
			{
				if((behaviour as AnimationEvents) != null)
				{
					hasEvents = true;
					break;
				}
			}

			if (!hasEvents)
				state.AddStateMachineBehaviour<AnimationEvents>();
		}

		private void SetHeader(string text)
		{
			controllerNameLabel.text = text;
		}
	}
}

