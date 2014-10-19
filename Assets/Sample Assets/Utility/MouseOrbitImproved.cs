using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioMutator))]
public class MouseOrbitImproved : MonoBehaviour
{
    public enum State
    {
        Playing,
        Finished,
        Paused,
        LostControl,
        NotEnoughCharacters
    }
	public static MouseOrbitImproved instance;

    private const float SwitchGap = 0.2f;

	public ThirdPersonUserControl target;
    public GoalTrigger goal;
    public RecruitmentCollider trigger;

    [Header("Control Adjustment")]
	public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;
	public float zSpeed = 1f;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;
	
	public float distanceMin = .5f;
	public float distanceMax = 15f;
	public float yOffset = 3f;
    public LayerMask raycastLayers;

	[Header("Lerp")]
	public float lerpTranslate = 5f;
	public float lerpRotate = 10f;
    public int pauseButtonWidth = 100;
    public int pauseButtonHeight = 50;

    [Header("HUD")]
    public float boxWidth = 0.2f;
    public float boxHeight = 0.2f;
    public float margin = 0.01f;
    public GUISkin skin = null;

    [Header("Audio")]
    public AudioClip success;
    public AudioClip fail;

    State state = State.Playing;
	float x = 0.0f;
	float y = 0.0f;

	public readonly HashSet<ThirdPersonUserControl> allLivingCharacters = new HashSet<ThirdPersonUserControl>();
	public readonly HashSet<ThirdPersonUserControl> standByCharacters = new HashSet<ThirdPersonUserControl>();
	public readonly List<ThirdPersonUserControl> activeCharacters = new List<ThirdPersonUserControl>();
    private int controlledIndex = 0;
    private float switchTime = 0;
    private Rect buttonRect = new Rect(0, 0, 0, 0);
    AudioMutator audioCache;

    public AudioMutator CachedAudio
    {
        get
        {
            if(audioCache == null)
            {
                audioCache = GetComponent<AudioMutator>();
            }
            return audioCache;
        }
    }

    public static State CurrentState
    {
        get
        {
            return instance.state;
        }
        set
        {
            if(instance.state != value)
            {
                instance.state = value;
                Screen.lockCursor = (instance.state == State.Playing);
                if(instance.state == State.Paused)
                {
                    Time.timeScale = 0;
                }
                switch(instance.state)
                {
                    case State.LostControl:
                    case State.NotEnoughCharacters:
                        instance.CachedAudio.Audio.clip = instance.fail;
                        instance.CachedAudio.Play();
                        break;
                    case State.Finished:
                        instance.CachedAudio.Audio.clip = instance.success;
                        instance.CachedAudio.Play();
                        break;
                }
            }
        }
    }

    void Awake()
	{
		instance = this;
	}

	void OnDestory()
	{
		instance = null;
	}
	
	// Use this for initialization
	void Start ()
    {
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
		
		// Make the rigid body not change rotation
        Screen.lockCursor = true;
	}
	
	void FixedUpdate ()
    {
        if((target) && (CurrentState == State.Playing))
        {

            if(Input.GetButton("Pause"))
            {
                SceneTransition transition = Singleton.Get<SceneTransition>();
                if(transition.State == SceneTransition.Transition.NotTransitioning)
                {
                    CurrentState = State.Paused;
                }
            }
			if(target.state == ThirdPersonUserControl.State.Standby)
			{
				Setup ();
			}

            // Grab the controls
            UpdateCharacterState();
            if((Time.time - switchTime) > SwitchGap)
            {
                UpdateControlledCharacter();
            }

            // Move the target character
			MoveCharacter ();
            trigger.transform.position = target.transform.position;
		}
	}

