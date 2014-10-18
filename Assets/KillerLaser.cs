using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class KillerLaser : MonoBehaviour
{
    public Transform[] segments = null;
    public float interval = 0.2f;
    public Vector3 min, max;

    LineRenderer rendererCache = null;
    float lastUpdate = 0;
    int index = 0;
    Vector3[] originalPositions = null;
    Vector3 position;
    bool setup = false;

    public LineRenderer CachedRenderer
    {
        get
        {
            if(rendererCache == null)
            {
                rendererCache = GetComponent<LineRenderer>();
            }
            return rendererCache;
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if((Time.time - lastUpdate) > interval)
        {
            setup = false;
            if(originalPositions == null)
            {
                originalPositions = new Vector3[segments.Length];
                setup = true;
            }
            for(index = 0; index < segments.Length; ++index)
            {
                if(setup == true)
                {
                    originalPositions[index] = segments[index].localPosition;
                }
                position = originalPositions[index];
                position.x += Random.Range(min.x, max.x);
                position.y += Random.Range(min.y, max.y);
                position.z += Random.Range(min.z, max.z);
                segments[index].localPosition = position;
                CachedRenderer.SetPosition(index, position);
            }
            lastUpdate = Time.time;
        }
	}
}
