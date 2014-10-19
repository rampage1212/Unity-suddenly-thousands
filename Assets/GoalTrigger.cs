using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioMutator))]
public class GoalTrigger : MonoBehaviour
{
    public TextMesh numberIndicator;
    public int expectedNumber = 1;

    [Header("Sound Effects")]
    public AudioClip enter;
    public AudioClip exit;

    readonly HashSet<Collider> characters = new HashSet<Collider>();
    AudioMutator mutator = null;
    void Start()
    {
        numberIndicator.text = string.Format("0/{0}", expectedNumber);
        mutator = GetComponent<AudioMutator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") == true)
        {
            characters.Add(other);
            numberIndicator.text = string.Format("{0}/{1}", characters.Count, expectedNumber);
            if(characters.Count >= expectedNumber)
            {
                MouseOrbitImproved.CurrentState = MouseOrbitImproved.State.Finished;
            }
            mutator.Audio.clip = enter;
            mutator.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") == true)
        {
            characters.Remove(other);
            numberIndicator.text = string.Format("{0}/{1}", characters.Count, expectedNumber);
            mutator.Audio.clip = exit;
            mutator.Play();
        }
    }
}
