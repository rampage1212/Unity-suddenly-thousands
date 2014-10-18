using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUITexture))]
[RequireComponent(typeof(AudioSource))]
public class SceneTransition : ISingletonScript
{
	public enum Transition
	{
		NotTransitioning,
		FadingOut,
		FadingIn,
		CompletelyFaded
	}
	
	public float fadeInDuration = 0.6f;
	public float fadeInSpeed = 1f;
	public float fadeOutDuration = 1f;
	public float fadeOutSpeed = 5f;
	
	private int mNextLevel = 1;
	private Transition mTransitionState = Transition.NotTransitioning;
	private float mTargetAlpha = 0;
	private float mCurrentAlpha = 0;
	private Color mTargetColor;
	
	public Transition State
	{
		get
		{
			return mTransitionState;
		}
	}

	public int NextLevel
	{
		get
		{
			return mNextLevel;
		}
	}

	public override void SingletonStart()
	{
		mTargetColor = guiTexture.color;
		mTargetAlpha = 0;
		mCurrentAlpha = 0;
		mTargetColor.a = mTargetAlpha;
		guiTexture.color = mTargetColor;
		guiTexture.enabled = false;
	}
	
	public override void SceneStart()
	{
		if(Application.loadedLevel == mNextLevel)
		{
			// Loaded the correct scene, display fade-out transition
			StartCoroutine(FadeOut());
		}
		else if(Application.loadedLevel == 0)
		{
			mTransitionState = Transition.NotTransitioning; 
		}
	}
	
	public void LoadLevel(int levelIndex)
	{
		if((State == Transition.NotTransitioning) && (levelIndex > 0) && (levelIndex <= (GameSettings.NumLevels + 1)))
		{
			// Play sound
			audio.Play();

			// Set the next level
			mNextLevel = levelIndex;
			
			// Start fading in
			StartCoroutine(FadeIn());
		}
	}
	
	void FixedUpdate()
	{
		// Do the transitioning here
		switch(State)
		{
			case Transition.FadingIn:
			{
				if(guiTexture.enabled == false)
				{
					mTargetColor = guiTexture.color;
					mTargetAlpha = 1;
					mCurrentAlpha = 0;
					mTargetColor.a = mTargetAlpha;
					guiTexture.color = mTargetColor;
					guiTexture.enabled = true;
				}
				else
				{
					mCurrentAlpha = Mathf.Lerp(mCurrentAlpha, mTargetAlpha, (Time.deltaTime * fadeInSpeed));
					mTargetColor.a = mCurrentAlpha;
					guiTexture.color = mTargetColor;
				}
				break;
			}
			case Transition.FadingOut:
			{
				mCurrentAlpha = Mathf.Lerp(mCurrentAlpha, mTargetAlpha, (Time.deltaTime * fadeOutSpeed));
				mTargetColor.a = mCurrentAlpha;
				guiTexture.color = mTargetColor;
				break;
			}
			case Transition.CompletelyFaded:
			{
				mTargetColor = guiTexture.color;
				mTargetAlpha = 0;
				mCurrentAlpha = 1;
				mTargetColor.a = mCurrentAlpha;
				guiTexture.color = mTargetColor;
				guiTexture.enabled = true;
				break;
			}
			default:
			{
				if(guiTexture.enabled == true)
				{
					guiTexture.enabled = false;
				}
				break;
			}
		}
	}
	
	IEnumerator FadeIn()
	{
		mTransitionState = Transition.FadingIn;
		yield return new WaitForSeconds(fadeInDuration);
		mTransitionState = Transition.CompletelyFaded;

		// Check if we're in a webplayer
		GameSettings settings = Singleton.Get<GameSettings>();
		if (settings.IsWebplayer == true)
		{
			// If so, load the loading scene
			Application.LoadLevelAsync(0);
		}
		else
		{
			// If not, directly load to the next level
			Application.LoadLevelAsync(mNextLevel);
		}
	}
	
	IEnumerator FadeOut()
	{
		mTransitionState = Transition.FadingOut;
		yield return new WaitForSeconds(fadeOutDuration);
		mTransitionState = Transition.NotTransitioning;
	}
}
