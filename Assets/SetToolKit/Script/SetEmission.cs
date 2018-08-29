using UnityEngine;

public class SetEmission : MonoBehaviour
{
	[SerializeField]
	private Color _color;
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
	public Color color
	{
		get
		{
			return _color;
		}

		set
		{
			_color = value;
		}
	}

	Material mat;

	private void Start()
	{
		mat = GetComponent<Renderer>().material;
	}

	public void Emission(float emission)
	{
		Color baseColor = _color;
		Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission * _multiplier);
		mat.SetColor("_EmissionColor", finalColor);
	}

	public void SetColorRed(float r)
	{
		_color.r = r * _multiplier;
		mat.SetColor("_EmissionColor", _color);
	}
	public void SetColorGreen(float g)
	{
		_color.g = g * _multiplier;
		mat.SetColor("_EmissionColor", _color);
	}
	public void SetColorBlue(float b)
	{
		_color.b = b * _multiplier;
		mat.SetColor("_EmissionColor", _color);
	}
}
