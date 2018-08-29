using UnityEngine;
using UnityEngine.Events;

public class SetVector3 : MonoBehaviour
{
	[System.Serializable]
	public class Vector3Event : UnityEvent<Vector3> { }
	public Vector3 baseValue;
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
	public Vector3Event output;


	public void AddValueXYZ(float value)
	{
		var newValue = baseValue + Vector3.one * value * _multiplier;
		 output.Invoke(newValue);
	}

	public void AddValueX(float value)
	{
		var newValue = baseValue + Vector3.right * value * _multiplier;
		 output.Invoke(newValue);
	}

	public void AddValueY(float value)
	{
		var newValue = baseValue + Vector3.up * value * _multiplier;
		 output.Invoke(newValue);
	}

	public void AddValueZ(float value)
	{
		var newValue = baseValue + Vector3.forward * value * _multiplier;
		 output.Invoke(newValue);
	}


	public void MultiplyrValueXYZ(float value)
	{
		var newValue = baseValue * value * _multiplier;
		 output.Invoke(newValue);
	}

	public void MultiplyrValueX(float value)
	{
		var newValue = baseValue;
		newValue.x *= value * _multiplier;
		 output.Invoke(newValue);
	}

	public void MultiplyrValueY(float value)
	{
		var newValue = baseValue;
		newValue.y *= value * _multiplier;
		 output.Invoke(newValue);
	}

	public void MultiplyrValueZ(float value)
	{
		var newValue = baseValue;
		newValue.z *= value * _multiplier;
		 output.Invoke(newValue);
	}
}
