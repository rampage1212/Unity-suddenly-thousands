using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioMutator))]
public class MovableCube : MonoBehaviour
{
    public const float Immovable = 99999;
    public int expectedNumber = 0;
    public TextMesh[] labels;
    AudioMutator audioMutator;

    readonly HashSet<Collider> controllers = new HashSet<Collider>();
    float originalMass = 0;
	// Use this for initialization
	void Start ()
    {
        audioMutator = GetComponent<AudioMutator>();
        if((expectedNumber > 0) && (labels != null))
        {
            for(int index = 0; index < labels.Length; ++index)
            {
                labels[index].text = expectedNumber.ToString();
            }
            originalMass = GetComponent<Rigidbody>().mass;
            GetComponent<Rigidbody>().mass = Immovable;
        }
	}
	
    void OnCollisionEnter(Collision info)
    {
        if(info.collider.CompareTag("Player"))
        {
            controllers.Add(info.collider);
            if((expectedNumber > 0) && (controllers.Count >= expectedNumber))
            {
                GetComponent<Rigidbody>().mass = originalMass;
            }
        }
        else
        {
            audioMutator.Play();
        }
    }

    void OnCollisionExit(Collision info)
    {
        if(info.collider.CompareTag("Player"))
        {
            controllers.Remove(info.collider);
            if((expectedNumber > 0) && (controllers.Count < expectedNumber))
            {
                GetComponent<Rigidbody>().mass = Immovable;
            }
        }
    }
}
