using Unity.VisualScripting;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public int colorIndex;
    public Animator anim;
    public bool goDown = true;


    void Start()
    {
        SetColor();
    }
    
    void Update()
    {
        if(goDown) SetPos();
    }

    void SetColor()
    {
        GetComponentInChildren<MeshRenderer>().material.color = GameScript.Instance.colors[colorIndex];
    }
    
    void SetPos() // neden false oluyor?
    {
        transform.position = Vector3.MoveTowards(transform.position, GetComponentInParent<GridScript>().transform.position, GameScript.Instance.blockSpeed * Time.deltaTime);
        if (transform.position == GetComponentInParent<GridScript>().transform.position)
        {
            goDown = false;
        }     
    }
    
}
