using UnityEngine;
using UnityEditor;
using System.Collections;

namespace AlpacaSound
{
	[CustomEditor (typeof (RetroPixel))]
	public class RetroPixelEditor : Editor
	{
		SerializedObject serObj;

		SerializedProperty horizontalResolution;
		SerializedProperty verticalResolution;
		SerializedProperty numColors;

		SerializedProperty color0;
		SerializedProperty color1;
		SerializedProperty color2;
		SerializedProperty color3;
		SerializedProperty color4;
		SerializedProperty color5;
		SerializedProperty color6;
		SerializedProperty color7;

		void OnEnable ()
		{
			serObj = new SerializedObject (target);
			
			horizontalResolution = serObj.FindProperty ("horizontalResolution");
			verticalResolution = serObj.FindProperty ("verticalResolution");
			numColors = serObj.FindProperty ("numColors");
			color0 = serObj.FindProperty ("color0");
			color1 = serObj.FindProperty ("color1");
			color2 = serObj.FindProperty ("color2");
			color3 = serObj.FindProperty ("color3");
			color4 = serObj.FindProperty ("color4");
			color5 = serObj.FindProperty ("color5");
			color6 = serObj.FindProperty ("color6");
			color7 = serObj.FindProperty ("color7");
		}

		override public void OnInspectorGUI ()
		{
			serObj.Update ();

			//RetroPixel myTarget = (RetroPixel) target;

			horizontalResolution.intValue = EditorGUILayout.IntField("Horizontal Resolution", horizontalResolution.intValue);
			verticalResolution.intValue = EditorGUILayout.IntField("Vertical Resolution", verticalResolution.intValue);
			numColors.intValue = EditorGUILayout.IntSlider("Number of colors", numColors.intValue, 2, RetroPixel.MAX_NUM_COLORS);

			if (numColors.intValue > 0) color0.colorValue = EditorGUILayout.ColorField("Color 0", color0.colorValue);
			if (numColors.intValue > 1) color1.colorValue = EditorGUILayout.ColorField("Color 1", color1.colorValue);
			if (numColors.intValue > 2) color2.colorValue = EditorGUILayout.ColorField("Color 2", color2.colorValue);
			if (numColors.intValue > 3) color3.colorValue = EditorGUILayout.ColorField("Color 3", color3.colorValue);
			if (numColors.intValue > 4) color4.colorValue = EditorGUILayout.ColorField("Color 4", color4.colorValue);
			if (numColors.intValue > 5) color5.colorValue = EditorGUILayout.ColorField("Color 5", color5.colorValue);
			if (numColors.intValue > 6) color6.colorValue = EditorGUILayout.ColorField("Color 6", color6.colorValue);
			if (numColors.intValue > 7) color7.colorValue = EditorGUILayout.ColorField("Color 7", color7.colorValue);

			serObj.ApplyModifiedProperties ();
		}
	}
}