    void OnGUI()
    {
        SceneTransition transition = Singleton.Get<SceneTransition>();
        if(transition.State == SceneTransition.Transition.NotTransitioning)
        {
            GUI.skin = skin;
            
            // Display the box
            buttonRect.width = (Screen.width * boxWidth);
            buttonRect.height = (Screen.height * boxHeight);
            buttonRect.x = margin;
            buttonRect.y = margin;
            GUI.Box(buttonRect, "Alive: " + allLivingCharacters.Count);
            
            buttonRect.x = (Screen.width - buttonRect.width) / 2;
            GUI.Box(buttonRect, "Active: " + activeCharacters.Count);
            
            buttonRect.x = (Screen.width - buttonRect.width - margin);
            GUI.Box(buttonRect, "Goal: " + goal.expectedNumber);
        }
    }
	
	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}

    public static void KillCharacter(ThirdPersonUserControl controller)
    {
        // Check if the character is still alive
        if((instance != null) && (controller.state != ThirdPersonUserControl.State.Dead))
        {
            // Check if this is the character we're controlling
            if(instance.activeCharacters[instance.controlledIndex] == controller)
            {
                // Check the number of characters
                if(instance.activeCharacters.Count <= 1)
                {
                    // If there's only 1, indicate we lost control of the character
                    CurrentState = State.LostControl;
                    instance.activeCharacters.Clear();
                }
                else
                {
                    // Remove the character from the list
                    instance.activeCharacters.RemoveAt(instance.controlledIndex);
                    if(instance.controlledIndex >= instance.activeCharacters.Count)
                    {
                        instance.controlledIndex = 0;
                    }

                    // Switch control to the next character
                    instance.target = instance.activeCharacters[instance.controlledIndex];
                    instance.target.characterController.indicator = ThirdPersonCharacter.Indicator.Controlled;
                }
            }
            else
            {
                // Check if the character is in the active list
                for(int index = 0; index < instance.activeCharacters.Count; ++index)
                {
                    if((instance.controlledIndex != index) && (instance.activeCharacters[index] == controller))
                    {
                        // Remove the character from the list
                        instance.activeCharacters.RemoveAt(index);
                        if(index < instance.controlledIndex)
                        {
                            instance.controlledIndex--;
                        }
                        break;
                    }
                }
            }

            // Remove the character in the standby and living list
            instance.standByCharacters.Remove(controller);
            instance.allLivingCharacters.Remove(controller);

            // Kill the character
            controller.state = ThirdPersonUserControl.State.Dead;
            controller.characterController.indicator = ThirdPersonCharacter.Indicator.Dead;

            // Check if there's enough character alive
            if(instance.allLivingCharacters.Count < instance.goal.expectedNumber)
            {
                CurrentState = State.NotEnoughCharacters;
            }
        }
    }

	void Setup()
	{
		target.state = ThirdPersonUserControl.State.Active;
		target.characterController.indicator = ThirdPersonCharacter.Indicator.Controlled;

		if(activeCharacters.Count == 0)
		{
			activeCharacters.Add(target);
			controlledIndex = 0;
		}
    }

    void UpdateCharacterState()
    {
        if ((Input.GetButtonDown("Fire1") == true) && (standByCharacters.Count > 0))
        {
            foreach (ThirdPersonUserControl controller in standByCharacters)
            {
                activeCharacters.Add(controller);
                controller.state = ThirdPersonUserControl.State.Active;
                controller.characterController.indicator = ThirdPersonCharacter.Indicator.Active;
            }
            standByCharacters.Clear();
        }
        else if ((Input.GetButtonDown("Fire2")) && (activeCharacters.Count > 1))
        {
            ThirdPersonUserControl currentController = activeCharacters[controlledIndex];
            foreach (ThirdPersonUserControl controller in activeCharacters)
            {
                if (controller != currentController)
                {
                    controller.state = ThirdPersonUserControl.State.Standby;
                    controller.characterController.indicator = ThirdPersonCharacter.Indicator.Standby;
                }
            }
            activeCharacters.Clear();
            activeCharacters.Add(currentController);
            controlledIndex = 0;
        }
    }

    void UpdateControlledCharacter()
    {
        // Make sure we're in control of at least one character
        if(activeCharacters.Count > 1)
        {
            if(Input.GetButtonDown("SwitchCharactersRight") == true)
            {
                // Downgrade the controlled character to just active
                ThirdPersonUserControl lastCharacter = activeCharacters[controlledIndex];
                lastCharacter.characterController.indicator = ThirdPersonCharacter.Indicator.Active;

                // Update the index
                ++controlledIndex;
                if(controlledIndex >= activeCharacters.Count)
                {
                    controlledIndex = 0;
                }

                // Move to the next character
                target = activeCharacters[controlledIndex];
                target.characterController.indicator = ThirdPersonCharacter.Indicator.Controlled;
                switchTime = Time.time;
            }
            else if(Input.GetButtonDown("SwitchCharactersLeft") == true)
            {
                // Downgrade the controlled character to just active
                ThirdPersonUserControl lastCharacter = activeCharacters[controlledIndex];
                lastCharacter.characterController.indicator = ThirdPersonCharacter.Indicator.Active;

                // Update the index
                --controlledIndex;
                if(controlledIndex < 0)
                {
                    controlledIndex = (activeCharacters.Count - 1);
                }
                
                // Move to the next character
                target = activeCharacters[controlledIndex];
                target.characterController.indicator = ThirdPersonCharacter.Indicator.Controlled;
                switchTime = Time.time;
            }
        }
    }

	void MoveCharacter ()
	{
        // Get input
		x += Input.GetAxis ("Mouse X") * xSpeed * distance * 0.02f;
		y -= Input.GetAxis ("Mouse Y") * ySpeed * 0.02f;
		y = ClampAngle (y, yMinLimit, yMaxLimit);
		Quaternion rotation = Quaternion.Euler (y, x, 0);
		distance = Mathf.Clamp (distance - Input.GetAxis ("Mouse Y") * zSpeed, distanceMin, distanceMax);
		RaycastHit hit;
        if (Physics.Linecast (target.position, transform.position, out hit, raycastLayers)) {
			distance -= hit.distance;
		}
		Vector3 negDistance = new Vector3 (0.0f, yOffset, -distance);
		Vector3 position = rotation * negDistance + target.position;
		transform.rotation = Quaternion.Lerp (transform.rotation, rotation, (Time.deltaTime * lerpRotate));
		transform.position = Vector3.Lerp (transform.position, position, (Time.deltaTime * lerpTranslate));
	}
}