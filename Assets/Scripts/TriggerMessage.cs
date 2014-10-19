using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class TriggerMessage : MonoBehaviour
{
	public string message;
	
	private bool mIsTriggered = false;
	
	void Awake()
	{
		collider.isTrigger = true;
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if((mIsTriggered == false) && (other.CompareTag("Player") == true))
		{
            // Grab the controller
            ThirdPersonUserControl controller = other.GetComponent<ThirdPersonUserControl>();
            if((controller != null) && (controller.characterController.indicator == ThirdPersonCharacter.Indicator.Controlled))
            {
                PauseMenu.ShowMessage(message);
                mIsTriggered = true;
            }
		}
	}
}
