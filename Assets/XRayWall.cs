using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class XRayWall : MonoBehaviour
{
    public Renderer wallRenderer = null;
    public Material normalMaterial = null;
    public Material xRayMaterial = null;

    bool wasMaterialReplaced = false;
    bool isMaterialSeeThrough = false;

	// Use this for initialization
	void Start ()
    {
        MouseOrbitImproved.instance.seeThroughWalls.Add(GetComponent<Collider>(), this);
	}

    Material WallMaterial
    {
        get
        {
            return wallRenderer.material;
        }
        set
        {
            // Replace material
            if(wasMaterialReplaced == false)
            {
                wasMaterialReplaced = true;
            }
            else
            {
                Destroy(wallRenderer.material);
            }
            wallRenderer.material = value;
        }
    }
	
	public void MakeWallSeeThrough(RaycastHit hit)
    {
        if(isMaterialSeeThrough == false)
        {
            WallMaterial = xRayMaterial;
            isMaterialSeeThrough = true;
        }
    }

    public void MakeWallNormal()
    {
        if(isMaterialSeeThrough == true)
        {
            WallMaterial = normalMaterial;
            isMaterialSeeThrough = false;
        }
    }
}
