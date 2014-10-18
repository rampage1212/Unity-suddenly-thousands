using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
	private static PauseMenu msInstance = null;
	
	public int pauseButtonWidth = 100;
	public int pauseButtonHeight = 50;
	public float boxWidth = 0.3f;
	public float boxHeight = 0.5f;
	public float boxBottomMargin = 0.02f;
	public float boxButtonWidth = 0.25f;
	public float continueButtonHeight = 0.28f;
	public float restartLevelButtonHeight = 0.1f;
	public float returnToMenuButtonHeight = 0.1f;
	
	[System.Serializable]
	public class PopUp
	{
		public float appearLength = 3f;
		public float appearSpeed = 5f;
		public int popUpWidth = 500;
		public int popUpHeight = 100;
		public int popUpMargin = 20;
		
		private float mTimeFirstAppeared = -1f;
		private string mText = string.Empty;
		private float mCurrentYCoordinate = -1f;
		private int mTargetYCoordinate = 0;
		
		public bool IsUp
		{
			get
			{
				return (mTimeFirstAppeared > 0);
			}
		}
		
		public string Message
		{
			get
			{
				return mText;
			}
		}
		
		public void ShowMessage(string message)
		{
			mTimeFirstAppeared = Time.time;
			mText = message;
		}
		
		public void HideMessage()
		{
			mTimeFirstAppeared = -1f;
		}

		public void ShowLastMessage()
		{
			mTimeFirstAppeared = Time.time;
		}
		
		public void Display(ref Rect buttonSize, float deltaTime, SceneTransition transition)
		{
			if(IsUp == true)
			{
				// Update the current Y coordinate, if necessary
				if(mCurrentYCoordinate < 0)
				{
					mCurrentYCoordinate = Screen.height;
				}
				
				// Find the target Y coordinate
				if((transition == null) || (transition.State != SceneTransition.Transition.NotTransitioning) || ((Time.time - mTimeFirstAppeared) > appearLength))
				{
					mTargetYCoordinate = Screen.height;
				}
				else
				{
					mTargetYCoordinate = (Screen.height - (popUpHeight + popUpMargin));
				}
				
				// Lerp to this value
				mCurrentYCoordinate = Mathf.Lerp(mCurrentYCoordinate, mTargetYCoordinate, (deltaTime * appearSpeed));
				
				// Display the dialog
				buttonSize.x = (Screen.width - popUpWidth) / 2;
				buttonSize.y = mCurrentYCoordinate;
				buttonSize.width = popUpWidth;
				buttonSize.height = popUpHeight;
				GUI.Box(buttonSize, mText);
				
				// Check if we should stop displaying the message
				if(((Time.time - mTimeFirstAppeared) > appearLength) && (mTargetYCoordinate <= Mathf.RoundToInt(mCurrentYCoordinate)))
				{
					mCurrentYCoordinate = -1f;
					mTimeFirstAppeared = -1f;
				}
			}
		}
	}
	public PopUp popUpSettings = null;
	
	public GUISkin skin = null;

	private Rect mTempRect = new Rect(0, 0, 0, 0);

	public static void ShowMessage(string message)
	{
		if((msInstance != null) && (msInstance.popUpSettings != null))
		{
			msInstance.popUpSettings.ShowMessage(message);
		}
	}
	
	public static void HideMessage()
	{
		if((msInstance != null) && (msInstance.popUpSettings != null))
		{
			msInstance.popUpSettings.HideMessage();
		}
	}

	void Awake()
	{
		msInstance = this;
	}
	
	void OnGUI()
	{
		GUI.skin = skin;
		SceneTransition transition = Singleton.Get<SceneTransition>();
		popUpSettings.Display(ref mTempRect, Time.deltaTime, transition);
		//if((ThrowHead.IsPaused == true) && (transition.State == SceneTransition.Transition.NotTransitioning))
		{
			//DisplayPauseMenu(mTempRect, transition);
		}
	}
	
	void DisplayPauseMenu (Rect buttonRect, SceneTransition transition)
	{
		// Display the box
		buttonRect.width = (Screen.width * boxWidth);
		buttonRect.height = (Screen.height * boxHeight);
		buttonRect.x = (Screen.width - buttonRect.width) / 2;
		buttonRect.y = (Screen.height - buttonRect.height) / 2;
		GUI.Box(buttonRect, "Pause");
		
		// Add margin
		buttonRect.y += buttonRect.height;
		buttonRect.y -= (Screen.height * boxBottomMargin);
		
		// Display the return to menu button
		buttonRect.width = (Screen.width * boxButtonWidth);
		buttonRect.height = (Screen.height * returnToMenuButtonHeight);
		buttonRect.x = (Screen.width - buttonRect.width) / 2;
		buttonRect.y -= buttonRect.height;
		if(GUI.Button(buttonRect, "Return To Menu") == true)
		{
			Time.timeScale = 1;
			transition.LoadLevel(GameSettings.MenuLevel);
		}
		
		// Add margin
		buttonRect.y -= (Screen.height * boxBottomMargin);
		
		// Display the return to menu button
		buttonRect.height = (Screen.height * restartLevelButtonHeight);
		buttonRect.y -= buttonRect.height;
		if(GUI.Button(buttonRect, "Restart Level") == true)
		{
			Time.timeScale = 1;
			transition.LoadLevel(Application.loadedLevel);
		}
		
		// Add margin
		buttonRect.y -= (Screen.height * boxBottomMargin);
		
		// Display the continue button
		buttonRect.height = (Screen.height * continueButtonHeight);
		buttonRect.y -= buttonRect.height;
		if(GUI.Button(buttonRect, "Continue") == true)
		{
			Time.timeScale = 1;
			Screen.lockCursor = true;
		}
	}
}
