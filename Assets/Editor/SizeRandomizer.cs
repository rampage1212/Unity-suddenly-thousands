using UnityEngine;
using UnityEditor;

public class SizeRandomizer : EditorWindow
{
	public enum ColliderShape
	{
		Sphere,
		Capsule,
		Cube
	}
	
	private Vector3 minimumSize = Vector3.one;
	private Vector3 maximumSize = Vector3.one * 2;
	private ColliderShape colliderShape = ColliderShape.Sphere;
    
    [MenuItem ("Omiya Games/Size Randomizer")]
    private static void Init ()
	{
        // Get existing open window or if none, make a new one:
        EditorWindow.GetWindow<SizeRandomizer>();
    }
    
    private void OnGUI ()
	{
		Rect box = new Rect(3, 3, position.width - 6, 20);
        minimumSize = EditorGUI.Vector3Field(box, "Minimum Size", minimumSize);
		box.y += 40;
        maximumSize = EditorGUI.Vector3Field(box, "Maximum Size", maximumSize);
		box.y += 40;
		colliderShape = (ColliderShape)EditorGUI.EnumPopup(box, "Shape of Collider", colliderShape);
		box.y += 20;
		if(GUI.Button(box, "Resize Selected Objects") == true)
		{
			foreach(Transform selection in Selection.transforms)
			{
				if(selection != null)
				{
					ResizeObject(selection);
				}
			}
		}
    }
	
	private void ResizeObject(Transform selection)
	{
		Vector3 newSize = minimumSize;
		switch(colliderShape)
		{
		case ColliderShape.Sphere:
		{
			Vector3 ratio = maximumSize - minimumSize;
			newSize += (ratio * Random.value);
			break;
		}
		case ColliderShape.Capsule:
		{
			Vector3 ratio = maximumSize - minimumSize;
			newSize += (ratio * Random.value);
			newSize.y = Random.Range(minimumSize.y, maximumSize.y);
			break;
		}
		case ColliderShape.Cube:
		{
			newSize.x = Random.Range(minimumSize.x, maximumSize.x);
			newSize.y = Random.Range(minimumSize.y, maximumSize.y);
			newSize.z = Random.Range(minimumSize.z, maximumSize.z);
			break;
		}
		}
		selection.localScale = newSize;
	}
}