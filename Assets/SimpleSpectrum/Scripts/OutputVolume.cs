using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OutputVolume : MonoBehaviour
{
	public enum SourceType
	{
		AudioSource, AudioListener, Custom
	}

	public enum OutputType
	{
		PrefabBar, ObjectPosition, ObjectRotation, ObjectScale
	}

	#region SAMPLING PROPERTIES
	/// <summary>
	/// Enables or disables the processing and display of volume data. 
	/// </summary>
	[Tooltip("Enables or disables the processing and display of volume data.")]
	public bool isEnabled = true;
	/// <summary>
	/// The type of source for volume data.
	/// </summary>
	[Tooltip("The type of source for volume data.")]
	public SourceType sourceType = SourceType.AudioSource;
	/// <summary>
	/// The AudioSource to take data from. Can be empty if sourceType is not AudioSource.
	/// </summary>
	[Tooltip("The AudioSource to take data from.")]
	public AudioSource audioSource;
	/// <summary>
	/// The number of samples to use when sampling. Must be a power of two.
	/// </summary>
	[Tooltip("The number of samples to use when sampling. Must be a power of two.")]
	public int sampleAmount = 256;
	/// <summary>
	/// The audio channel to take data from when sampling.
	/// </summary>
	[Tooltip("The audio channel to take data from when sampling.")]
	public int channel = 0;
	/// <summary>
	/// The amount of dampening used when the new scale is higher than the bar's existing scale. Must be between 0 (slowest) and 1 (fastest).
	/// </summary>
	[Range(0, 1)]
	[Tooltip("The amount of dampening used when the new scale is higher than the bar's existing scale.")]
	public float attackDamp = .75f;
	/// <summary>
	/// The amount of dampening used when the new scale is lower than the bar's existing scale. Must be between 0 (slowest) and 1 (fastest).
	/// </summary>
	[Range(0, 1)]
	[Tooltip("The amount of dampening used when the new scale is lower than the bar's existing scale.")]
	public float decayDamp = .25f;
	#endregion

	#region OUTPUT PROPERTIES
	/// <summary>
	/// How the volume data should be presented to the user.
	/// </summary>
	[Tooltip("How the volume data should be presented to the user.")]
	public OutputType outputType = OutputType.PrefabBar;
	/// <summary>
	/// A multiplier / mask to use when object position or rotation is used.
	/// </summary>
	[Tooltip("A multiplier / mask for positioning or rotating. The volume data is multiplied by this vector, so 0 will mask that dimension out.")]
	public Vector3 valueMultiplier = new Vector3(0, 0, -90);
	/// <summary>
	/// The minimum scale when object scaling is used.
	/// </summary>
	[Tooltip("The scale used when output volume is lowest (0).")]
	public float outputScaleMin = 1;
	/// <summary>
	/// The maximum scale when object scaling is used.
	/// </summary>
	[Tooltip("The scale used when output volume is highest (1).")]
	public float outputScaleMax = 1.5f;

	/// <summary>
	/// The prefab of bar to use when building.
	/// Refer to the documentation to use a custom prefab.
	/// </summary>
	[Tooltip("The prefab of bar to use. Use a prefab from SimpleSpectrum/Bar Prefabs or refer to the documentation to use a custom prefab.")]
	public GameObject prefab;
	/// <summary>
	/// Determines whether to scale the bar prefab (i.e. disable for just colouring).
	/// </summary>
	[Tooltip("Determines whether to scale the bar prefab (i.e. disable for just colouring).")]
	public bool scalePrefab = true;
	/// <summary>
	/// Determines whether to apply a color gradient on the bar.
	/// </summary>
	[Tooltip("Determines whether to apply a color gradient on the bar.")]
	public bool useColorGradient = false;
	/// <summary>
	/// The minimum (low value) color when outputType is PrefabBar.
	/// </summary>
	[Tooltip("The minimum (low value) color.")]
	public Color MinColor = Color.black;
	/// <summary>
	/// The maximum (high value) color when outputType is PrefabBar.
	/// </summary>
	[Tooltip("The maximum (high value) color.")]
	public Color MaxColor = Color.white;
	/// <summary>
	/// The curve that determines the interpolation between colorMin and colorMax, when outputType is PrefabBar.
	/// </summary>
	[Tooltip("The curve that determines the interpolation between Color Min and Color Max")]
	public AnimationCurve colorCurve;
	/// <summary>
	/// The amount of dampening used when the new color value is higher than the existing color value. Must be between 0 (slowest) and 1 (fastest).
	/// </summary>
	[Range(0, 1)]
	[Tooltip("The amount of dampening used when the new color value is higher than the existing color value.")]
	public float colorAttackDamp = 1;
	/// <summary>
	/// The amount of dampening used when the new color value is lower than the existing color value. Must be between 0 (slowest) and 1 (fastest).
	/// </summary>
	[Range(0, 1)]
	[Tooltip("The amount of dampening used when the new color value is lower than the existing color value.")]
	public float colorDecayDamp = 1;
	#endregion

	/// <summary>
	/// Sets the input value for scaling. Can only be used when sourceType is Custom. 
	/// </summary>
	public float inputValue
	{
		set
		{
			if (sourceType == SourceType.Custom)
				newValue = value;
			else
				Debug.LogError("Error from OutputVolume: inputValue cannot be set while sourceType is not Custom.");
		}
	}

	/// <summary>
	/// The output value of the scaler, after attack/decay (Read Only).
	/// </summary>
	public float outputValue
	{
		get { return oldScale; }
	}

	//float[] samples;

	GameObject bar;

	Transform barT;

	float newValue;
	float oldScale;
	float oldColorVal = 0;
	Material mat;

	int mat_ValId;
	bool materialColourCanBeUsed = true;

	void Start()
	{
		if (outputType == OutputType.PrefabBar)
		{
			bar = Instantiate(prefab) as GameObject;
			barT = bar.transform;
			barT.SetParent(transform, false);
			barT.localPosition = Vector3.zero;

			Renderer rend = barT.GetChild(0).GetComponent<Renderer>();
			if (rend != null)
			{
				mat = rend.material;
			}
			else
			{
				Image img = barT.GetChild(0).GetComponent<Image>();
				if (img != null)
				{
					img.material = new Material(img.material);
					mat = img.material;
				}
				else
				{
					Debug.LogWarning("Warning from OutputVolume: The Bar Prefab you're using doesn't have a Renderer or Image component as its first child. Dynamic colouring will not work.");
					materialColourCanBeUsed = false;
				}
			}
			mat_ValId = Shader.PropertyToID("_Val");
			mat.SetColor("_Color1", MinColor);
			mat.SetColor("_Color2", MaxColor);
		}
	}

	void Update()
	{
		if (isEnabled && sourceType != SourceType.Custom)
		{
			if (sourceType == SourceType.AudioListener)
				newValue = GetRMS(sampleAmount, channel);
			else
				newValue = GetRMS(audioSource, sampleAmount, channel);
		}

		float newScale = newValue > oldScale ? Mathf.Lerp(oldScale, newValue, attackDamp) : Mathf.Lerp(oldScale, newValue, decayDamp);

		oldScale = newScale;

		switch (outputType)
		{
			case OutputType.PrefabBar:
				if (scalePrefab)
					barT.localScale = new Vector3(1, newScale, 1);

				if (useColorGradient && materialColourCanBeUsed)
				{
					float newColorVal = colorCurve.Evaluate(newScale);
					if (newColorVal > oldColorVal) //it's attacking
					{
						if (colorAttackDamp != 1)
						{
							newColorVal = Mathf.Lerp(oldColorVal, newColorVal, colorAttackDamp);
						}
					}
					else //it's decaying
					{
						if (colorDecayDamp != 1)
						{
							newColorVal = Mathf.Lerp(oldColorVal, newColorVal, colorDecayDamp);
						}
					}
					mat.SetFloat(mat_ValId, newColorVal);
					oldColorVal = newColorVal;
				}
				break;

			case OutputType.ObjectPosition:
				transform.localPosition = valueMultiplier * newScale;
				break;

			case OutputType.ObjectRotation:
				transform.localEulerAngles = valueMultiplier * newScale;
				break;

			case OutputType.ObjectScale:
				float s = Mathf.Lerp(outputScaleMin, outputScaleMax, newScale);
				transform.localScale = new Vector3(s, s, s);
				break;
		}
	}

	/// <summary>
	/// Returns the current output volume of the specified AudioSource, using the RMS method.
	/// </summary>
	/// <param name="aSource">The AudioSource to reference.</param>
	/// <param name="sampleSize">The number of samples to take, as a power of two. Higher values mean more precise volume.</param>
	/// <param name="channelUsed">The audio channel to take data from.</param>
	public static float GetRMS(AudioSource aSource, int sampleSize, int channelUsed = 0)
	{
		sampleSize = Mathf.ClosestPowerOfTwo(sampleSize);
		float[] outputSamples = new float[sampleSize];
		aSource.GetOutputData(outputSamples, channelUsed);

		float rms = 0;
		foreach (float f in outputSamples)
		{
			rms += f * f; //sum of squares
		}
		return Mathf.Sqrt(rms / (outputSamples.Length)); //mean and root
	}

	/// <summary>
	/// Returns the current output volume of the scene's AudioListener, using the RMS method.
	/// </summary>
	/// <param name="sampleSize">The number of samples to take, as a power of two. Higher values mean more precise volume.</param>
	/// <param name="channelUsed">The audio channel to take data from.</param>
	public static float GetRMS(int sampleSize, int channelUsed = 0)
	{
		sampleSize = Mathf.ClosestPowerOfTwo(sampleSize);
		float[] outputSamples = new float[sampleSize];
		AudioListener.GetOutputData(outputSamples, channelUsed);

		float rms = 0;
		foreach (float f in outputSamples)
		{
			rms += f * f; //sum of squares
		}
		return Mathf.Sqrt(rms / (outputSamples.Length)); //mean and root
	}
}
