using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    public Animator anim;
    public int colorIndex;

    void Start()
    {
        colorIndex = transform.GetComponentInParent<GridScript>().startColor;
        SetColor();
    }

    void SetColor()
    {
        GetComponentInChildren<MeshRenderer>().material.color = GameScript.Instance.colors[colorIndex];
    }
}
