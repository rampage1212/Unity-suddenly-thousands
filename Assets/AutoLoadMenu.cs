using UnityEngine;
using System.Collections;

public class AutoLoadMenu : MonoBehaviour
{
    public Transform lastPoint;
    bool triggerOnce = false;

	// Update is called once per frame
	void Update ()
    {
        if((triggerOnce == false) && (transform.position.z > lastPoint.position.z))
        {
            Singleton.Get<SceneTransition>().LoadLevel(GameSettings.MenuLevel);
        }
	}
}
