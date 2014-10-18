using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
	public float startButtonPosition = 0.5f;
	public float endButtonPosition = 0.95f;
	public float buttonWidth = 0.5f;
	public float buttonMargin = 0.02f;
	
	public float smallButtonStartPosition = 0.85f;
	public float smallButtonEndPosition = 1f;
	public float smallButtonWidth = 0.2f;
	
	public GUIText[] titleGuiText = null;
	
	public GUISkin skin = null;

	private float mTargetAlpha = 0;
	private float mCurrentAlpha = 0;
	private Color mCurrentColor = Color.white;
	
	void Awake()
	{
		Screen.lockCursor = false;
		Time.timeScale = 1;
	}
	
	void Start()
	{
		SceneTransition transition = Singleton.Get<SceneTransition>();
		if(transition.State == SceneTransition.Transition.FadingIn)
		{
			mTargetAlpha = 1;
			mCurrentAlpha = 0;
		}
		else
		{
			mTargetAlpha = 1;
			mCurrentAlpha = 1;
		}
		if(titleGuiText != null)
		{
			foreach(GUIText guiText in titleGuiText)
			{
				if(guiText != null)
				{
					mCurrentColor = guiText.material.color;
					mCurrentColor.a = mCurrentAlpha;
					guiText.material.color = mCurrentColor;
				}
			}
		}
	}
	
	void OnGUI()
	{
		// Setup variables
		int level = 1;
		Rect buttonDimension = new Rect();
		string buttonText = string.Empty;
		float margin = (Screen.height * buttonMargin);
		float numLevelsHalf = (GameSettings.NumLevels / 2);
		SceneTransition sceneTransitionInstance = Singleton.Get<SceneTransition>();
		GameSettings gameSettingInstance = Singleton.Get<GameSettings>();
		
		// Disable the GUI if it's loading a level
		if(sceneTransitionInstance.State == SceneTransition.Transition.NotTransitioning)
		{
			GUI.skin = skin;
			
			// Position the first column of buttons
			if((GameSettings.NumLevels % 2) == 1)
			{
				numLevelsHalf += 1;
			}
			// Check if this isn't a webplayer
			if(gameSettingInstance.IsWebplayer == false)
			{
				numLevelsHalf += 1;
			}
			buttonDimension.width = (Screen.width * buttonWidth);
			buttonDimension.y = (Screen.height * startButtonPosition);
			buttonDimension.x = (Screen.width / 2f) - (buttonDimension.width + (margin / 2f));
			buttonDimension.height = (Screen.height * ((endButtonPosition - startButtonPosition) / numLevelsHalf));
			buttonDimension.height -= margin;
			for(level = 1; level <= GameSettings.NumLevels; level += 2)
			{
				buttonText = ("Level " + level);
				GUI.enabled = (level <= gameSettingInstance.NumLevelsUnlocked);
				if(GUI.Button(buttonDimension, buttonText) == true)
				{
					sceneTransitionInstance.LoadLevel(level);
				}
				buttonDimension.y += (buttonDimension.height + margin);
			}
			
			// Position the second column of buttons
			buttonDimension.y = (Screen.height * startButtonPosition);
			buttonDimension.x = (Screen.width / 2f) + (margin / 2f);
			for(level = 2; level <= GameSettings.NumLevels; level += 2)
			{
				buttonText = ("Level " + level);
				GUI.enabled = (level <= gameSettingInstance.NumLevelsUnlocked);
				if(GUI.Button(buttonDimension, buttonText) == true)
				{
					sceneTransitionInstance.LoadLevel(level);
				}
				buttonDimension.y += (buttonDimension.height + margin);
			}
			
			// Re-enable the GUI
			GUI.enabled = true;

			// Check if this isn't a webplayer
			if(gameSettingInstance.IsWebplayer == false)
			{
				// Position the quit button
				buttonDimension.width = (Screen.width * smallButtonWidth);
				buttonDimension.height = (Screen.height * (smallButtonEndPosition - smallButtonStartPosition)) - margin;
				buttonDimension.x = ((Screen.width - buttonDimension.width) / 2f);
				buttonDimension.y = (Screen.height * smallButtonStartPosition);
				if(GUI.Button(buttonDimension, ("Quit Game")) == true)
				{
					Application.Quit();
				}
			}
		}
	}
	
	void Update()
	{
		SceneTransition transition = Singleton.Get<SceneTransition>();
		float fadeSpeed = -1f;
		bool enableGuiText = true;
		switch(transition.State)
		{
			case SceneTransition.Transition.FadingIn:
			{
				mTargetAlpha = 0;
				fadeSpeed = transition.fadeInSpeed;
				break;
			}
			case SceneTransition.Transition.FadingOut:
			{
				mTargetAlpha = 1;
				fadeSpeed = transition.fadeOutSpeed;
				break;
			}
			case SceneTransition.Transition.NotTransitioning:
			{
				mCurrentAlpha = 1;
				break;
			}
			case SceneTransition.Transition.CompletelyFaded:
			{
				mCurrentAlpha = 0;
				enableGuiText = false;
				break;
			}
		}
		fadeSpeed *= 2;
		if(titleGuiText != null)
		{
			if(fadeSpeed > 0)
			{
				mCurrentAlpha = Mathf.Lerp(mCurrentAlpha, mTargetAlpha, (Time.deltaTime * fadeSpeed));
			}
			foreach(GUIText guiText in titleGuiText)
			{
				if(guiText != null)
				{
					if(enableGuiText == true)
					{
						mCurrentColor = guiText.material.color;
						mCurrentColor.a = mCurrentAlpha;
						guiText.material.color = mCurrentColor;
					}
					guiText.enabled = enableGuiText;
				}
			}
		}
	}
}
