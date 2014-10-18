using UnityEngine;
using System.Collections;

public abstract class ISingletonScript : MonoBehaviour
{
	abstract public void SingletonStart();
	abstract public void SceneStart();
}
