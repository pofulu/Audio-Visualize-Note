using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ValueMapping : MonoBehaviour
{
    [System.Serializable]
    public class MappingEvent : UnityEvent<float> { }

    public Vector2 origin = new Vector2(1, 2);
    public Vector2 map = new Vector2(0, 1);

    [Header("Print")]
    public float input;
    public float output;

    [Header("Event")]
    public MappingEvent Output;

    public void Mapping(float value)
    {
        input = value;
        output = ClampedMap(value, origin.x, origin.y, map.x, map.y);
        Output.Invoke(output);
    }

    private float ClampedMap(float value, float low1, float high1, float low2, float high2)
    {
        if (low2 > high2)
        {
            return Mathf.Clamp(low2 + (value - low1) * (high2 - low2) / (high1 - low1), high2, low2);
        }
        else
        {
            return Mathf.Clamp(low2 + (value - low1) * (high2 - low2) / (high1 - low1), low2, high2);
        }
    }
}
