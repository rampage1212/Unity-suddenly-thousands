using UnityEngine;
using System.Collections;

public class AutoRotateObject : MonoBehaviour
{
	public int dir = 1;
	
	bool rotating;
	
	float speed = 50.0f;
	
	// Use this for initialization
	void Start ()
	{
		GetComponent<Animation>().GetClip("idle1").wrapMode = WrapMode.Loop;
		GetComponent<Animation>().Play("idle1");
		
		speed += Random.Range(-10.0f,10.0f);
		rotating = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(rotating)
		{
			transform.Rotate(Vector3.up * 60 * Time.deltaTime * dir);
		}
	}
	
	void OnMouseDown()
	{
		rotating = !rotating;
		if(rotating) 	dir = ((dir < 0) ? 1 : -1);

	}
}
