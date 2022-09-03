using NaughtyAttributes;
using SplineMesh;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoop_Controller : MonoBehaviour
{
    public Spline spline_Left;
    public Spline spline_Right;

    [Header("Cache")]
    public List<SplineNode> listDefaultNode = new List<SplineNode>();
    public List<SplineNode> listSqueezeNode = new List<SplineNode>();

    #region Invoke by extern (CIB/CSB)
    public void OnMouseDown(bool isDown)
    {
        //PS:因为SplineNode里面是通过调用event进行更新，因此不能直接赋值
        for (int i = 0; i != spline_Left.nodes.Count; i++)
        {
            var left_node = spline_Left.nodes[i];
            var right_node = spline_Right.nodes[i];
            left_node.Position = right_node.Position = isDown ? listSqueezeNode[i].Position : listDefaultNode[i].Position;
            left_node.Direction = right_node.Direction = isDown ? listSqueezeNode[i].Direction : listDefaultNode[i].Direction;
        }
    }

    public void OnSqueezeProcess(float percent)
    {
        for (int i = 0; i != spline_Left.nodes.Count; i++)
        {
            var left_node = spline_Left.nodes[i];
            var right_node = spline_Right.nodes[i];
            left_node.Position = right_node.Position = Vector3.Lerp(listDefaultNode[i].Position, listSqueezeNode[i].Position, percent);
            left_node.Direction = right_node.Direction = Vector3.Lerp(listDefaultNode[i].Direction, listSqueezeNode[i].Direction, percent);
        }
    }
    #endregion

#if UNITY_EDITOR
    [Button("SaveDefaultInfo")]
    public void SaveDefaultInfo()
    {
        listDefaultNode = spline_Left.nodes;
    }
    [Button("SaveSqueezeInfo")]
    public void SaveSqueezeInfo()
    {
        listSqueezeNode = spline_Left.nodes;
    }
#endif
}
