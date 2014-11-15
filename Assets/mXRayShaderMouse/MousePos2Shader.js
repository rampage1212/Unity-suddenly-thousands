// update object position to shader v1.0 - mgear - http://unitycoder.com/blog

#pragma strict

//var obj:Transform;
private var radius:float=2;


function Update () 
{

	// get mouse pos
	var ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
	var hit : RaycastHit;
	if (Physics.Raycast (ray, hit, Mathf.Infinity)) 
	{
//		renderer.material.SetVector("_ObjPos", Vector4(obj.position.x,obj.position.y,obj.position.z,0));
		renderer.material.SetVector("_ObjPos", Vector4(hit.point.x,hit.point.y,hit.point.z,0));

		// convert hit.point to our plane local coordinates, not sure how to do in shader.. IN.pos.. ??
//		var hitlocal = transform.InverseTransformPoint(hit.point);
//		renderer.material.SetVector("_ObjPos",Vector4(hitlocal.x,hitlocal.y,hitlocal.z,0));
		
	}

	
	// box rotation for testing..
	if (Input.GetKey ("a"))
	{
		transform.Rotate(Vector3(0,30,0) * Time.deltaTime);
	}
	if (Input.GetKey ("d"))
	{
		transform.Rotate(Vector3(0,-30,0) * Time.deltaTime);
	}
	
	// mousewheel for radius
	if (Input.GetAxis("Mouse ScrollWheel")!=0)
	{
		radius +=Input.GetAxis("Mouse ScrollWheel")*0.8;
		renderer.material.SetFloat( "_Radius", radius);
	}
}