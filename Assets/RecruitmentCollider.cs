using UnityEngine;
using System.Collections;

public class RecruitmentCollider : MonoBehaviour
{
	public MouseOrbitImproved controls;

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") == true)
		{
			ThirdPersonUserControl controller = other.GetComponent<ThirdPersonUserControl>();
			if(controller.characterController.indicator == ThirdPersonCharacter.Indicator.Standby)
			{
				controller.characterController.indicator = ThirdPersonCharacter.Indicator.Recruitable;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Player") == true)
		{
			ThirdPersonUserControl controller = other.GetComponent<ThirdPersonUserControl>();
			if(controller.characterController.indicator == ThirdPersonCharacter.Indicator.Recruitable)
			{
				controller.characterController.indicator = ThirdPersonCharacter.Indicator.Standby;
			}
		}
	}
}
