using UnityEngine;
using UnityEngine.Events;

public class SetColor : MonoBehaviour
{
	[System.Serializable]
	public class ColorEvent : UnityEvent<Color> { }
	[SerializeField]
	private Color _baseColor;
	[SerializeField]
	private float _multiplier = 1;

	public float multiplier
	{
		get
		{
			return _multiplier;
		}

		set
		{
			_multiplier = value;
		}
	}
	public Color baseColor
	{
		get
		{
			return _baseColor;
		}

		set
		{
			_baseColor = value;
		}
	}
	public ColorEvent colorOutput;

	public void SetRed(float r)
	{
		var newColor = _baseColor;
		newColor.r = r * _multiplier;
		colorOutput.Invoke(newColor);
	}

	public void SetGreen(float g)
	{
		var newColor = _baseColor;
		newColor.g = g * _multiplier;
		colorOutput.Invoke(newColor);
	}

	public void SetBlue(float b)
	{
		var newColor = _baseColor;
		newColor.b = b * _multiplier;
		colorOutput.Invoke(newColor);
	}

	public void SetAlpha(float a)
	{
		var newColor = _baseColor;
		newColor.a = a * _multiplier;
		colorOutput.Invoke(newColor);
	}
}
