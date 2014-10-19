using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioMutator))]
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

    [Header("Sound")]
    public AudioClip enter;
    public AudioClip exit;
    public AudioClip triggered;
    public AudioClip untriggered;

    bool isTriggered = false;
    readonly HashSet<Collider> characters = new HashSet<Collider>();
    Color switchColor;
    Color wireColor;
    AudioMutator audioCache = null;

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

                if(isTriggered == true)
                {
                    audioCache.Audio.clip = triggered;
                }
                else
                {
                    audioCache.Audio.clip = untriggered;
                }
                audioCache.Play();
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
        audioCache = GetComponent<AudioMutator>();
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
            else if(IsTriggered == false)
            {
                audioCache.Audio.clip = enter;
                audioCache.Play();
            }
            if((triggerOnce == true) && (IsTriggered == true))
            {
                numberIndicator.text = "OK";
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
            else if(IsTriggered == false)
            {
                numberIndicator.text = string.Format("{0}/{1}", characters.Count, expectedNumber);
                audioCache.Audio.clip = exit;
                audioCache.Play();
            }
        }
    }
}
