using UnityEngine;
using System.Collections.Generic;

public class Door : MonoBehaviour
{
    public GameObject doorObject;
    public AudioMutator doorAudio;
    public AudioClip doorOpen;
    public AudioClip doorClose;
    public bool triggerAllSwitches = false;

    int numSwitches = 0;
    bool isOpen = false;
    readonly HashSet<Switch> triggeredSwitches = new HashSet<Switch>();

    public bool IsOpen
    {
        get
        {
            return isOpen;
        }
        private set
        {
            if(isOpen != value)
            {
                isOpen = value;
                doorObject.SetActive(!isOpen);
                if(isOpen == true)
                {
                    doorAudio.Audio.clip = doorOpen;
                    doorAudio.Play();
                    if(triggerAllSwitches == true)
                    {
                        foreach(Switch update in triggeredSwitches)
                        {
                            update.ForceTrigger();
                        }
                    }
                }
                else
                {
                    doorAudio.Audio.clip = doorClose;
                    doorAudio.Play();
                }
            }
        }
    }

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
        IsOpen = (triggeredSwitches.Count >= numSwitches);
    }
}
