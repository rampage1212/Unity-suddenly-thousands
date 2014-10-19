using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class Switch : MonoBehaviour
{
    [Header("Common Properties")]
    public Door door;
    public int expectedNumber = 1;
    public TextMesh numberIndicator;
    public float lerpColor = 10f;

    [Header("Switch Colors")]
    public Renderer switchRenderer;
    public Color switchTriggeredColor = Color.green;
    public Color switchNotTriggeredColor = Color.white;

    [Header("Wire Colors")]
    public LineRenderer wireRenderer;
    public Color wireTriggeredColor = Color.green;
    public Color wireNotTriggeredColor = Color.white;

    [Header("Trigger Properties")]
    public bool triggerOnce = true;

    bool isTriggered = false;
    readonly HashSet<Collider> characters = new HashSet<Collider>();
    Color switchColor;
    Color wireColor;

    public bool IsTriggered
    {
        get
        {
            return isTriggered;
        }
        private set
        {
            if(isTriggered != value)
            {
                isTriggered = value;
                if(switchRenderer != null)
                {
                    switchColor = (isTriggered ? switchTriggeredColor : switchNotTriggeredColor);
                }
                if(wireRenderer != null)
                {
                    wireColor = (isTriggered ? wireTriggeredColor : wireNotTriggeredColor);
                }
                door.OnSwitchTriggerChanged(this);
            }
        }
    }

    public void ForceTrigger()
    {
        isTriggered = true;
        triggerOnce = true;
        numberIndicator.text = "-";
    }

    void Start()
    {
        numberIndicator.text = string.Format("0/{0}", expectedNumber);
        switchColor = switchNotTriggeredColor;
        wireColor = wireNotTriggeredColor;
        door.AddSwitch(this);
    }

    void Update()
    {
        if(switchRenderer != null)
        {
            switchRenderer.material.color = Color.Lerp(switchRenderer.material.color, switchColor, (lerpColor * Time.deltaTime));
        }
        if(wireRenderer != null)
        {
            wireRenderer.material.color = Color.Lerp(wireRenderer.material.color, wireColor, (lerpColor * Time.deltaTime));
            wireRenderer.SetPosition(0, transform.position);
            wireRenderer.SetPosition(1, door.transform.position);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") == true)
        {
            characters.Add(other);
            if(characters.Count >= expectedNumber)
            {
                IsTriggered = true;
            }
            if((triggerOnce == true) && (IsTriggered == true))
            {
                numberIndicator.text = "-";
            }
            else
            {
                numberIndicator.text = string.Format("{0}/{1}", characters.Count, expectedNumber);
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") == true)
        {
            characters.Remove(other);
            if((triggerOnce == false) && (characters.Count < expectedNumber))
            {
                numberIndicator.text = string.Format("{0}/{1}", characters.Count, expectedNumber);
                IsTriggered = false;
            }
        }
    }
}
