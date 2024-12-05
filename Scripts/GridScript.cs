using UnityEngine;

public class GridScript : MonoBehaviour
{
    public GridScript[] neighbours; //komsular
    public ItemScript item; //itemi
    public int itemIndex; //ne tur item
    public int startColor; //baslangic level ayari
    public bool checkedUpNeighbours = true;
    public System.Random rand;


    void Start()
    {
        rand = new System.Random();
        GetNeighbours();
        item = Instantiate(GameScript.Instance.items[itemIndex],transform.position, Quaternion.identity,transform).GetComponent<ItemScript>();
        item.colorIndex = startColor%3;
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
            //GameScript.Instance.blupSound.Play();
            startColor = RandomColor(rand);
            item = Instantiate(GameScript.Instance.items[itemIndex],GameScript.Instance.newItemPos + new Vector3(transform.position.x,0,0), Quaternion.identity,transform).GetComponent<ItemScript>();
            item.colorIndex = startColor;
            item.goDown = true;
            GameScript.Instance.newItem = item;
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
            if(neighbour.item)//bir item bulundu
            {
                item = neighbour.item;
                //parenti bu olacak
                item.gameObject.transform.parent = this.transform;
                item.goDown = true;
                neighbour.item = null;
                return;
            }
            else
            {
                neighbour = neighbour.neighbours[0];
            }
        }
        //item yok olustur
        CreateItem();
    }

    int RandomColor(System.Random rand)
    {
        int[] values = {0,1,2};
        return values[rand.Next(values.Length)];
    }
}
