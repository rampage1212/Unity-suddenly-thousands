using UnityEngine;
using System.Collections;

public class RandomizeSetup : ISingletonScript
{
	public Texture[] allTextures;
	
	public Color RandomColor
	{
		get
		{
			return new Color(Random.value, Random.value, Random.value);
		}
	}
	
	public Texture RandomTexture
	{
		get
		{
			return allTextures[Random.Range(0, allTextures.Length)];
		}
	}
	
	public override void SingletonStart()
	{
	}
	
	public override void SceneStart()
	{
	}
}
