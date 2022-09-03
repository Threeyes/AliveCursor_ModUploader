using NaughtyAttributes;
using SplineMesh;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoop_Controller : MonoBehaviour
{
    public List<Vector3> listDefaultNodePosition = new List<Vector3>();
    public List<Vector3> listSqueezeNodePosition = new List<Vector3>();

    public Spline spline_Left;
    [Button("SaveDefaultInfo")]
    public void SaveDefaultInfo()
    {
        listDefaultNodePosition.Clear();
        foreach (var node in spline_Left.nodes)
        {
            listDefaultNodePosition.Add(node.Position);
        }
    }
    [Button("SaveSqueezeInfo")]
    public void SaveSqueezeInfo()
    {
        listSqueezeNodePosition.Clear();
        foreach (var node in spline_Left.nodes)
        {
            listSqueezeNodePosition.Add(node.Position);
        }
    }

}
