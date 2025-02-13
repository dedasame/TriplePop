using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GridScript : MonoBehaviour
{
    public GridScript[] neighbours; //komsular
    public GameObject item; //itemi
    public int itemIndex; //ne tur item
    public int startColor; //baslangic level ayari
    public bool checkedUpNeighbours = true;
    public System.Random rand;


    void Start()
    {
        rand = new System.Random();
        GetNeighbours();
        item = Instantiate(GameScript.Instance.items[itemIndex],transform.position, Quaternion.identity,transform);
        if(itemIndex==0)
        {
            item.GetComponent<ItemScript>().colorIndex = startColor%GameScript.Instance.colors.Count();
        }
        else if(itemIndex == 1)
        {
            item.GetComponent<BoxScript>().colorIndex = startColor%GameScript.Instance.colors.Count();
        }
    }

    void Update()
    {
        if(!item)
        {
            checkedUpNeighbours = false;
            CheckUp();
        }
    }

    void GetNeighbours()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, Vector3.up,out hit, 1f, 1 << 6)) neighbours[0] = hit.collider.gameObject.GetComponent<GridScript>();
        if(Physics.Raycast(transform.position, Vector3.right,out hit, 1f, 1 << 6)) neighbours[1] = hit.collider.gameObject.GetComponent<GridScript>();
        if(Physics.Raycast(transform.position, Vector3.down,out hit, 1f, 1 << 6)) neighbours[2] = hit.collider.gameObject.GetComponent<GridScript>();
        if(Physics.Raycast(transform.position, Vector3.left,out hit, 1f, 1 << 6)) neighbours[3] = hit.collider.gameObject.GetComponent<GridScript>();
    }
    void CreateItem()
    {
        if(!GameScript.Instance.newItem)
        {
            startColor = RandomColor(rand);
            item = Instantiate(GameScript.Instance.items[0],GameScript.Instance.newItemPos + new Vector3(transform.position.x,0,0), Quaternion.identity,transform);
            item.GetComponent<ItemScript>().colorIndex = startColor;
            item.GetComponent<ItemScript>().goDown = true;
            GameScript.Instance.newItem = item.GetComponent<ItemScript>();
        }
        else
        {
            GameScript.Instance.newItem.goDown = true;
        } 
    }

    void CheckUp()
    {
        if(!neighbours[0]) //ustte hic item yoksa direkt olustur gec
        {
            CreateItem();
            return;
        }
        GridScript neighbour = neighbours[0];
        while(neighbour)
        {
            if(neighbour.item && neighbour.item.GetComponent<ItemScript>())//bir item bulundu
            {
                item = neighbour.item;
                //parenti bu olacak
                item.gameObject.transform.parent = this.transform;
                item.GetComponent<ItemScript>().goDown = true;
                neighbour.item = null;
                return;
            }
            else if(neighbour.item && !neighbour.item.GetComponent<ItemScript>()) 
            {
                checkedUpNeighbours = true;
                return; //item degilse
            }
            else // item -> scripti farklı ise
            {
                neighbour = neighbour.neighbours[0];    
            }
        }
        //item yok olustur
        CreateItem();
    }

    int RandomColor(System.Random rand)
    {
        int[] values = GetComponentInParent<LevelScript>().values;
        //int[] values = {0,1,2};
        return values[rand.Next(values.Length)];
    }

    public void CheckItemPos()
    {
        if(item.transform.position != transform.position && item.GetComponent<ItemScript>())
        {
            item.GetComponent<ItemScript>().goDown = true;
        }
    }
}
