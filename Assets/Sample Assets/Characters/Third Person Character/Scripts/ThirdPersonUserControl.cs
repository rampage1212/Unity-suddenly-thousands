using UnityEngine;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class ThirdPersonUserControl : MonoBehaviour
{
	public enum State
	{
		Standby = 0,
		Active,
		Dead
	}
	
	public bool walkByDefault = false;                  // toggle for walking state
	State currentState = State.Standby;

	private Vector3 lookPos;                            // The position that the character should be looking towards
    private ThirdPersonCharacter character;             // A reference to the ThirdPersonCharacter on the object
	private Transform cam;                              // A reference to the main camera in the scenes transform
	private Vector3 camForward;							// The current forward direction of the camera
	private Vector3 move;								// the world-relative desired move direction, calculated from the camForward and user input.
	private Transform transformCache = null;

	public ThirdPersonCharacter characterController
	{
		get
		{
			return character;
		}
	}

	public Vector3 position
	{
		get
		{
			if(transformCache == null)
			{
				transformCache = transform;
			}
			return transformCache.position;
		}
	}

	public State state
	{
		get
		{
			return currentState;
		}
		set
		{
			if(currentState != value)
			{
				character.ScaleCapsuleForCrouching(value != State.Active);

				// Update animator
				character.CachedAnimator.SetInteger("State", (int)value);

				// Update flag
				currentState = value;
			}
		}
	}

	// Use this for initialization
	void Start ()
	{
        // get the transform of the main camera
		if (Camera.main != null)
		{
			cam = Camera.main.transform;
		} else {
			Debug.LogWarning("Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
			// we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
		}

        // get the third person character ( this should never be null due to require component )
		character = GetComponent<ThirdPersonCharacter>();
		character.ScaleCapsuleForCrouching(true);

		// Add this character to the controls
		MouseOrbitImproved.instance.allCharacters.Add(this);
	}

	// Fixed update is called in sync with physics
	void FixedUpdate ()
	{
        move = Vector3.zero;
        lookPos = transform.position + transform.forward * 100;
        bool jump = false;
        bool crouching = true;
		if(currentState == State.Active)
		{
            crouching = false;
			#if CROSS_PLATFORM_INPUT
			jump = CrossPlatformInput.GetButton("Jump");
			float h = CrossPlatformInput.GetAxis("Horizontal");
			float v = CrossPlatformInput.GetAxis("Vertical");
			#else
			jump = Input.GetButton("Jump");
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");
			#endif

			// calculate move direction to pass to character
			if (cam != null)
			{
				// calculate camera relative direction to move:
				camForward = Vector3.Scale (cam.forward, new Vector3(1,0,1)).normalized;
				move = v * camForward + h * cam.right;	
			} else {
				// we use world-relative directions in the case of no main camera
				move = v * Vector3.forward + h * Vector3.right;
			}

			if (move.magnitude > 1) move.Normalize();

			#if !MOBILE_INPUT
			// On non-mobile builds, walk/run speed is modified by a key press.
			bool walkToggle = Input.GetKey(KeyCode.LeftShift);
			// We select appropriate speed based on whether we're walking by default, and whether the walk/run toggle button is pressed:
			float walkMultiplier = (walkByDefault ? walkToggle ? 1 : 0.5f : walkToggle ? 0.5f : 1);
			move *= walkMultiplier;
			#endif

		}

        // pass all parameters to the character control script
        character.Move( move, crouching, jump, lookPos );
    }
}
