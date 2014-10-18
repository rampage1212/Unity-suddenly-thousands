using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class KillerTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ThirdPersonUserControl controller = other.GetComponent<ThirdPersonUserControl>();
            if((controller != null) && (controller.state != ThirdPersonUserControl.State.Dead))
            {
                MouseOrbitImproved.KillCharacter(controller);
            }
        }
    }
}
