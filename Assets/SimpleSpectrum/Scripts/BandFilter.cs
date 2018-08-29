using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BandFilter : MonoBehaviour
{
	[System.Serializable]
	public class FloatEvent : UnityEvent<float> { }
	[System.Serializable]
	public class BooleanEvent : UnityEvent<bool> { }

	public enum Band
	{
		SubBass = 1 << 0,
		Bass = 1 << 1,
		LowMidrange = 1 << 2,
		Midrange = 1 << 3,
		UpperMidrangeLow = 1 << 4,
		UpperMidrangeHigh = 1 << 5,
		Presence = 1 << 6,
		Brilliance = 1 << 7,
	}

	public SimpleSpectrum simpleSpectrum;

	[EnumFlag] public Band filter = Band.Bass;
	public float bandValue;
	[Range(0, 1)] public float attackDamp = 0.25f;
	[Range(0, 1)] public float decayDamp = 0.25f;
	public FloatEvent floatEvent;
	[SerializeField]
	private float _triggerThreshold;
	public BooleanEvent triggerEvent;

	private float[] freqBand = new float[8];
	private float[] newValue = new float[8];
	private float[] oldScale = new float[8];
	private bool isTriggered;

	public float triggerThreshold
	{
		get
		{
			return _triggerThreshold;
		}

		set
		{
			_triggerThreshold = value;
		}
	}

	#region Public method

	public float GetBand()
	{
		return GetBand(filter);
	}

	public float GetBand(Band filter)
	{
		List<int> index = new List<int>();

		if ((Band.SubBass & filter) == Band.SubBass)
			index.Add(0);

		if ((Band.Bass & filter) == Band.Bass)
			index.Add(1);

		if ((Band.LowMidrange & filter) == Band.LowMidrange)
			index.Add(2);

		if ((Band.Midrange & filter) == Band.Midrange)
			index.Add(3);

		if ((Band.UpperMidrangeLow & filter) == Band.UpperMidrangeLow)
			index.Add(4);

		if ((Band.UpperMidrangeHigh & filter) == Band.UpperMidrangeHigh)
			index.Add(5);

		if ((Band.Presence & filter) == Band.Presence)
			index.Add(6);

		if ((Band.Brilliance & filter) == Band.Brilliance)
			index.Add(7);

		return GetAmplitude(index.ToArray());
	}

	#endregion

	#region Private method

	private void OnValidate()
	{
		if (simpleSpectrum)
			simpleSpectrum.numSamples = 512;
	}

	private void Update()
	{
		MakeFrequencyBands();

		bandValue = GetBand();
		floatEvent.Invoke(bandValue);

		if (bandValue >= triggerThreshold && !isTriggered)
		{
			isTriggered = true;
			triggerEvent.Invoke(true);
		}
		else if (bandValue < triggerThreshold && isTriggered)
		{
			isTriggered = false;
			triggerEvent.Invoke(false);
		}
	}

	private void MakeFrequencyBands()
	{
		int count = 0;
		for (int i = 0; i < freqBand.Length; i++)
		{
			float average = 0;
			int sampleCount = (int)Mathf.Pow(2, i) * 2;

			if (i == 7)
			{
				sampleCount += 2;
			}

			for (int j = 0; j < sampleCount; j++)
			{
				average += simpleSpectrum.spectrumInputData[count] * (count + 1);
				count++;
			}
			average /= count;
			freqBand[i] = average * 10;

			newValue[i] = freqBand[i];
			float newScale = newValue[i] > oldScale[i] ? Mathf.Lerp(oldScale[i], newValue[i], attackDamp) : Mathf.Lerp(oldScale[i], newValue[i], decayDamp);
			freqBand[i] = newScale;
			oldScale[i] = newScale;
		}
	}

	private float GetAmplitude(int[] index)
	{
		if (index.Length == 0) return 0;

		float currentAmplitude = 0;

		for (int i = 0; i < index.Length; i++)
		{
			currentAmplitude += freqBand[index[i]];
		}

		currentAmplitude /= index.Length;

		return currentAmplitude;
	}

	#endregion
}
