using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseOrbitImproved : MonoBehaviour
{
	public static MouseOrbitImproved instance;

    private const float SwitchGap = 0.2f;

	public ThirdPersonUserControl target;
    public RecruitmentCollider trigger;
	public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;
	public float zSpeed = 1f;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;
	
	public float distanceMin = .5f;
	public float distanceMax = 15f;
	public float yOffset = 3f;

	[Header("Lerp")]
	public float lerpTranslate = 5f;
	public float lerpRotate = 10f;

	float x = 0.0f;
	float y = 0.0f;

	public readonly HashSet<ThirdPersonUserControl> allCharacters = new HashSet<ThirdPersonUserControl>();
	public readonly HashSet<ThirdPersonUserControl> standByCharacters = new HashSet<ThirdPersonUserControl>();
	public readonly List<ThirdPersonUserControl> activeCharacters = new List<ThirdPersonUserControl>();
    private int controlledIndex = 0;
    private float switchTime = 0;

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
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}
	
	void FixedUpdate ()
    {
		if (target)
        {
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
        // FIXME: kill the character, and do something about updating the controlled characters!
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
		if (Physics.Linecast (target.position, transform.position, out hit)) {
			distance -= hit.distance;
		}
		Vector3 negDistance = new Vector3 (0.0f, yOffset, -distance);
		Vector3 position = rotation * negDistance + target.position;
		transform.rotation = Quaternion.Lerp (transform.rotation, rotation, (Time.deltaTime * lerpRotate));
		transform.position = Vector3.Lerp (transform.position, position, (Time.deltaTime * lerpTranslate));
	}
}