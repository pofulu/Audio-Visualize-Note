using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMaterialColor : MonoBehaviour
{
    public Renderer[] meshes;
    public int materialIndex = 0;
    public string propertyName = "_Color";
    public Color color = UnityEngine.Color.white;

    private void Update()
    {
        foreach (var mesh in meshes)
        {
            mesh.materials[materialIndex].SetColor(propertyName, color);
        }
    }

    public void Color(Color color)
    {
        foreach (var mesh in meshes)
        {
            mesh.materials[materialIndex].SetColor(propertyName, color);
        }
    }

    public void Red(float value)
    {
        foreach (var mesh in meshes)
        {
            var color = mesh.materials[materialIndex].color;
            color.r = value;
            mesh.materials[materialIndex].SetColor(propertyName, color);
        }
    }

    public void Green(float value)
    {
        foreach (var mesh in meshes)
        {
            var color = mesh.materials[materialIndex].color;
            color.g = value;
            mesh.materials[materialIndex].SetColor(propertyName, color);
        }
    }

    public void Blue(float value)
    {
        foreach (var mesh in meshes)
        {
            var color = mesh.materials[materialIndex].color;
            color.b = value;
            mesh.materials[materialIndex].SetColor(propertyName, color);
        }
    }

    public void Alhpa(float value)
    {
        foreach (var mesh in meshes)
        {
            var color = mesh.materials[materialIndex].color;
            color.a = value;
            mesh.materials[materialIndex].SetColor(propertyName, color);
        }
    }
}
