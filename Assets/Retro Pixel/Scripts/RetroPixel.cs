using UnityEngine;
using System.Collections;

namespace AlpacaSound
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu("Image Effects/Custom/Retro Pixel")]
	public class RetroPixel : MonoBehaviour
	{
		public static readonly int MAX_NUM_COLORS = 8;

		public int horizontalResolution = 160;
		public int verticalResolution = 200;

		public int numColors = MAX_NUM_COLORS;
		int oldNumColors = 0;

		public Color color0 = new Color (0.00f, 0.00f, 0.00f);
		public Color color1 = new Color (0.14f, 0.14f, 0.14f);
		public Color color2 = new Color (0.29f, 0.29f, 0.29f);
		public Color color3 = new Color (0.43f, 0.43f, 0.43f);
		public Color color4 = new Color (0.57f, 0.57f, 0.57f);
		public Color color5 = new Color (0.71f, 0.71f, 0.71f);
		public Color color6 = new Color (0.86f, 0.86f, 0.86f);
		public Color color7 = new Color (1.00f, 1.00f, 1.00f);
		
		Shader[] shaders = new Shader[MAX_NUM_COLORS];

		Material m_material;
		Material material
		{
			get
			{
				if (m_material == null)
				{
					for (int i = 1; i < MAX_NUM_COLORS; ++i)
					{
						string shaderName = "AlpacaSound/RetroPixel" + (i+1);
						Shader shader = Shader.Find (shaderName);

						if (shader == null)
						{
							Debug.LogError ("Shader \'" + shaderName + "\' not found. Was it deleted?");
							enabled = false;
							return null;
						}
						
						shaders[i] = shader;
					}
					
					m_material = new Material (shaders[1]);
					m_material.hideFlags = HideFlags.DontSave;
				}
				return m_material;
			} 
		}

		void Start ()
		{
			if (!SystemInfo.supportsImageEffects)
			{
				enabled = false;
				return;
			}
		}
		
		public void OnRenderImage (RenderTexture src, RenderTexture dest)
		{
			horizontalResolution = Mathf.Clamp(horizontalResolution, 1, 2048);
			verticalResolution = Mathf.Clamp(verticalResolution, 1, 2048);
			numColors = Mathf.Clamp(numColors, 2, 8);

			if (material)
			{
				if (oldNumColors != numColors)
				{
					material.shader = shaders[numColors-1];
				}
				
				material.SetColor ("_Color0", color0);
				material.SetColor ("_Color1", color1);
				if (numColors > 2) material.SetColor ("_Color2", color2);
				if (numColors > 3) material.SetColor ("_Color3", color3);
				if (numColors > 4) material.SetColor ("_Color4", color4);
				if (numColors > 5) material.SetColor ("_Color5", color5);
				if (numColors > 6) material.SetColor ("_Color6", color6);
				if (numColors > 7) material.SetColor ("_Color7", color7);
				
				RenderTexture scaled = RenderTexture.GetTemporary (horizontalResolution, verticalResolution);
				scaled.filterMode = FilterMode.Point;
				Graphics.Blit (src, scaled, material);
				Graphics.Blit (scaled, dest);
				RenderTexture.ReleaseTemporary (scaled);
				
			}
			else
			{
				Graphics.Blit (src, dest);
			}
		}

		void OnDisable ()
		{
			if (m_material)
			{
				Material.DestroyImmediate (m_material);
			}
		}
	}
}



