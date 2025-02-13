using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    public GameObject blockControl;
    public int itemIndex;
    public int color;
    public int targetVal;
    public int temp = 0;
    public bool done;
    public Vector3 position;
    public List<ItemScript> moveItems;
    

    public void SetColor() 
    {
        if(itemIndex != 1) blockControl.GetComponentInChildren<UnityEngine.UI.Image>().color = GameScript.Instance.colors[color];
        //block degil ise -> kutu resmi? -> kutu rengi??????
        else blockControl.GetComponentInChildren<UnityEngine.UI.Image>().color = GameScript.Instance.colors[color];
    }

    void Update()
    {
        if(moveItems.Count>0) MoveItem();
    }


    //ITEM TARGETA EKLER
    public void AddItem(Vector3 targetPos) 
    {
        position = targetPos;//targetin pozisyonu
        foreach(ItemScript item in GameScript.Instance.selectedItems)
        {
            if(!moveItems.Contains(item)) moveItems.Add(item);
            item.GetComponentInParent<GridScript>().item = null;
            item.anim.SetTrigger("target");
        }
    }
    public void MoveItem()
    {
        foreach(ItemScript item in moveItems)
        {
            item.transform.position = Vector3.MoveTowards(item.transform.position,position,10f*Time.deltaTime);
            if(Vector3.Distance(item.transform.position,position)<=.5f)
            {
                temp++;
                SetText();
                GameScript.Instance.popSound.Play();
                item.gameObject.SetActive(false);
                moveItems.Remove(item);
                if(moveItems.Count == 0) TargetCheck();
            }
        }
    }


    //BOX TARGETA EKLER
    public void AddBox()
    {
        temp += GameScript.Instance.boxes.Count;
        SetText();
        foreach(BoxScript box in GameScript.Instance.boxes)
        {
            box.GetComponentInParent<GridScript>().item = null;
            box.gameObject.SetActive(false);
        }
        GameScript.Instance.glassSound.Play();
        TargetCheck();
    }
    
    //EKLENEN ITEM SAYISINI AYARLAR
    public void SetText()
    {
        if(temp>=targetVal)
        {
            done = true;
            blockControl.GetComponentInChildren<TextMeshProUGUI>().SetText(" DONE ");
            blockControl.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;

        }
        else blockControl.GetComponentInChildren<TextMeshProUGUI>().SetText(targetVal-temp+"");
    }

    public void TargetCheck() //butun itemlar yerine gittikten sonra
    {
        //Level Completed Kontrolu
        transform.GetComponent<LevelScript>().LevelCompleteCheck();
    }

   
    
    
    
}
