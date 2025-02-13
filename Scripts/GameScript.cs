using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScript : MonoBehaviour
{
    public static GameScript Instance;
    void Awake()
    {
        Instance = this;
    }
    public GameObject controlBlock;
    public GameObject targetPanel;

    public List<GameObject> levels;
    public Color[] colors;
    public GameObject[] items; //baslangic -> tek bir item cesidi -> kirilacak item??
    public ItemScript selectedItem;
    public List<ItemScript> selectedItems;
    public List<BoxScript> boxes;
    public Vector3 newItemPos;
    public float blockSpeed;
    public ItemScript newItem;
    public bool completed;
    public bool failed;
    public GameObject completedScreen;
    public GameObject failedScreen;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI moveText;
    public AudioSource popSound;
    public AudioSource glassSound;
    public int tempMoveCount;
    public GameObject boxSprite;
    public List<Vector3> targetPosList;


    void Start()
    {
        //LEVEL 1'DEN BASLAR
        if(PlayerPrefs.GetInt("levelIndex")==0)
        {
            PlayerPrefs.SetInt("levelIndex",1);
        }

        //LEVEL LOOP ALMA
        if(PlayerPrefs.GetInt("level")>=GameScript.Instance.levels.Count)
        {
            PlayerPrefs.SetInt("level",1); //level 4'ten sonrasini loop 
        }
        
        levels[PlayerPrefs.GetInt("level")].SetActive(true);

        //Levelden move sayisini alir
        tempMoveCount = levels[PlayerPrefs.GetInt("level")].GetComponent<LevelScript>().moveCount;
        moveText.SetText("MOVE : " + tempMoveCount);
        
        levelText.SetText("LEVEL " + PlayerPrefs.GetInt("levelIndex"));
    }

    void Update()
    {
        if(completed)
        {
            if(Input.GetMouseButtonDown(0))
            {
                PlayerPrefs.SetInt("levelIndex",PlayerPrefs.GetInt("levelIndex")+1);
                SceneManager.LoadScene(0);
            }
            return;
        }
        if(failed)
        { 
            if(Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene(0);
            }
            return;
        }

        GetGrids();

        if(newItem)
        {
            newItem.goDown = true;
            //0.5f ilerledikten sonra diger itemlari da olustursun diye
            if(Vector3.Distance(newItem.transform.position,newItemPos + new Vector3(newItem.GetComponentInParent<GridScript>().transform.position.x,0,0)) >= 0.5f)
            {
                newItem = null;
            }
        }
    }

    void GetGrids() 
    {
        if (Input.GetMouseButtonDown(0) && !selectedItem && !newItem) //bir item secili degilse
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit) && hit.collider.GetComponent<GridScript>() && hit.collider.GetComponent<GridScript>().item.GetComponent<ItemScript>())
            {
                //bir grid secildi -> selected Item listesine eklendi
                selectedItem = hit.collider.GetComponent<GridScript>().item.GetComponent<ItemScript>();
                selectedItems.Add(selectedItem);
                CheckNeighbours();
            }
        }
    }
    void CheckNeighbours()
    {
        foreach(ItemScript item in selectedItems) //listedeki her bir elemanin
        {
            foreach(GridScript neighbour in item.GetComponentInParent<GridScript>().neighbours) //komsulari
            {
                if(neighbour && neighbour.item && neighbour.item.GetComponent<ItemScript>() && neighbour.item.GetComponent<ItemScript>().colorIndex == selectedItem.colorIndex && !selectedItems.Contains(neighbour.item.GetComponent<ItemScript>()))
                {
                    //yeni bir item eklendi
                    selectedItems.Add(neighbour.item.GetComponent<ItemScript>());
                    CheckNeighbours();
                    return;
                }
                else if(neighbour && neighbour.item && neighbour.item.GetComponent<BoxScript>() && !boxes.Contains(neighbour.item.GetComponent<BoxScript>())) //item degilse -> bir box ise
                {
                    boxes.Add(neighbour.item.GetComponent<BoxScript>());
                    CheckNeighbours();
                    return;
                }
            }
        }
        //butun itemlar eklendi
        if(selectedItems.Count >= 2)
        {
            tempMoveCount--;
            moveText.SetText("MOVE : "+ tempMoveCount);
            levels[PlayerPrefs.GetInt("level")].GetComponent<LevelScript>().CheckSelectedItems();
        }
        else //secili item sayisi <2 ise hicibr sey yapma
        {
            selectedItem = null;
            selectedItems.Clear();
            boxes.Clear();
        }
    }


}
