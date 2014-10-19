using UnityEngine;
using System.Collections.Generic;

public class Door : MonoBehaviour
{
    public const float CutoffSnap = 0.01f;
    public Collider doorObject;
    public Renderer[] doorRenderers;
    public float doorLerp = 5f;
    public AudioMutator doorAudio;
    public AudioClip doorOpen;
    public AudioClip doorClose;
    public bool triggerAllSwitches = false;

    int numSwitches = 0;
    bool isOpen = false, doorChanged = false;
    readonly HashSet<Switch> triggeredSwitches = new HashSet<Switch>();
    readonly List<Material> allMaterials = new List<Material>();
    float targetCutOff = 0, currentCutOff = 0;

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
                doorObject.enabled = !isOpen;
                if(isOpen == true)
                {
                    doorAudio.Audio.clip = doorOpen;
                    targetCutOff = 1;
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
                    targetCutOff = 0;
                    doorAudio.Audio.clip = doorClose;
                }
                doorAudio.Play();
                doorChanged = true;
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

    void Start()
    {
        foreach(Renderer door in doorRenderers)
        {
            foreach(Material material in door.materials)
            {
                allMaterials.Add(material);
            }
        }
    }

    void Update()
    {
        if(doorChanged == true)
        {
            foreach(Renderer door in doorRenderers)
            {
                door.enabled = true;
            }
            if(Mathf.Abs(currentCutOff - targetCutOff) > CutoffSnap)
            {
                currentCutOff = Mathf.Lerp(currentCutOff, targetCutOff, (Time.deltaTime * doorLerp));
                foreach(Material material in allMaterials)
                {
                    material.SetFloat("_Cutoff", currentCutOff);
                }
            }
            else if(Mathf.Approximately(0, targetCutOff) == true)
            {
                foreach(Material material in allMaterials)
                {
                    material.SetFloat("_Cutoff", 0);
                }
                doorChanged = false;
            }
            else if(Mathf.Approximately(1, targetCutOff) == true)
            {
                foreach(Renderer door in doorRenderers)
                {
                    door.enabled = false;
                }
                doorChanged = false;
            }
        }
    }
}
