using UnityEngine;
using System.Collections.Generic;

public class Door : MonoBehaviour
{
    public GameObject doorObject;

    int numSwitches = 0;
    readonly HashSet<Switch> triggeredSwitches = new HashSet<Switch>();

    public void AddSwitch(Switch trigger)
    {
        numSwitches++;
    }

    public void OnSwitchTriggerChanged(Switch trigger)
    {
        if(trigger.IsTriggered == true)
        {
            triggeredSwitches.Add(trigger);
        }
        else
        {
            triggeredSwitches.Remove(trigger);
        }
        doorObject.SetActive(triggeredSwitches.Count < numSwitches);
    }
}
