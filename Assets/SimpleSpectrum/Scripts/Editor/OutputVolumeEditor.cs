
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OutputVolume))]
[CanEditMultipleObjects]
public class OutputVolumeEditor : Editor
{
	SerializedProperty propertyWindow;

	SerializedProperty propertyEnabled;

	SerializedProperty propertySourceType;
	SerializedProperty propertyAudioSource;
	SerializedProperty propertySampleAmount;
	SerializedProperty propertyChannel;
	SerializedProperty propertyAttackDamp;
	SerializedProperty propertyDecayDamp;

	SerializedProperty propertyOutputType;
	SerializedProperty propertyValueMultiplier;
	SerializedProperty propertyOutputScaleMin;
	SerializedProperty propertyOutputScaleMax;
	SerializedProperty propertyBarPrefab;
	SerializedProperty propertyScalePrefab;
	SerializedProperty propertyUseColorGradient;
	SerializedProperty propertyMinColor;
	SerializedProperty propertyMaxColor;
	SerializedProperty propertyColorCurve;
	SerializedProperty propertyColorAttackDamp;
	SerializedProperty propertyColorDecayDamp;

	bool foldoutSamplingOpen = true;
	bool foldoutOutputOpen = true;

	void OnEnable()
	{
		propertyEnabled = serializedObject.FindProperty("isEnabled");

		propertySourceType   = serializedObject.FindProperty("sourceType");
		propertyAudioSource  = serializedObject.FindProperty("audioSource");
		propertySampleAmount = serializedObject.FindProperty("sampleAmount");
		propertyChannel      = serializedObject.FindProperty("channel");
		propertyAttackDamp   = serializedObject.FindProperty("attackDamp");
		propertyDecayDamp    = serializedObject.FindProperty("decayDamp");

		propertyOutputType       = serializedObject.FindProperty("outputType");
		propertyValueMultiplier  = serializedObject.FindProperty("valueMultiplier");
		propertyOutputScaleMin   = serializedObject.FindProperty("outputScaleMin");
		propertyOutputScaleMax   = serializedObject.FindProperty("outputScaleMax");
		propertyBarPrefab        = serializedObject.FindProperty("prefab");
		propertyScalePrefab      = serializedObject.FindProperty("scalePrefab");
		propertyUseColorGradient = serializedObject.FindProperty("useColorGradient");
		propertyMinColor         = serializedObject.FindProperty("MinColor");
		propertyMaxColor         = serializedObject.FindProperty("MaxColor");
		propertyColorCurve       = serializedObject.FindProperty("colorCurve");
		propertyColorAttackDamp  = serializedObject.FindProperty("colorAttackDamp");
		propertyColorDecayDamp   = serializedObject.FindProperty("colorDecayDamp");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.LabelField("A simple volume display by Sam Boyer.", new GUIStyle { fontSize = 10 });

#if UNITY_WEBGL
        EditorGUILayout.LabelField("Unfortunately SimpleSpectrum will not work with WebGL! :(", new GUIStyle { wordWrap = true });
#endif

		EditorGUILayout.PropertyField(propertyEnabled);

		foldoutSamplingOpen = EditorGUILayout.Foldout(foldoutSamplingOpen, "Sampling Settings");
		if (foldoutSamplingOpen)
		{
			EditorGUILayout.PropertyField(propertySourceType);

			if (propertySourceType.enumValueIndex == 0) //audioSource
			{
				EditorGUILayout.PropertyField(propertyAudioSource);
			}

			if (propertySourceType.enumValueIndex == 2) //custom
			{
				EditorGUILayout.LabelField("Use the inputValue property to set your own data.", new GUIStyle { fontSize = 10, wordWrap = true });
			}

			EditorGUILayout.PropertyField(propertySampleAmount);
			EditorGUILayout.PropertyField(propertyChannel);

			EditorGUILayout.PropertyField(propertyAttackDamp);
			EditorGUILayout.PropertyField(propertyDecayDamp);
		}

		foldoutOutputOpen = EditorGUILayout.Foldout(foldoutOutputOpen, "Output Settings");
		if (foldoutOutputOpen)
		{
			EditorGUILayout.PropertyField(propertyOutputType);

			switch (propertyOutputType.enumValueIndex)
			{
				case 0: //prefab
					EditorGUILayout.PropertyField(propertyBarPrefab);
					EditorGUILayout.PropertyField(propertyScalePrefab);
					EditorGUILayout.PropertyField(propertyUseColorGradient);

					if (propertyUseColorGradient.boolValue)
					{
						EditorGUILayout.PropertyField(propertyMinColor);
						EditorGUILayout.PropertyField(propertyMaxColor);
						EditorGUILayout.PropertyField(propertyColorCurve);
						EditorGUILayout.PropertyField(propertyColorAttackDamp);
						EditorGUILayout.PropertyField(propertyColorDecayDamp);
					}
					break;

				case 1: //pos
					EditorGUILayout.LabelField("Use the Value Multiplier to scale and mask different dimensions for positioning.", new GUIStyle { fontSize = 10, wordWrap = true });
					EditorGUILayout.PropertyField(propertyValueMultiplier);
					break;

				case 2: //rot
					EditorGUILayout.LabelField("Use the Value Multiplier to scale and mask different dimensions for rotation.", new GUIStyle { fontSize = 10, wordWrap = true });
					EditorGUILayout.PropertyField(propertyValueMultiplier);
					break;

				case 3: //scale
					EditorGUILayout.PropertyField(propertyOutputScaleMin);
					EditorGUILayout.PropertyField(propertyOutputScaleMax);
					break;
			}
		}


		serializedObject.ApplyModifiedProperties();
	}
}

