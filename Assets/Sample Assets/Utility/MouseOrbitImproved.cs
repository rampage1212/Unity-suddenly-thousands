using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour {
	
	public ThirdPersonUserControl target;
	public Collider triggerCollider;
	public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;
	
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

	public readonly List<ThirdPersonUserControl> controlledCharacters = new List<ThirdPersonUserControl>();
	private int controlledIndex = 0;
	
	// Use this for initialization
	void Start () {
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
		
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}
	
	void FixedUpdate () {
		if (target) {
			if(target.state == ThirdPersonUserControl.State.Standby)
			{
				target.state = ThirdPersonUserControl.State.Active;
				target.characterController.indicator = ThirdPersonCharacter.Indicator.Controlled;
			}

			x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
			y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			
			y = ClampAngle(y, yMinLimit, yMaxLimit);
			
			Quaternion rotation = Quaternion.Euler(y, x, 0);
			
			distance = Mathf.Clamp(distance - Input.GetAxis("Mouse Y") * 5, distanceMin, distanceMax);
			
			RaycastHit hit;
			if (Physics.Linecast (target.position, transform.position, out hit)) {
				distance -=  hit.distance;
			}
			Vector3 negDistance = new Vector3(0.0f, yOffset, -distance);
			Vector3 position = rotation * negDistance + target.position;
			
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, (Time.deltaTime * lerpRotate));
			transform.position = Vector3.Lerp(transform.position, position, (Time.deltaTime * lerpTranslate));

			// Update the collider position
			triggerCollider.transform.position = target.position;
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
}