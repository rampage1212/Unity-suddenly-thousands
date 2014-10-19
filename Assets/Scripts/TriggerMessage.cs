using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class TriggerMessage : MonoBehaviour
{
	public string message;
	
	void Awake()
	{
		collider.isTrigger = true;
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") == true)
		{
            // Grab the controller
            ThirdPersonUserControl controller = other.GetComponent<ThirdPersonUserControl>();
            if((controller != null) && (controller.characterController.indicator == ThirdPersonCharacter.Indicator.Controlled))
            {
                PauseMenu.ShowMessage(message);
            }
		}
	}

    public void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") == true)
        {
            // Grab the controller
            ThirdPersonUserControl controller = other.GetComponent<ThirdPersonUserControl>();
            if((controller != null) && (controller.characterController.indicator == ThirdPersonCharacter.Indicator.Controlled))
            {
                PauseMenu.HideMessage();
            }
        }
    }
}
