using UnityEngine;
using System.Collections;

public class RecruitmentCollider : MonoBehaviour
{
	public MouseOrbitImproved controls;
    public float radius = 5;

    Vector3 distance;

    void Update()
    {
        foreach(ThirdPersonUserControl controller in controls.allLivingCharacters)
        {
            if(controller.characterController.indicator == ThirdPersonCharacter.Indicator.Standby)
            {
                // Check if the character is within range
                if(InRange(controller) == true)
                {
                    // If so, make the character recruitable
                    controller.characterController.indicator = ThirdPersonCharacter.Indicator.Recruitable;
                    controls.standByCharacters.Add(controller);
                }
            }
            else if(controller.characterController.indicator == ThirdPersonCharacter.Indicator.Recruitable)
            {
                // Check if the character is out of range
                if(InRange(controller) == false)
                {
                    controller.characterController.indicator = ThirdPersonCharacter.Indicator.Standby;
                    controls.standByCharacters.Remove(controller);
                }
            }
        }
    }

    bool InRange(ThirdPersonUserControl controller)
    {
        distance = controls.target.position - controller.position;
        distance.y = 0;
        return (distance.sqrMagnitude < (radius * radius));
    }
}
