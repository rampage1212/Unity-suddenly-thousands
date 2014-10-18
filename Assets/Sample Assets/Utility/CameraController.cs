using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public Transform target;
	public Vector3 offset = new Vector3(0f, 7.5f, 0f);
	public float lerpRotate = 10f;
	
	void LateUpdate ()
	{
		transform.position = target.position + offset;
		//Input.mousePosition
	}
}
