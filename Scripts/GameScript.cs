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
    public List<GameObject> levels;
    public Color[] colors;
    public GameObject[] items; //baslangic -> tek bir item cesidi -> kirilacak item??
    public ItemScript selectedItem;
    public GridScript checkGrid;
    public List<ItemScript> selectedItems;
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
    public AudioSource blupSound;

    public int tempMoveCount;


    void Start()
    {
        if(PlayerPrefs.GetInt("levelIndex")==0)
        {
            PlayerPrefs.SetInt("levelIndex",1);
        }
        
        levels[PlayerPrefs.GetInt("level")].SetActive(true);

        tempMoveCount = levels[PlayerPrefs.GetInt("level")].GetComponent<TargetScript>().moveCount;
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
                completed = false;
                SceneManager.LoadScene(0);
            }
            return;
        }
        if(failed)
        {
            
            if(Input.GetMouseButtonDown(0))
            {
                completed = false;
                SceneManager.LoadScene(0);
            }
            return;
        }

        GetGrids();

        if(newItem)
        {
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

            if(Physics.Raycast(ray, out hit) && hit.collider.GetComponent<GridScript>())
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
                if(neighbour && neighbour.item && neighbour.item.colorIndex == selectedItem.colorIndex && !selectedItems.Contains(neighbour.item))
                {
                    //yeni bir item eklendi
                    selectedItems.Add(neighbour.item);
                    CheckNeighbours();
                    return;
                }
            }
        }
        //butun itemlar eklendi
        if(selectedItems.Count >= 2) DestroyItems();
        else
        {
            selectedItem = null;
            selectedItems.Clear();
        }
    }

    void DestroyItems()
    {
        tempMoveCount--;
        moveText.SetText("MOVE : "+ tempMoveCount);
        
        checkGrid = selectedItem.GetComponentInParent<GridScript>();
        foreach(ItemScript item in selectedItems)
        {
            item.anim.SetTrigger("pop"); //item siliniyor
            item.GetComponentInParent<GridScript>().item = null;
            popSound.Play();

        }
        //patlatilan item sayisi?
        if(selectedItem.colorIndex == TargetScript.Instance.targetColor)
        {
            TargetScript.Instance.temp += selectedItems.Count;
            TargetScript.Instance.SetText();
        }

        if(tempMoveCount == 0)
        {
            failed = true;
            failedScreen.SetActive(true);
            failedScreen.GetComponentInChildren<Animator>().SetTrigger("anim");
        }

        //butun itemlar yerine yerlesti ve yeni itemlar olustuktan sonra:
        selectedItems.Clear();
        selectedItem = null;
    }

}
