using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class GoalTrigger : MonoBehaviour
{
    public TextMesh numberIndicator;
    public int expectedNumber = 1;

    readonly HashSet<Collider> characters = new HashSet<Collider>();

    void Start()
    {
        numberIndicator.text = string.Format("0/{1}", 0, expectedNumber);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") == true)
        {
            characters.Add(other);
            numberIndicator.text = string.Format("{0}/{1}", characters.Count, expectedNumber);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") == true)
        {
            characters.Remove(other);
            numberIndicator.text = string.Format("{0}/{1}", characters.Count, expectedNumber);
        }
    }
}
